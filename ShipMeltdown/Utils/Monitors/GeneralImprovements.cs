using System.Net.Mime;
using GeneralImprovements.Utilities;
using GeneralImprovements;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace ShipMeltdown.Utils.Monitors;

public class GeneralImprovements : MonitorCompatibility
{
    internal static Queue<TextMeshProUGUI> monitors_text;
    internal static Queue<ManualCameraRenderer> monitors_cameras;
    internal static Queue<MeshRenderer> monitors_mesh;

    internal GeneralImprovements()
    {
        ShipMeltdown.instance.harmony.PatchAll(typeof(StartOfRoundGeneralPatch));
        ShipMeltdown.instance.harmony.PatchAll(typeof(MonitorsHelperPatch));
    }
    
    private ControlledTask maintainScreenOff = new ControlledTask(() => { StartOfRound.Instance.mapScreen.SwitchScreenOn(false); }, false);
    public ControlledTask MaintainScreenOff()
    {
        return maintainScreenOff;
    }

    public void MostSystemsDead()
    {
        foreach (TextMeshProUGUI t in monitors_text)
        {
            t.enabled = false;
        }

        foreach (ManualCameraRenderer mc in monitors_cameras)
        {
            mc.enabled = false;
        }

        foreach (MeshRenderer m in monitors_mesh)
        {
            m.enabled = false;
        }
        
        StartOfRound.Instance.mapScreen.SwitchScreenOn(false);
        MonitorsHelperPatch.allow = false;
    }

    public void ReviveSystems()
    {
        foreach (TextMeshProUGUI t in monitors_text)
        {
            t.enabled = true;
        }

        foreach (ManualCameraRenderer mc in monitors_cameras)
        {
            mc.enabled = true;
        }

        foreach (MeshRenderer m in monitors_mesh)
        {
            m.enabled = true;
        }

        MonitorsHelperPatch.allow = true;
        StartOfRound.Instance.mapScreen.SwitchScreenOn(true);
    }
}

// Register every screen, hoping not to catch something else in the process
[HarmonyPatch(typeof(StartOfRound))]
internal static class StartOfRoundGeneralPatch
{
    [HarmonyPatch(typeof(StartOfRound), "Start")]
    [HarmonyPostfix]
    [HarmonyAfter("ShaosilGaming.GeneralImprovements")]
    private static void StartPatch()
    {
        GeneralImprovements.monitors_cameras ??= new Queue<ManualCameraRenderer>();
        GeneralImprovements.monitors_cameras.Clear();
        
        GeneralImprovements.monitors_mesh ??= new Queue<MeshRenderer>();
        GeneralImprovements.monitors_mesh.Clear();

        GeneralImprovements.monitors_text ??= new Queue<TextMeshProUGUI>();
        GeneralImprovements.monitors_text.Clear();

        foreach (ManualCameraRenderer mc in StartOfRound.Instance.GetComponents<ManualCameraRenderer>()) { if (mc.enabled) GeneralImprovements.monitors_cameras.Enqueue(mc); }
        foreach (ManualCameraRenderer mc in StartOfRound.Instance.GetComponentsInChildren<ManualCameraRenderer>()) { if (mc.enabled) GeneralImprovements.monitors_cameras.Enqueue(mc);}
        
        foreach (MeshRenderer m in StartOfRound.Instance.GetComponents<MeshRenderer>()) { if (m.enabled) GeneralImprovements.monitors_mesh.Enqueue(m); }
        foreach (MeshRenderer m in StartOfRound.Instance.GetComponentsInChildren<MeshRenderer>()) { if (m.enabled) GeneralImprovements.monitors_mesh.Enqueue(m);}
        
        foreach (TextMeshProUGUI t in StartOfRound.Instance.GetComponents<TextMeshProUGUI>()) { if (t.enabled) GeneralImprovements.monitors_text.Enqueue(t); }
        foreach (TextMeshProUGUI t in StartOfRound.Instance.GetComponentsInChildren<TextMeshProUGUI>()) { if (t.enabled) GeneralImprovements.monitors_text.Enqueue(t);}
        
        ShipMeltdown.mls.LogInfo($"Found {GeneralImprovements.monitors_cameras.Count} ManualCameraRenderers, {GeneralImprovements.monitors_mesh.Count} MeshRenderers, {GeneralImprovements.monitors_text.Count} TextMeshProUGUIs while initializing queues for GeneralImprovements support");
    }
}

[HarmonyPatch(typeof(MonitorsHelper))]
internal static class MonitorsHelperPatch
{
    internal static bool allow = true;
    
    // For some obscure reason, this function broke GeneralImprovements when called from ShipMeltdown. That is why its
    // effects are reproduced manually
    [HarmonyPatch("ToggleExtraMonitorsPower")]
    [HarmonyPrefix]
    private static bool ToggleExtraMonitorsPowerPatch()
    {
        return allow;
    }
}