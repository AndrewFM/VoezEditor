using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class ProjectData {

    public Texture2D background;
    public AudioClip songClip;
    public List<NoteData> notes;
    public List<TrackData> tracks;

    public ProjectData()
    {
        notes = new List<NoteData>();
        tracks = new List<TrackData>();
    }

    public void LoadFromActiveProject()
    {
        string projectFolder = Application.dataPath + "/../ActiveProject";
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

            // Notes Mapping File
            if (projectFiles[i].Contains("note_")) {
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
                    newNote.time = (float)((double)noteProperties["Time"]);
                    newNote.dir = (int)((long)noteProperties["Dir"]);
                    newNote.hold = (float)((double)noteProperties["Hold"]);
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
                    newTrack.x = (float)((double)basicTrackProperties["X"]);
                    newTrack.size = (float)((double)basicTrackProperties["Size"]);
                    newTrack.start = (float)((double)basicTrackProperties["Start"]);
                    newTrack.end = (float)((double)basicTrackProperties["End"]);
                    newTrack.color = (int)((long)basicTrackProperties["Color"]);
                    newTrack.positionLock = (bool)basicTrackProperties["PositionLock"];
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
            if (transProperties["To"] is System.Double)
                transObj.to = (float)((double)transProperties["To"]);
            else
                transObj.to = (long)transProperties["To"];
            transObj.start = (float)((double)transProperties["Start"]);
            transObj.end = (float)((double)transProperties["End"]);
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
            else if (easeStyle == "easeoutback")
                transObj.ease = Easing.BACK_OUT;
            else {
                Debug.Log("UNKNOWN EASING STYLE: " + easeStyle);
            }
            transformList.Add(transObj);
        }
        return transformList;
    }

    public enum Easing {
        LINEAR,
        EXP_IN,
        EXP_OUT,
        QUAD_IN,
        QUAD_OUT,
        QUAD_INOUT,
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
            return Util.LerpQuadEaseOut;
        }
    }
}
