using System;
using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public static class MapSaveLoadUtils
{
    private const string SCREENSHOT_FILE_NAME = "map_screen_*.png";

    private static string SaveFolder => Path.Combine(Application.dataPath, "Sources", "Minimap", "SaveData");

    private static void CreateDirectoryIfNotExists(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    public static void SaveTexture(Texture2D screen, int i)
    {
        CreateDirectoryIfNotExists(SaveFolder);
        string path = Path.Combine(SaveFolder, SCREENSHOT_FILE_NAME.Replace("*", i.ToString()));

        byte[] textureBytes = screen.EncodeToPNG();
        File.WriteAllBytes(path, textureBytes);

        Debug.LogWarning($"Save texture at path {path}");
    }

    public static Texture2D GetSaveTexture(int i)
    {
        string path = Path.Combine(SaveFolder, SCREENSHOT_FILE_NAME.Replace("*", i.ToString()));

        string localPath = path.Substring(path.IndexOf("Assets", StringComparison.Ordinal));
        Debug.Log("localPath = " + localPath);

        Texture2D savedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(localPath);

        if (savedTexture == null)
            Debug.Log("savedTexture == null");
        else
            Debug.Log("savedTexture.name = " + savedTexture.name);

        return savedTexture;
    }
}
#endif