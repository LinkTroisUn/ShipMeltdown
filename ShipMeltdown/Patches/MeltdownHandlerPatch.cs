using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using FacilityMeltdown;
using FacilityMeltdown.MeltdownSequence.Behaviours;
using HarmonyLib;
using UnityEngine;

namespace ShipMeltdown.Patches;

[HarmonyPatch(typeof(MeltdownHandler))]
public class MeltdownHandlerPatch
{
    
    [HarmonyPrefix, HarmonyPatch("OnDisable")]
    private static void onDisablePatch()
    {
        ShipPanic.ReviveSystems();
        ShipPanic.meltdownTimer = 120f;
    }

    [HarmonyPostfix, HarmonyPatch("Update")]
    private static void UpdatePatch()
    {
        ShipPanic.meltdownTimer -= Time.deltaTime;

        // if non explosion has occured then panic
        if (ShipPanic.meltdownTimer >= 0f)
        {
            ShipPanic.delta += Time.deltaTime;
            ShipPanic.delta2 += Time.deltaTime;
        }

        // if waited long enough then toggle lights
        if (ShipPanic.meltdownTimer >= 0 && ShipPanic.delta > 0.5f + (ShipPanic.meltdownTimer / 120f) * 5f)
        {
            ShipPanic.delta = 0f;

            foreach ((Light l, bool b) in ShipPanic.toggleGroup)
            {
                l.enabled = b && !l.enabled;
            }
            
            StartOfRound.Instance.shipRoomLights.ToggleShipLights();
        }
        
        // Oops, the screens died
        if (ShipPanic.meltdownTimer <= 30f)
        {
            ShipPanic.KillSystems.Run();
            ShipPanic.maintainScreeOff.Run();
        }

        // if enabled in config and waited long enough then make the shipdoor panic
        if (Config.Instance.shipDoorMalfunction.Value && ShipPanic.meltdownTimer >= 0 && ShipPanic.delta2 > 10f)
        {
            ShipPanic.h.shipDoorsAnimator.SetBool("Closed", ShipPanic.repeat);

            ShipPanic.repeat = !ShipPanic.repeat;
            ShipPanic.delta2 = 0f;
        }

        // Oops, the lever won't work :)
        if (ShipPanic.meltdownTimer < 3f)
        { 
            ShipPanic.BreakLever.Run();
        }
        
        // Anyways, if the ship never takes off, the game is softlocked
        // So better make it take off after we have waited long enough to be sure the explosion has reached the ship,
        // even if the ship is far away from the facility (on some modded moons)
        if (ShipPanic.meltdownTimer <= -16f)
        {
            ShipPanic.takeOff.Run();
        }
    }
}