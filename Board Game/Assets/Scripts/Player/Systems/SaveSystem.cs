using UnityEngine;
using System.IO;

/// <summary>
/// A static class for handling all saving and loading in the runtime / editor
/// </summary>
public static class SaveSystem
{
    public static readonly string LEVEL_DESIGN_SAVE_FOLDER_NAME = "/Level Design/";

    public static bool SaveLevelDesign(string fileName, LevelDesign level)
    {
        if(level == null) { return false; }
        string path = Application.dataPath + "/Resources" + LEVEL_DESIGN_SAVE_FOLDER_NAME + $"{fileName}.txt";
        // Test if save folder exists
        if (!Directory.Exists(Application.dataPath + "/Resources" + LEVEL_DESIGN_SAVE_FOLDER_NAME))
            Directory.CreateDirectory(Application.dataPath + "/Resources" + LEVEL_DESIGN_SAVE_FOLDER_NAME);

        string json = JsonUtility.ToJson(level);
        StreamWriter sw = File.CreateText(path);
        sw.Write(json);
        sw.Close();
        return true;
    }

    public static LevelDesign LoadLevelDesign(string fileName)
    {
        string path = Application.dataPath + "/Resources" + LEVEL_DESIGN_SAVE_FOLDER_NAME + $"{fileName}.txt";
        // Test if save folder exists
        if (!Directory.Exists(Application.dataPath + "/Resources" + LEVEL_DESIGN_SAVE_FOLDER_NAME))
        {
            Debug.Log($"No file at {path}");
            return null;
        }
        // Test if overlapping file exists
        if (File.Exists(path))
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            string saveString = File.ReadAllText(path);
            fs.Close();
            Debug.Log($"Loaded from {path}");
            return JsonUtility.FromJson<LevelDesign>(saveString);
        }
        else
        {
            Debug.Log($"No file at {path}");
            return null;
        }

    }
}
