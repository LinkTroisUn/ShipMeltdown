using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using BepInEx.Logging;
using FacilityMeltdown.API;
// using FacilityMeltdown.MeltdownSequence.Behaviours;
using HarmonyLib;
using ShipMeltdown.Patches;
using UnityEngine;
using UnityEngine.UI;

namespace ShipMeltdown;

// Main class
internal static class ShipPanic
{
    internal static float delta;                      // delta for emergency lights
    internal static bool repeat = true;               // Should the shipdoor be closed or opened ?
    internal static float delta2;                     // delta for ship doors
    internal static HangarShipDoor h;
    internal static Queue<(Light, Color)> colors;      // Initial color of lights,
    internal static Queue<(Light, bool)> toggleGroup; // Initial activation state of lights
    internal static bool canTakeOff = true;
    internal static float meltdownTimer;              // Time remaining before meltdown
    internal static ControlledTask KillSystems;
    internal static ControlledTask MaintainScreenOff;
    internal static ControlledTask BreakLever;
    internal static ControlledTask takeOff;
    internal static bool OnlyLights;
    
    public static bool CanTakeOff => canTakeOff;

    
    // Initializes the different controlled task and main global variables
    // Meltdown listener
    public static void OnMeltdownStarted()
    {
       KillSystems  ??= new ControlledTask(MostSystemsDead, true);
       BreakLever ??= new ControlledTask(ShipCantTakeOff, true);
       MaintainScreenOff ??=
           new ControlledTask(() => StartOfRound.Instance.mapScreen.SwitchScreenOn(false), false);
       takeOff ??= new ControlledTask(() => { ShipPanic.canTakeOff = true; 
           StartOfRound.Instance.shipHasLanded = false;
           StartOfRound.Instance.shipIsLeaving = true;
           StartOfRound.Instance.shipAnimator.ResetTrigger("ShipLeave");
           StartOfRound.Instance.shipAnimator.SetTrigger("ShipLeave");}, true);
       meltdownTimer = 120f; // Changing the meltdown duration in FacilityMeltdown config will sure break things
       delta = 0f;
       delta2 = 0f;
       
       LightAlarm();
    }

    // Prevent the player to manually take off when there is very very little time remaining before the explosion
    private static void ShipCantTakeOff()
    {
        canTakeOff = false;
        StartMatchLever startMatchLever = UnityEngine.Object.FindObjectOfType<StartMatchLever>();
        startMatchLever.triggerScript.interactable = false;
    }

    // Function that gets and sets up the required parameters for the effects of the mod to be run.
    //  O = true emergency
    //  1 = activate emergency lights
    // -1 = deactivates emergency light
    //  * = sus
    internal static void LightAlarm(int emergencyType = 0)
    {
        StartOfRound shipManager = StartOfRound.Instance;
        colors ??= new Queue<(Light, Color)>();
        toggleGroup ??= new Queue<(Light, bool)>();
        
        while (colors.Count > 0)
        {
            (Light l,Color c ) = colors.Dequeue();
            l.color = c;
        }

        foreach ((Light l, bool b) in toggleGroup)
            l.enabled = b;
        toggleGroup.Clear();

        if (emergencyType == -1)
        {
            OnlyLights = false;
            return;
        }
        
        Light[] interior = shipManager.shipRoomLights.GetComponentsInChildren<Light>();
        foreach (Light l in shipManager.shipAnimator.GetComponentsInChildren<Light>())
        {
            colors.Enqueue((l, l.color));
            l.color = Color.red;
            if (!interior.Contains(l))
            {
                toggleGroup.Enqueue((l, l.enabled));
            }
        }

        if (emergencyType == 1)
            OnlyLights = true;

        if (emergencyType != 0) return;
        
        h = UnityEngine.Object.FindObjectOfType<HangarShipDoor>();
        h.SetDoorButtonsEnabled(false);
    }

    // Manual emergency light toggle
    internal static void ToggleLightsOnly()
    {
        foreach ((Light l, bool b) in toggleGroup)
        {
            l.enabled = b && !l.enabled;
        }
        StartOfRound.Instance.shipRoomLights.ToggleShipLights();
    }

    // Only purpose of that function is to prevent the mod from breaking the game if for some reason, a conflict with
    // another mod makes the Action action fail to run
    private static void TryWithoutNullpo(Action action)
    {
        try {action.Invoke();} catch (NullReferenceException e){}
    }

     
    // Force shutdown as many screens as possible
    private static void MostSystemsDead()
    {
        if (ShipMeltdown.openMonitorSupport)
            MostSystemsDeadWithMonitors();
        else
            MostSystemsDeadWithoutMonitors();
    }

    private static void MostSystemsDeadWithoutMonitors()
    {
         h.hydraulicsDisplay.SetActive(false); 
            TryWithoutNullpo((() => {StartOfRound.Instance.deadlineMonitorText.enabled = false;}));
            TryWithoutNullpo((() => {StartOfRound.Instance.deadlineMonitorBGImage.enabled = false;}));
            TryWithoutNullpo((() => {StartOfRound.Instance.profitQuotaMonitorText.enabled = false;}));
            TryWithoutNullpo((() => {StartOfRound.Instance.profitQuotaMonitorBGImage.enabled = false;}));
    }
   
    private static void MostSystemsDeadWithMonitors()
    {
        h.hydraulicsDisplay.SetActive(false);
        
        if (!OpenMonitors.Monitors.Config.HideCredits.Value)
        {
            CreditsMonitorPatch.meshEnable = false;
            OpenMonitors.Monitors.CreditsMonitor.Instance.UpdateMonitor();
        }

        if (!OpenMonitors.Monitors.Config.HideLifeSupport.Value)
        {
            LifeSupportMonitorPatch.meshEnable = false;
            OpenMonitors.Monitors.LifeSupportMonitor.Instance.UpdateMonitor();
        }
            
        if (OpenMonitors.Monitors.Config.KeepBlueBackground2.Value)
            StartOfRound.Instance.deadlineMonitorBGImage.enabled = false;
        if (OpenMonitors.Monitors.Config.KeepBlueBackground1.Value)
            StartOfRound.Instance.profitQuotaMonitorBGImage.enabled = false;
            
        StartOfRound.Instance.deadlineMonitorText.enabled = false;
        StartOfRound.Instance.profitQuotaMonitorText.enabled = false;

        if (!OpenMonitors.Monitors.Config.HideDay.Value)
            OpenMonitors.Monitors.DayMonitor.Instance.textMesh.enabled = false;

        if (!OpenMonitors.Monitors.Config.HideLoot.Value)
            OpenMonitors.Monitors.LootMonitor.Instance.textMesh.enabled = false;

        if (!OpenMonitors.Monitors.Config.HideTime.Value)
            OpenMonitors.Monitors.TimeMonitor.Instance.textMesh.enabled = false;

        if (!OpenMonitors.Monitors.Config.HideLifeSupport.Value)
            OpenMonitors.Monitors.LifeSupportMonitor.Instance.enabled = false;

        if (!OpenMonitors.Monitors.Config.HidePlayersLifeSupport.Value)
            OpenMonitors.Monitors.PlayersLifeSupportMonitor.Instance.textMesh.enabled = false;
    }

    // When reaching Low Moon Orbit, re-activate the systems that might have been disabled (and stop trying to force them to die)
    internal static void ReviveSystems()
    {
        h.hydraulicsDisplay.SetActive(true);
        
        KillSystems.Reset();
        BreakLever.Reset();
        MaintainScreenOff.Reset();
        takeOff.Reset();
        
        if (ShipMeltdown.openMonitorSupport)
            ReviveSystemsWithMonitors();
        else
            ReviveSystemsWithoutMonitors();
        
        canTakeOff = true;
        
        foreach ((Light l, Color c) in colors)
            l.color = c;
        colors.Clear();
            
        foreach ((Light l, bool b) in toggleGroup)
        {
            l.enabled = b;
        }

        toggleGroup.Clear();

        if (repeat)
            h.shipDoorsAnimator.SetBool("Closed", true);
    }

    //
    private static void ReviveSystemsWithoutMonitors()
    {
        TryWithoutNullpo((() => {StartOfRound.Instance.deadlineMonitorText.enabled = true;}));
        TryWithoutNullpo((() => {StartOfRound.Instance.deadlineMonitorBGImage.enabled = true;}));
        TryWithoutNullpo((() => {StartOfRound.Instance.profitQuotaMonitorText.enabled = true;}));
        TryWithoutNullpo((() => {StartOfRound.Instance.profitQuotaMonitorBGImage.enabled = true;}));
    }

    private static void ReviveSystemsWithMonitors()
    { 
        if (!OpenMonitors.Monitors.Config.HideCredits.Value)
        {
            CreditsMonitorPatch.meshEnable = true;
            OpenMonitors.Monitors.CreditsMonitor.Instance.UpdateMonitor();
        }
            
            
        if (!OpenMonitors.Monitors.Config.HideLifeSupport.Value)
        {
            LifeSupportMonitorPatch.meshEnable = true;
            OpenMonitors.Monitors.LifeSupportMonitor.Instance.UpdateMonitor();
        }
            
        if (OpenMonitors.Monitors.Config.KeepBlueBackground2.Value)
            StartOfRound.Instance.deadlineMonitorBGImage.enabled = true;
        if (OpenMonitors.Monitors.Config.KeepBlueBackground1.Value)
            StartOfRound.Instance.profitQuotaMonitorBGImage.enabled = true;
            
        StartOfRound.Instance.deadlineMonitorText.enabled = true;
        StartOfRound.Instance.profitQuotaMonitorText.enabled = true;

        if (!OpenMonitors.Monitors.Config.HideCredits.Value)
            OpenMonitors.Monitors.CreditsMonitor.Instance.enabled = true;

        if (!OpenMonitors.Monitors.Config.HideDay.Value)
            OpenMonitors.Monitors.DayMonitor.Instance.textMesh.enabled = true;

        if (!OpenMonitors.Monitors.Config.HideLoot.Value)
            OpenMonitors.Monitors.LootMonitor.Instance.textMesh.enabled = true;

        if (!OpenMonitors.Monitors.Config.HideTime.Value)
            OpenMonitors.Monitors.TimeMonitor.Instance.textMesh.enabled  = true;

        if (!OpenMonitors.Monitors.Config.HideLifeSupport.Value)
            OpenMonitors.Monitors.LifeSupportMonitor.Instance.enabled = true;

        if (!OpenMonitors.Monitors.Config.HidePlayersLifeSupport.Value)
            OpenMonitors.Monitors.PlayersLifeSupportMonitor.Instance.textMesh.enabled = true;
    }
}