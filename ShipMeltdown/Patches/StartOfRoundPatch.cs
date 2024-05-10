using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShipMeltdown.Patches;

[HarmonyPatch(typeof(StartOfRound))]
internal static class StartOfRoundPatch
{
    internal static DialogueSegment[] failure; // read around explosion time
    private static int emergencyType = 1;      // for manual emergency lights toggle
    internal static float delta = 0f;
    
    // Prevent the ship from taking off when the nuclear explosion occurs (original reason for this mod)
    [HarmonyPrefix, HarmonyPatch("ShipLeave")]
    internal static bool ShipLeavePatch()
    {
        if (!ShipPanic.CanTakeOff)
        {
            StartOfRound.Instance.shipIsLeaving = false;
            HUDManager.Instance.ReadDialogue(failure);
            return false;
        }

        return true;
    }

    [HarmonyPostfix, HarmonyPatch("Update")]
    private static void UpdatePatch()
    {

#if DEBUG // To test the mod without having to enter the facility in search of the apparatus
        if (Keyboard.current.f7Key.wasPressedThisFrame || Keyboard.current.f8Key.isPressed)
        {
            FacilityMeltdown.API.MeltdownAPI.StartMeltdown(ShipMeltdown.modGUID);
        }
#endif 
        // Manual emergency lights toggle
        if (Keyboard.current[Config.Instance.toggleEmergencyLights.Value].wasReleasedThisFrame && !FacilityMeltdown.API.MeltdownAPI.MeltdownStarted && StartOfRound.Instance.localPlayerController.isInElevator)
        {
            ShipPanic.LightAlarm(emergencyType);
            emergencyType = -emergencyType;
        }
        
        // Switch on/off every 5s
        if (ShipPanic.OnlyLights && delta >= 5f)
        {
            ShipPanic.ToggleLightsOnly();
            delta = 0f;
        }
        else if (ShipPanic.OnlyLights)
            delta += Time.deltaTime; 
    }
}