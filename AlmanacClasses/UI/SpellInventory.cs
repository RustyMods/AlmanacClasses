using System.Collections.Generic;
using System.Linq;
using AlmanacClasses.Classes;
using AlmanacClasses.Classes.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace AlmanacClasses.UI;

public class SpellInventory : MonoBehaviour
{
    public static readonly GameObject m_element = AlmanacClassesPlugin._AssetBundle.LoadAsset<GameObject>("Inventory_element");
    public static readonly Dictionary<Talent, InventoryButton> m_elements = new();
    public static SpellInventory m_instance = null!;
    public Text m_title = null!;
    public RectTransform m_mask = null!;
    public RectTransform m_contentList = null!;
    public GridLayoutGroup m_layoutGroup = null!;
    public void Init()
    {
        m_instance = this;
        m_title = transform.Find("$text_inventory").GetComponent<Text>();
        m_mask = transform.Find("List").GetComponent<RectTransform>();
        
        var list = transform.Find("List/$part_content");
        m_contentList = list.GetComponent<RectTransform>();
        m_layoutGroup = list.GetComponent<GridLayoutGroup>();
        m_element.AddComponent<InventoryButton>();
        m_element.AddComponent<ButtonSfx>().m_sfxPrefab = LoadUI.m_vanillaButtonSFX.m_sfxPrefab;
        m_title.text = "$text_inventory";
    }
    
    public void Resize()
    {
        var size = m_layoutGroup.cellSize + m_layoutGroup.spacing;
        var contentHeight = m_mask.sizeDelta.y;
        var height = Mathf.CeilToInt(GetChildCount() / 4f) * size.y;
        if (contentHeight > height)
        {
            m_contentList.offsetMin = Vector2.zero;
        }
        else
        {
            var difference = height - contentHeight;
            m_contentList.offsetMin = new Vector2(0f, -difference);
        }
    }

    public void Remove(Talent talent)
    {
        if (!IsInInventory(talent)) return;
        if (!m_elements.TryGetValue(talent, out InventoryButton element)) return;
        Destroy(element.gameObject);
        Resize();
    }

    public void Add(Talent talent, bool active, bool resize = false)
    {
        var element = Instantiate(m_element, m_contentList).GetComponent<InventoryButton>();
        element.Init();
        element.m_talent = talent;
        element.SetName(talent.GetName());
        element.SetBorder(active);
        element.SetIcon(talent.m_sprite);
        element.OnClick(() =>
        {
            if (SpellBook.IsAbilityInBook(talent))
            {
                if (!AbilityManager.IsReady(talent))
                    return;
                
                SpellBook.Remove(talent);
                element.SetBorder(false);
            }
            else
            {
                if (SpellBook.ActiveSlotCount() > 7)
                {
                    Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_spell_book_full");
                }
                else
                {
                    SpellBook.Add(talent);
                    element.SetBorder(true);
                }
            }
        });
        m_elements[talent] = element;
        if (resize) Resize();
    }

    public static bool IsInInventory(Talent talent) => m_elements.Keys.Contains(talent);

    private int GetChildCount() => m_contentList.childCount;

    public void Clear()
    {
        foreach (Transform child in m_contentList)
        {
            Destroy(child.gameObject);
        }
    }
}