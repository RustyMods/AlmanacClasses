using HarmonyLib;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace AlmanacClasses.Managers;

public static class DisplayText
{
    public static void ShowText(Color color, Vector3 pos, string text)
    {
        Vector3 colorFormat = new Vector3(color.r, color.g, color.b);
        SendTextPackage(colorFormat, pos, text);
    }

    private static void SendTextPackage(Vector3 color, Vector3 pos, string text)
    {
        ZPackage pkg = new ZPackage();
        pkg.Write(color);
        pkg.Write(pos);
        pkg.Write(text);
        ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, nameof(RPC_ReceiveTextPackage), pkg);
    }

    public static void RPC_ReceiveTextPackage(long sender, ZPackage pkg)
    {
        try
        {
            Vector3 colorVector = pkg.ReadVector3();
            Color color = new Color(colorVector.x, colorVector.y, colorVector.z);
            Vector3 pos = pkg.ReadVector3();
            string? text = pkg.ReadString();
            
            float distance = Vector3.Distance(Utils.GetMainCamera().transform.position, pos);

            DamageText.WorldTextInstance instance = new DamageText.WorldTextInstance
            {
                m_worldPos = pos + Random.insideUnitSphere * 0.5f,
                m_gui = Object.Instantiate(DamageText.instance.m_worldTextBase, DamageText.instance.transform)
            };
            instance.m_textField = instance.m_gui.GetComponent<TMP_Text>();
            DamageText.instance.m_worldTexts.Add(instance);
            instance.m_textField.color = color;
            instance.m_textField.fontSize = distance <= DamageText.instance.m_smallFontDistance
                ? DamageText.instance.m_smallFontSize
                : DamageText.instance.m_largeFontSize;
            instance.m_textField.text = Localization.instance.Localize(text);
            instance.m_timer = 0.0f;
        }
        catch
        {
            //
        }
    }

    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    private static class ZNetScene_Awake_Register_RPC
    {
        private static void Postfix()
        {
            ZRoutedRpc.instance.Register<ZPackage>(nameof(RPC_ReceiveTextPackage), RPC_ReceiveTextPackage);
        }
    }
}