using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ProjectMigrator {
    public ProjectMigrator()
    {
        MigrateToV104();
        MigrateToV105();
    }

    // v1.04 Project -> v1.05 Project
    public void MigrateToV105()
    {
        string rootFolder = Application.dataPath + "/../ActiveProject";
        string[] projectFolders = Directory.GetDirectories(rootFolder);
        for(int i=0; i<projectFolders.Length; i++) {
            string[] projectFiles = Directory.GetFiles(projectFolders[i], "*.*", SearchOption.TopDirectoryOnly);
            for(int j=0; j<projectFiles.Length; j++) {
                if (projectFiles[j].EndsWith(".txt") && !projectFiles[j].Contains("songconfig")) {
                    File.Move(projectFiles[j], projectFiles[j].Substring(0, projectFiles[j].Length - 4) + ".json");
                }
            }
        }
    }

    // v1.00-v1.03 Project -> v1.04 Project
    public void MigrateToV104()
    {
        // Search root of ActiveProjects for project files not in a subfolder.
        string rootFolder = Application.dataPath + "/../ActiveProject";
        string[] projectFiles = Directory.GetFiles(rootFolder, "*.*", SearchOption.TopDirectoryOnly);
        string tempDirectory = rootFolder + "/Untitled";
        if (Directory.Exists(tempDirectory)) {
            int i = 2;
            while(Directory.Exists(tempDirectory)) {
                tempDirectory = rootFolder + "/Untitled_" + i.ToString();
                i++;
            }
        }

        for (int i = 0; i < projectFiles.Length; i += 1) {
            // Info File
            if (projectFiles[i].Contains("info_")) {
                if (!Directory.Exists(tempDirectory))
                    Directory.CreateDirectory(tempDirectory);

                // Read BPM from old Info File
                string infoString = File.ReadAllText(projectFiles[i]);
                int songBPM = 120;
                if (infoString.Contains("\"bpm\"")) {
                    string bpmPart = infoString.Substring(infoString.IndexOf("\"bpm\""));
                    int bpmStartInd = bpmPart.IndexOf(":");
                    int bpmEndInd = bpmPart.IndexOf(",");
                    songBPM = int.Parse(bpmPart.Substring(bpmStartInd + 1, bpmEndInd - bpmStartInd - 1));
                    songBPM = Mathf.Clamp(songBPM, 10, 250);
                }

                // Write new Info File format into new folder
                string newInfoString = "{\"info\":{";
                newInfoString += "\"version\":\"1.04\",";
                newInfoString += "\"author\":\"\",";
                newInfoString += "\"bpm\":" + songBPM.ToString() + ",";
                newInfoString += "\"name\":\"Untitled\"";
                newInfoString += "},\"level\":{";
                newInfoString += "\"easy\":1,";
                newInfoString += "\"hard\":1,";
                newInfoString += "\"extra\":1}";
                newInfoString += "}";
                File.WriteAllText(tempDirectory + "/info_song.txt", newInfoString);

                // Delete old info file
                File.Delete(projectFiles[i]);
            }

            // Notes File
            if (projectFiles[i].Contains("note_")) {
                if (!Directory.Exists(tempDirectory))
                    Directory.CreateDirectory(tempDirectory);
                File.Move(projectFiles[i], tempDirectory + "/note_easy.txt");
            }
            // Tracks File
            if (projectFiles[i].Contains("track_")) {
                if (!Directory.Exists(tempDirectory))
                    Directory.CreateDirectory(tempDirectory);
                File.Move(projectFiles[i], tempDirectory + "/track_easy.txt");
            }
            // Background Image File
            if (projectFiles[i].Contains("image_") && projectFiles[i].Contains(".png")) {
                if (!Directory.Exists(tempDirectory))
                    Directory.CreateDirectory(tempDirectory);
                File.Move(projectFiles[i], tempDirectory + "/image_regular.png");
            }
            // Song File
            if (projectFiles[i].Contains("song_") && projectFiles[i].Contains(".wav")) {
                if (!Directory.Exists(tempDirectory))
                    Directory.CreateDirectory(tempDirectory);
                File.Move(projectFiles[i], tempDirectory + "/song_full.wav");
            }
        }
    }
}
