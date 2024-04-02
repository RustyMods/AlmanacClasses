using AlmanacClasses.Classes;
using HarmonyLib;

namespace AlmanacClasses.Managers;

public static class ReceiveDataManager
{
    [HarmonyPatch(typeof(Player), nameof(Player.Message))]
    private static class Player_Message_Patch
    {
        private static void Postfix(MessageHud.MessageType type, string msg)
        {
            if (type is not MessageHud.MessageType.TopLeft) return;
            if (msg.StartsWith("Added") && msg.EndsWith("experience"))
            {
                string[] input = msg.Split(' ');
                if (int.TryParse(input[1], out int experience))
                {
                    AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Received experience from external source");
                    PlayerManager.m_tempPlayerData.m_experience += experience;
                    AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug($"Added {experience} class experience");
                }
            }
        }
    }
    
    
}