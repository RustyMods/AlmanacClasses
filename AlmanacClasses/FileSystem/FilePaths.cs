using System.IO;
using BepInEx;

namespace AlmanacClasses.FileSystem;

public static class FilePaths
{
    private static readonly string FolderPath = Paths.ConfigPath + Path.DirectorySeparatorChar + "AlmanacClasses";
    public static readonly string ExperienceFolderPath = FolderPath + Path.DirectorySeparatorChar + "Experience";
    public static readonly string ExperienceFilePath = ExperienceFolderPath + Path.DirectorySeparatorChar + "AlmanacExperienceMap.yml";

    public static void CreateFolders()
    {
        if (!Directory.Exists(FolderPath)) Directory.CreateDirectory(FolderPath);
        if (!Directory.Exists(ExperienceFolderPath)) Directory.CreateDirectory(ExperienceFolderPath);
    }
}