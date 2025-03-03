using AlmanacClasses.Classes;
using UnityEngine;

namespace AlmanacClasses.UI;

/// <summary>
/// Component attached to buildable altar prefab
/// Allows user to interact with the prefab and access the UI
/// </summary>
public class TalentBook : MonoBehaviour, Interactable, Hoverable
{
    public string m_name = "$title_altar";
    
    public bool Interact(Humanoid user, bool hold, bool alt)
    {
        if (hold || alt) return false;
        Show();
        return true;
    }
    
    public void Show()
    {
        Player.m_localPlayer.m_zanim.SetInt("crafting", 1);
        SkillTree.m_instance.Show();
    }
    public bool UseItem(Humanoid user, ItemDrop.ItemData item) => false;
    public string GetHoverText() => Localization.instance.Localize(m_name) + "\n" + Localization.instance.Localize("[<color=yellow><b>$KEY_Use</b></color>] $info_open_book");
    public string GetHoverName() => Localization.instance.Localize(m_name);
}