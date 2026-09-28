using System.IO;
using UnityEngine;

public static class SaveSettings
{
    public const int SaveVersion = 1;

    public const string SaveFileName = "save.json";

    public const string BackupFileName = "save_backup.json";

    public static string SavePath =>
        Path.Combine(Application.persistentDataPath, SaveFileName);

 
    public static string BackupPath =>
        Path.Combine(Application.persistentDataPath, BackupFileName);
}