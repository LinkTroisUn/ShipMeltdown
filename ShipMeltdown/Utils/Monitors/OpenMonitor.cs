﻿using ShipMeltdown.Patches;

namespace ShipMeltdown.Utils.Monitors;

public class OpenMonitor : MonitorCompatibility
{

    public void MostSystemsDead()
    {
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

    public void ReviveSystems()
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