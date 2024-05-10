using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using OpenMonitors.Monitors;
using ShipMeltdown.Patches;
using TMPro;

namespace ShipMeltdown;

[BepInPlugin(modGUID, modName, modVersion)]
[BepInDependency("me.loaforc.facilitymeltdown")]
[BepInDependency("xxxstoner420bongmasterxxx.open_monitors",BepInDependency.DependencyFlags.SoftDependency)]
public class ShipMeltdown : BaseUnityPlugin
{
    internal const string modGUID = "catragryff.ShipMeltdown";
    internal const string modName = "ShipMeltdown";
    internal const string modVersion = "1.5.50";

    private readonly Harmony harmony = new Harmony(modGUID);
    internal static ShipMeltdown instance;
    internal static bool openMonitorSupport;
    
    public static ManualLogSource mls = BepInEx.Logging.Logger.CreateLogSource(modName);
    
    public static Config ShipMeltdownConfig { get; private set; }
    
    public void Awake()
    {
        mls.LogInfo("ShipMeltdown loading...");
        ShipMeltdownConfig = new Config(Config);

        openMonitorSupport = Chainloader.PluginInfos.Keys.Contains("xxxstoner420bongmasterxxx.open_monitors");
        harmony.PatchAll(typeof(StartOfRoundPatch));
        harmony.PatchAll(typeof(MeltdownHandlerPatch));
        
        if (openMonitorSupport)
        {
            harmony.PatchAll(typeof(CreditsMonitorPatch));
            harmony.PatchAll(typeof(LifeSupportMonitorPatch));
            CreditsMonitorPatch.act ??= new ControlledTask((() => { CreditsMonitor.Instance.GetComponent<TextMeshProUGUI>().enabled = CreditsMonitorPatch.meshEnable; }),
                true);
            CreditsMonitorPatch.meshEnable = true;
            LifeSupportMonitorPatch.act ??= new ControlledTask((() => { LifeSupportMonitor.Instance.GetComponent<TextMeshProUGUI>().enabled = LifeSupportMonitorPatch.meshEnable; }),
                true);
            LifeSupportMonitorPatch.meshEnable = true;
        }
        
        StartOfRoundPatch.failure = new DialogueSegment[1];
        StartOfRoundPatch.failure[0] = new DialogueSegment
        {
            waitTime = 6f,
            bodyText = "Ship engines not detected. The company warned you not to play with radiations"
        };
        
        
        FacilityMeltdown.API.MeltdownAPI.RegisterMeltdownListener(ShipPanic.OnMeltdownStarted);
        mls.LogInfo("ShipMeltdown loaded");
    }
}