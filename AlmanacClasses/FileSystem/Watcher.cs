using System.IO;
using AlmanacClasses.Classes;
using BepInEx;

namespace AlmanacClasses.FileSystem;

public static class Watcher
{
    public static void InitWatcher()
    {
        FileSystemWatcher ExperienceMapWatcher = new FileSystemWatcher(FilePaths.ExperienceFolderPath)
        {
            Filter = "*.yml",
            EnableRaisingEvents = true,
            IncludeSubdirectories = true,
            SynchronizingObject = ThreadingHelper.SynchronizingObject,
            NotifyFilter = NotifyFilters.LastWrite
        };
        ExperienceMapWatcher.Changed += OnExperienceMapChange;
        ExperienceMapWatcher.Created += OnExperienceMapChange;
        ExperienceMapWatcher.Deleted += OnExperienceMapChange;
    }

    private static void OnExperienceMapChange(object sender, FileSystemEventArgs e)
    {
        if (!ZNet.instance) return;
        if (!ZNet.instance.IsServer()) return;
        string fileName = Path.GetFileName(e.Name);
        switch (e.ChangeType)
        {
            case WatcherChangeTypes.Changed:
                AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Server: " + fileName + " changed, reloading experience map");
                break;
            case WatcherChangeTypes.Created:
                AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Server: " + fileName + " created, reloading experience map");
                break;
            case WatcherChangeTypes.Deleted:
                AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Server: " + fileName + " deleted, reloading experience map");
                break;
        }
        ExperienceManager.ReadExperienceFiles(true);
    }
}