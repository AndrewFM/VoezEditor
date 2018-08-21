using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class ProjectData {
    public Texture2D background;
    public AudioClip songClip;
    public List<NoteData> notes;
    public List<TrackData> tracks;
    public string projectFolder;
    public string notesFileName;
    public string tracksFileName;
    public string infoFileName;
    public string infoString;
    public int songBPM = 120;

    public ProjectData()
    {
        projectFolder = Application.dataPath + "/../ActiveProject";
        notesFileName = projectFolder + "/note_default.txt";
        tracksFileName = projectFolder + "/track_default.txt";
        notes = new List<NoteData>();
        tracks = new List<TrackData>();
    }

    public void DeleteNote(int ID)
    {
        for(int i=0; i<notes.Count; i+=1) {
            if (notes[i].id == ID) {
                notes.RemoveAt(i);
                break;
            }
        }
    }

    public void AddNote(NoteData data)
    {
        notes.Add(data);
    }

    // Load Project from disk
    public void LoadFromActiveProject()
    {
        string[] projectFiles = Directory.GetFiles(projectFolder, "*.*", SearchOption.TopDirectoryOnly);
        for (int i = 0; i < projectFiles.Length; i += 1) {
            // Audio File
            if (projectFiles[i].Contains("song_") && projectFiles[i].Contains(".wav")) {
                WWW www = new WWW("file://" + projectFiles[i]);
                songClip = www.GetAudioClip(false, false, AudioType.WAV);
            }

            // Background Image File
            if (projectFiles[i].Contains("image_") && projectFiles[i].Contains(".png")) {
                byte[] fileData = File.ReadAllBytes(projectFiles[i]);
                background = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                background.LoadImage(fileData);
            }

            // Info File
            if (projectFiles[i].Contains("info_")) {
                infoFileName = projectFiles[i];
                infoString = File.ReadAllText(projectFiles[i]);
                if (infoString.Contains("\"bpm\"")) {
                    string bpmPart = infoString.Substring(infoString.IndexOf("\"bpm\""));
                    int bpmStartInd = bpmPart.IndexOf(":");
                    int bpmEndInd = bpmPart.IndexOf(",");
                    songBPM = int.Parse(bpmPart.Substring(bpmStartInd + 1, bpmEndInd - bpmStartInd - 1));
                    songBPM = Mathf.Clamp(songBPM, 10, 250);
                }
            }

            // Notes Mapping File
            if (projectFiles[i].Contains("note_")) {
                notesFileName = projectFiles[i];
                string notesString = File.ReadAllText(projectFiles[i]);
                notesString = notesString.Substring(1, notesString.Length - 2);
                notesString = Regex.Replace(notesString, @"\t|\n|\r", "");
                string[] indNotes = Regex.Split(notesString, @"\},");
                for (int j = 0; j < indNotes.Length; j += 1) {
                    string ind = indNotes[j] + (indNotes[j].EndsWith("}") ? "" : "}");
                    Dictionary<string, object> noteProperties = (Dictionary<string, object>)Json.Deserialize(ind);
                    NoteData newNote = new NoteData();
                    newNote.id = (int)((long)noteProperties["Id"]);
                    newNote.track = (int)((long)noteProperties["Track"]);
                    newNote.time = Util.ParseJSONFloat(noteProperties["Time"]);
                    newNote.dir = (int)((long)noteProperties["Dir"]);
                    newNote.hold = Util.ParseJSONFloat(noteProperties["Hold"]);
                    string type = (string)noteProperties["Type"];
                    if (type == "click")
                        newNote.type = NoteData.NoteType.CLICK;
                    else if (type == "hold")
                        newNote.type = NoteData.NoteType.HOLD;
                    else if (type == "swipe")
                        newNote.type = NoteData.NoteType.SWIPE;
                    else if (type == "slide")
                        newNote.type = NoteData.NoteType.SLIDE;
                    notes.Add(newNote);
                }
            }

            // Tracks Mapping File
            if (projectFiles[i].Contains("track_")) {
                tracksFileName = projectFiles[i];
                string tracksString = File.ReadAllText(projectFiles[i]);
                tracksString = tracksString.Substring(1, tracksString.Length - 2);
                tracksString = Regex.Replace(tracksString, @"\t|\n|\r", "");
                string[] indTracks = Regex.Split(tracksString, @"\]\},");
                for (int j = 0; j < indTracks.Length; j += 1) {
                    string ind = indTracks[j];
                    ind = ind.Substring(0, ind.IndexOf(",\"Move\""));
                    ind += (ind.EndsWith("}") ? "" : "}");
                    Dictionary<string, object> basicTrackProperties = (Dictionary<string, object>)Json.Deserialize(ind);

                    TrackData newTrack = new TrackData();
                    newTrack.id = (int)((long)basicTrackProperties["Id"]);
                    newTrack.x = Util.ParseJSONFloat(basicTrackProperties["X"]);
                    newTrack.size = Util.ParseJSONFloat(basicTrackProperties["Size"]);
                    newTrack.start = Util.ParseJSONFloat(basicTrackProperties["Start"]);
                    newTrack.end = Util.ParseJSONFloat(basicTrackProperties["End"]);
                    newTrack.color = (int)((long)basicTrackProperties["Color"]);
                    if (basicTrackProperties.ContainsKey("PositionLock"))
                        newTrack.positionLock = (bool)basicTrackProperties["PositionLock"];
                    if (basicTrackProperties.ContainsKey("EntranceOn"))
                        newTrack.entranceOn = (bool)basicTrackProperties["EntranceOn"];

                    ind = indTracks[j];
                    int moveStart = ind.IndexOf("[");
                    int moveEnd = ind.IndexOf("]");
                    if (moveEnd - moveStart > 1) {
                        string moveString = ind.Substring(moveStart + 1, moveEnd - moveStart - 1);
                        newTrack.move = parseTransformationList(moveString);
                    }

                    ind = ind.Substring(moveEnd+1);
                    int scaleStart = ind.IndexOf("[");
                    int scaleEnd = ind.IndexOf("]");
                    if (scaleEnd - scaleStart > 1) {
                        string scaleString = ind.Substring(scaleStart + 1, scaleEnd - scaleStart - 1);
                        newTrack.scale = parseTransformationList(scaleString);
                    }

                    ind = ind.Substring(scaleEnd + 1);
                    ind += (ind.EndsWith("]") ? "" : "]");
                    int colorStart = ind.IndexOf("[");
                    int colorEnd = ind.IndexOf("]");
                    if (colorEnd - colorStart > 1) {
                        string colorString = ind.Substring(colorStart + 1, colorEnd - colorStart - 1);
                        newTrack.colorChange = parseTransformationList(colorString);
                    }
                    tracks.Add(newTrack);
                }
            }
        }
    }

    private List<TrackTransformation> parseTransformationList(string jsonList)
    {
        List<TrackTransformation> transformList = new List<TrackTransformation>();
        string[] transforms = Regex.Split(jsonList, @"\},");
        for (int k = 0; k < transforms.Length; k += 1) {
            string trans = transforms[k];
            trans += (trans.EndsWith("}") ? "" : "}");
            Dictionary<string, object> transProperties = (Dictionary<string, object>)Json.Deserialize(trans);
            TrackTransformation transObj = new TrackTransformation();
            transObj.to = Util.ParseJSONFloat(transProperties["To"]);
            transObj.start = Util.ParseJSONFloat(transProperties["Start"]);
            transObj.end = Util.ParseJSONFloat(transProperties["End"]);
            string easeStyle = (string)transProperties["Ease"];
            if (easeStyle == "easelinear")
                transObj.ease = Easing.LINEAR;
            else if (easeStyle == "easeinexpo")
                transObj.ease = Easing.EXP_IN;
            else if (easeStyle == "easeoutexpo")
                transObj.ease = Easing.EXP_OUT;
            else if (easeStyle == "easeinquad")
                transObj.ease = Easing.QUAD_IN;
            else if (easeStyle == "easeoutquad")
                transObj.ease = Easing.QUAD_OUT;
            else if (easeStyle == "easeinoutquad")
                transObj.ease = Easing.QUAD_INOUT;
            else if (easeStyle == "easeincirc")
                transObj.ease = Easing.CIRC_IN;
            else if (easeStyle == "easeoutcirc")
                transObj.ease = Easing.CIRC_OUT;
            else if (easeStyle == "easeoutback")
                transObj.ease = Easing.BACK_OUT;
            else if (easeStyle == "easeinback")
                transObj.ease = Easing.BACK_IN;
            else {
                Debug.Log("UNKNOWN EASING STYLE: " + easeStyle);
            }
            transformList.Add(transObj);
        }
        return transformList;
    }

    // Save Project back to disk
    public void ExportActiveProject()
    {
        // Info File
        if (infoFileName == null) {
            File.WriteAllText(projectFolder+"/info_song.txt", "{\"bpm\":"+songBPM.ToString()+",\"id\":0}");
        } else {
            infoString = Regex.Replace(infoString, "\"bpm\":[0-9]+,", "\"bpm\":" + songBPM.ToString() + ",");
            File.WriteAllText(infoFileName, infoString);
        }

        // Notes Mapping File
        notes.Sort((a, b) => (a.time.CompareTo(b.time)));
        string notesString = "[";
        for (int i = 0; i < notes.Count; i += 1) {
            notesString += "{";
            notesString += "\"Id\":" + i.ToString() + ",";
            notesString += "\"Type\":\"";
            if (notes[i].type == NoteData.NoteType.CLICK)
                notesString += "click";
            else if (notes[i].type == NoteData.NoteType.HOLD)
                notesString += "hold";
            else if (notes[i].type == NoteData.NoteType.SWIPE)
                notesString += "swipe";
            else if (notes[i].type == NoteData.NoteType.SLIDE)
                notesString += "slide";
            notesString += "\",\"Track\":" + notes[i].track.ToString() + ",";
            notesString += "\"Time\":" + notes[i].time.ToString() + ",";
            if (notes[i].type == NoteData.NoteType.HOLD)
                notesString += "\"Hold\":" + notes[i].hold.ToString() + ",";
            else
                notesString += "\"Hold\":0.0,";
            notesString += "\"Dir\":" + notes[i].dir.ToString();
            if (i == notes.Count - 1)
                notesString += "}";
            else
                notesString += "},";
        }
        notesString += "]";
        File.WriteAllText(notesFileName, notesString);

        // Track Mapping File
        string tracksString = "[";
        for (int i = 0; i < tracks.Count; i += 1) {
            tracksString += "{";
            tracksString += "\"Id\":" + tracks[i].id.ToString() + ",";
            tracksString += "\"EntranceOn\":" + (tracks[i].entranceOn ? "true" : "false") + ",";
            tracksString += "\"PositionLock\":" + (tracks[i].positionLock ? "true" : "false") + ",";
            tracksString += "\"X\":" + tracks[i].x.ToString() + ",";
            tracksString += "\"Size\":" + tracks[i].size.ToString() + ",";
            tracksString += "\"Start\":" + tracks[i].start.ToString() + ",";
            tracksString += "\"End\":" + tracks[i].end.ToString() + ",";
            tracksString += "\"Color\":" + tracks[i].color.ToString() + ",";
            tracksString += "\"Move\":[";
            tracksString += TransformationsListToString(tracks[i].move, false);
            tracksString += "],\"Scale\":[";
            tracksString += TransformationsListToString(tracks[i].scale, false);
            tracksString += "],\"ColorChange\":[";
            tracksString += TransformationsListToString(tracks[i].colorChange, true);
            if (i == tracks.Count - 1)
                tracksString += "]}";
            else
                tracksString += "]},";
        }
        tracksString += "]";
        File.WriteAllText(tracksFileName, tracksString);
    }

    private string TransformationsListToString(List<TrackTransformation> transformList, bool intValue)
    {
        string retStr = "";
        for (int i = 0; i < transformList.Count; i += 1) {
            retStr += "{";
            if (intValue)
                retStr += "\"To\":" + ((int)transformList[i].to).ToString() + ",";
            else
                retStr += "\"To\":" + transformList[i].to.ToString() + ",";
            retStr += "\"Ease\":\"";
            if (transformList[i].ease == Easing.LINEAR)
                retStr += "easelinear";
            else if (transformList[i].ease == Easing.EXP_IN)
                retStr += "easeinexpo";
            else if (transformList[i].ease == Easing.EXP_OUT)
                retStr += "easeoutexpo";
            else if (transformList[i].ease == Easing.QUAD_IN)
                retStr += "easeinquad";
            else if (transformList[i].ease == Easing.QUAD_OUT)
                retStr += "easeoutquad";
            else if (transformList[i].ease == Easing.QUAD_INOUT)
                retStr += "easeinoutquad";
            else if (transformList[i].ease == Easing.CIRC_IN)
                retStr += "easeincirc";
            else if (transformList[i].ease == Easing.CIRC_OUT)
                retStr += "easeoutcirc";
            else if (transformList[i].ease == Easing.BACK_IN)
                retStr += "easeinback";
            else if (transformList[i].ease == Easing.BACK_OUT)
                retStr += "easeoutback";
            retStr += "\",\"Start\":" + transformList[i].start.ToString() + ",";
            retStr += "\"End\":" + transformList[i].end.ToString();
            if (i == transformList.Count - 1)
                retStr += "}";
            else
                retStr += "},";
        }
        return retStr;
    }

    public enum Easing {
        LINEAR,
        EXP_IN,
        EXP_OUT,
        QUAD_IN,
        QUAD_OUT,
        QUAD_INOUT,
        CIRC_IN,
        CIRC_OUT,
        BACK_IN,
        BACK_OUT
    };

    public static Color[] colors = new Color[]
    {
        Util.Color255(234, 138, 142), // 0 - Red
        Util.Color255(234, 217, 160), // 1 - Yellow
        Util.Color255(206, 206, 206), // 2 - Gray
        Util.Color255(121, 212, 230), // 3 - Light Blue
        Util.Color255(159, 234, 152), // 4 - Green
        Util.Color255(216, 173, 128), // 5 - Orange
        Util.Color255(209, 139, 193), // 6 - Violet
        Util.Color255(139, 174, 226), // 7 - Blue
        Util.Color255(118, 202, 195), // 8 - Cyan
        Util.Color255(208, 176, 252), // 9 - Purple
    };

    public class NoteData {
        public enum NoteType {
            CLICK,
            SWIPE,
            SLIDE,
            HOLD
        };
        public int id;
        public NoteData.NoteType type;
        public int track;
        public float time;
        public int dir;
        public float hold;
    }

    public class TrackData {
        public int id;
        public float x;
        public float size;
        public float start;
        public float end;
        public int color;
        public bool entranceOn;
        public bool positionLock;
        public List<TrackTransformation> move;
        public List<TrackTransformation> scale;
        public List<TrackTransformation> colorChange;
        
        public TrackData()
        {
            move = new List<TrackTransformation>();
            scale = new List<TrackTransformation>();
            colorChange = new List<TrackTransformation>();
        }
    }

    public class TrackTransformation {
        public float to;
        public float start;
        public float end;
        public Easing ease;

        public System.Func<float, float, float, float> GetEaseFunction()
        {
            // TODO: Easing styles: Easing.BACK_IN, Easing.BACK_OUT
            if (ease == Easing.LINEAR)
                return Util.LerpLinearEase;
            if (ease == Easing.EXP_IN)
                return Util.LerpExpEaseIn;
            if (ease == Easing.EXP_OUT)
                return Util.LerpExpEaseOut;
            if (ease == Easing.QUAD_IN)
                return Util.LerpQuadEaseIn;
            if (ease == Easing.QUAD_OUT)
                return Util.LerpQuadEaseOut;
            if (ease == Easing.QUAD_INOUT)
                return Util.LerpQuadEaseInOut;
            if (ease == Easing.CIRC_IN)
                return Util.LerpCircEaseIn;
            if (ease == Easing.CIRC_OUT)
                return Util.LerpCircEaseOut;
            return Util.LerpQuadEaseOut;
        }
    }
}
