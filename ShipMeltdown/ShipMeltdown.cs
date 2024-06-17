using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using OpenMonitors.Monitors;
using ShipMeltdown.Patches;
using ShipMeltdown.Utils;
using ShipMeltdown.Utils.Monitors;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShipMeltdown;

[BepInPlugin(modGUID, modName, modVersion)]
[BepInDependency("me.loaforc.facilitymeltdown")]
[BepInDependency("xxxstoner420bongmasterxxx.open_monitors",BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("ShaosilGaming.GeneralImprovements", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("PizzaProbability", BepInDependency.DependencyFlags.SoftDependency)]
public class ShipMeltdown : BaseUnityPlugin
{
    internal const string modGUID = "catragryff.ShipMeltdown";
    internal const string modName = "ShipMeltdown";
    internal const string modVersion = "1.7.50";

    internal readonly Harmony harmony = new Harmony(modGUID);
    internal static ShipMeltdown instance;
    private static bool openMonitorSupport;
    private static bool generalImprovementsSupport;
    
    internal static ManualLogSource mls = BepInEx.Logging.Logger.CreateLogSource(modName);
    
    public static Config ShipMeltdownConfig { get; private set; }
    
    public void Awake()
    {
        if (instance == null)
            instance = this;
        
        mls.LogInfo("ShipMeltdown loading...");
        ShipMeltdownConfig = new Config(Config);

        openMonitorSupport = Chainloader.PluginInfos.Keys.Contains("xxxstoner420bongmasterxxx.open_monitors");
        generalImprovementsSupport = Chainloader.PluginInfos.Keys.Contains("ShaosilGaming.GeneralImprovements");

        if (Chainloader.PluginInfos.Keys.Contains("PizzaProbability"))
        {
            mls.LogError("PizzaProbability detected ! This mod is incompatible with ShipMeltdown. Trying to remove PizzaProbability patches to avoid breaking the game...");
            Harmony.UnpatchID("PizzaProbability");
        }
        
        harmony.PatchAll(typeof(StartOfRoundPatch1));
        if (ShipMeltdownConfig.getToggleEmergencyLights != Key.None)
            harmony.PatchAll(typeof(StartOfRoundPatch2));
        harmony.PatchAll(typeof(MeltdownHandlerPatch));
        
        if (generalImprovementsSupport)
        {
            mls.LogInfo("Adding GeneralImprovements support");
            MonitorCompatibilityHandler.AddMonitorCompatibilityHandler(new Utils.Monitors.GeneralImprovements());
        }
        
        if (openMonitorSupport)
        {
            mls.LogInfo("Adding OpenMonitor support");
            MonitorCompatibilityHandler.AddMonitorCompatibilityHandler(new OpenMonitor());
        }
        
        if (!(openMonitorSupport || generalImprovementsSupport))
        {
            mls.LogInfo("No monitor support were added. Adding the default one");
            MonitorCompatibilityHandler.AddMonitorCompatibilityHandler(new DefaultMonitor(), false);
        }
        
        StartOfRoundPatch1.failure = new DialogueSegment[1];
        StartOfRoundPatch1.failure[0] = new DialogueSegment
        {
            waitTime = 6f,
            bodyText = "Ship engines not detected. The company warned you not to play with radiations"
        };
        
        
        FacilityMeltdown.API.MeltdownAPI.RegisterMeltdownListener(ShipPanic.OnMeltdownStarted);
        mls.LogInfo("ShipMeltdown loaded");
    }
}