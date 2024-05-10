using System.Net.Mime;
using HarmonyLib;
using OpenMonitors.Monitors;
using TMPro;

namespace ShipMeltdown.Patches;

// Patch the CreditsMonitor that has a useful attribute in private
[HarmonyPatch(typeof(CreditsMonitor))]
public class CreditsMonitorPatch
{
    internal static bool meshEnable = true;

    internal static ControlledTask act;

    [HarmonyPrefix, HarmonyPatch("UpdateMonitor")]
    private static bool UpdatePatch()
    {
        if (!meshEnable)
        {
            act.Run();
            return false;
        }
        else
        {
            act.Reset();
            act.Run();
            act.Reset();
            return true;
        }
    }
}

// Patch the LifeSupportMonitor that has a useful attribute in private
[HarmonyPatch(typeof(LifeSupportMonitor))]
public class LifeSupportMonitorPatch
{
    internal static bool meshEnable = true;

    internal static ControlledTask act;

    [HarmonyPrefix, HarmonyPatch("UpdateMonitor")]
    private static bool UpdatePatch()
    {
        if (!meshEnable)
        {
            act.Run();
            return false;
        }
        else
        {
            act.Reset();
            act.Run();
            act.Reset();
            return true;
        }
    }
}
// Other monitors have that useful attribute in public