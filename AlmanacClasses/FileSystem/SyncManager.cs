using AlmanacClasses.Classes;

namespace AlmanacClasses.FileSystem;

public static class SyncManager
{
    public static void InitSynchronizedFiles()
    {
        Watcher.InitWatcher();
        ExperienceManager.InitServerExperienceMap();
    }
}