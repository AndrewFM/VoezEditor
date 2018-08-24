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

    public List<TrackTransformation> transformClipboard;
    public TrackTransformation.TransformType clipboardContentsType;

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

    public void DeleteTrack(int ID)
    {
        for (int i = 0; i < tracks.Count; i += 1) {
            if (tracks[i].id == ID) {
                List<int> noteIDsToDelete = new List<int>();
                for(int j = 0; j<notes.Count; j+=1) {
                    if (notes[j].track == ID)
                        noteIDsToDelete.Add(notes[j].id);
                }
                for (int j = 0; j < noteIDsToDelete.Count; j += 1)
                    DeleteNote(noteIDsToDelete[j]);
                tracks.RemoveAt(i);
                break;
            }
        }
    }

    public void AddTrack(TrackData data)
    {
        tracks.Add(data);
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
            else if (easeStyle == "easeinoutexpo")
                transObj.ease = Easing.EXP_INOUT;
            else if (easeStyle == "easeoutinexpo")
                transObj.ease = Easing.EXP_OUTIN;
            else if (easeStyle == "easeinquad")
                transObj.ease = Easing.QUAD_IN;
            else if (easeStyle == "easeoutquad")
                transObj.ease = Easing.QUAD_OUT;
            else if (easeStyle == "easeinoutquad")
                transObj.ease = Easing.QUAD_INOUT;
            else if (easeStyle == "easeoutinquad")
                transObj.ease = Easing.QUAD_OUTIN;
            else if (easeStyle == "easeincirc")
                transObj.ease = Easing.CIRC_IN;
            else if (easeStyle == "easeoutcirc")
                transObj.ease = Easing.CIRC_OUT;
            else if (easeStyle == "easeinoutcirc")
                transObj.ease = Easing.CIRC_INOUT;
            else if (easeStyle == "easeoutincirc")
                transObj.ease = Easing.CIRC_OUTIN;
            else if (easeStyle == "easeoutback")
                transObj.ease = Easing.BACK_OUT;
            else if (easeStyle == "easeinback")
                transObj.ease = Easing.BACK_IN;
            else if (easeStyle == "easeinoutback")
                transObj.ease = Easing.BACK_INOUT;
            else if (easeStyle == "easeoutinback")
                transObj.ease = Easing.BACK_OUTIN;
            else if (easeStyle == "easeintelastic") // not a typo
                transObj.ease = Easing.ELASTIC_IN;
            else if (easeStyle == "easeoutelastic")
                transObj.ease = Easing.ELASTIC_OUT;
            else if (easeStyle == "easeinoutelastic")
                transObj.ease = Easing.ELASTIC_INOUT;
            else if (easeStyle == "easeoutinelastic")
                transObj.ease = Easing.ELASTIC_OUTIN;
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

        // Track Mapping File
        // WARNING: Order matters here. Track mapping needs to be exported first before notes mapping.
        tracks.Sort((a, b) => (a.start.CompareTo(b.start))); // Sort tracks ascending by start time.
        // Track IDs will be changed to match new sorted order. Relink all notes to new track IDs.
        for (int i=0; i<notes.Count; i+=1) {
            for(int j=0; j<tracks.Count; j+=1) {
                if (notes[i].track == tracks[j].id) {
                    notes[i].track = j;
                    break;
                }
            }
        }
        // Apply new track IDs
        for (int i = 0; i < tracks.Count; i += 1)
            tracks[i].id = i;
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

        // Notes Mapping File
        notes.Sort((a, b) => (a.time.CompareTo(b.time))); // Sort notes ascending by time spawned.
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
        VoezEditor.Editor.RefreshAllNotes();
        VoezEditor.Editor.RefreshAllTracks();
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
            else if (transformList[i].ease == Easing.EXP_INOUT)
                retStr += "easeinoutexpo";
            else if (transformList[i].ease == Easing.EXP_OUTIN)
                retStr += "easeoutinexpo";
            else if (transformList[i].ease == Easing.QUAD_IN)
                retStr += "easeinquad";
            else if (transformList[i].ease == Easing.QUAD_OUT)
                retStr += "easeoutquad";
            else if (transformList[i].ease == Easing.QUAD_INOUT)
                retStr += "easeinoutquad";
            else if (transformList[i].ease == Easing.QUAD_OUTIN)
                retStr += "easeoutinquad";
            else if (transformList[i].ease == Easing.CIRC_IN)
                retStr += "easeincirc";
            else if (transformList[i].ease == Easing.CIRC_OUT)
                retStr += "easeoutcirc";
            else if (transformList[i].ease == Easing.CIRC_INOUT)
                retStr += "easeinoutcirc";
            else if (transformList[i].ease == Easing.CIRC_OUTIN)
                retStr += "easeoutincirc";
            else if (transformList[i].ease == Easing.BACK_IN)
                retStr += "easeinback";
            else if (transformList[i].ease == Easing.BACK_OUT)
                retStr += "easeoutback";
            else if (transformList[i].ease == Easing.BACK_INOUT)
                retStr += "easeinoutback";
            else if (transformList[i].ease == Easing.BACK_OUTIN)
                retStr += "easeoutinback";
            else if (transformList[i].ease == Easing.ELASTIC_IN)
                retStr += "easeintelastic"; // not a typo
            else if (transformList[i].ease == Easing.ELASTIC_OUT)
                retStr += "easeoutelastic";
            else if (transformList[i].ease == Easing.ELASTIC_INOUT)
                retStr += "easeinoutelastic";
            else if (transformList[i].ease == Easing.ELASTIC_OUTIN)
                retStr += "easeoutinelastic";
            retStr += "\",\"Start\":" + transformList[i].start.ToString() + ",";
            retStr += "\"End\":" + transformList[i].end.ToString();
            if (i == transformList.Count - 1)
                retStr += "}";
            else
                retStr += "},";
        }
        return retStr;
    }

    // Creates a deep copy of a list of track transformations (ie: not a reference to the original)
    public static List<TrackTransformation> DeepCopyTransformationList(List<TrackTransformation> list)
    {
        List<TrackTransformation> newList = new List<TrackTransformation>();
        for(int i=0; i<list.Count; i+=1) {
            newList.Add(list[i].Copy());
        }
        return newList;
    }

    // Replaces the contents of the dest transformation list with the source transformation list, without breaking its original reference
    public static void ReplaceTransformationList(List<TrackTransformation> source, List<TrackTransformation> dest)
    {
        dest.Clear();
        for(int i=0; i<source.Count; i+=1) {
            TrackTransformation newTrans = new TrackTransformation();
            newTrans.Paste(source[i]);
            dest.Add(newTrans);
        }
    }

    public enum Easing {
        LINEAR,
        EXP_IN,
        EXP_OUT,
        EXP_INOUT,
        EXP_OUTIN,
        QUAD_IN,
        QUAD_OUT,
        QUAD_INOUT,
        QUAD_OUTIN,
        CIRC_IN,
        CIRC_OUT,
        CIRC_INOUT,
        CIRC_OUTIN,
        BACK_IN,
        BACK_OUT,
        BACK_INOUT,
        BACK_OUTIN,
        ELASTIC_IN,
        ELASTIC_OUT,
        ELASTIC_INOUT,
        ELASTIC_OUTIN
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

    public static string[] colorNames = new string[]
    {
        "Red", "Yellow", "Gray", "Light Blue", "Green", "Orange", "Violet", "Blue", "Cyan", "Purple"
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

        public enum TransformType {
            MOVE,
            SCALE,
            COLOR
        };

        public TrackTransformation Copy()
        {
            TrackTransformation copiedTrans = new TrackTransformation();
            copiedTrans.to = to;
            copiedTrans.start = start;
            copiedTrans.end = end;
            copiedTrans.ease = ease;
            return copiedTrans;
        }

        public void Paste(TrackTransformation newData)
        {
            to = newData.to;
            start = newData.start;
            end = newData.end;
            ease = newData.ease;
        }

        public System.Func<float, float, float, float> GetEaseFunction()
        {
            if (ease == Easing.LINEAR)
                return Util.LerpLinearEase;
            if (ease == Easing.EXP_IN)
                return Util.LerpExpEaseIn;
            if (ease == Easing.EXP_OUT)
                return Util.LerpExpEaseOut;
            if (ease == Easing.EXP_INOUT)
                return Util.LerpExpEaseInOut;
            if (ease == Easing.EXP_OUTIN)
                return Util.LerpExpEaseOutIn;
            if (ease == Easing.QUAD_IN)
                return Util.LerpQuadEaseIn;
            if (ease == Easing.QUAD_OUT)
                return Util.LerpQuadEaseOut;
            if (ease == Easing.QUAD_INOUT)
                return Util.LerpQuadEaseInOut;
            if (ease == Easing.QUAD_OUTIN)
                return Util.LerpQuadEaseOutIn;
            if (ease == Easing.CIRC_IN)
                return Util.LerpCircEaseIn;
            if (ease == Easing.CIRC_OUT)
                return Util.LerpCircEaseOut;
            if (ease == Easing.CIRC_INOUT)
                return Util.LerpCircEaseInOut;
            if (ease == Easing.CIRC_OUTIN)
                return Util.LerpCircEaseOutIn;
            if (ease == Easing.BACK_IN)
                return Util.LerpBackEaseIn;
            if (ease == Easing.BACK_OUT)
                return Util.LerpBackEaseOut;
            if (ease == Easing.BACK_INOUT)
                return Util.LerpBackEaseInOut;
            if (ease == Easing.BACK_OUTIN)
                return Util.LerpBackEaseOutIn;
            if (ease == Easing.ELASTIC_IN)
                return Util.LerpElasticEaseIn;
            if (ease == Easing.ELASTIC_OUT)
                return Util.LerpElasticEaseOut;
            if (ease == Easing.ELASTIC_INOUT)
                return Util.LerpElasticEaseInOut;
            if (ease == Easing.ELASTIC_OUTIN)
                return Util.LerpElasticEaseOutIn;
            return Util.LerpQuadEaseOut;
        }
    }
}
