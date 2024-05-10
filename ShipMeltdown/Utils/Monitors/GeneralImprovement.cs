using GeneralImprovements.Utilities;

namespace ShipMeltdown.Utils.Monitors;

public class GeneralImprovement : MonitorCompatibility
{
    public void MostSystemsDead()
    {
        StartOfRound.Instance.mapScreen.SwitchScreenOn(false);
        //MonitorsHelper.ToggleExtraMonitorsPower(false);
    }

    public void ReviveSystems()
    {
        StartOfRound.Instance.mapScreen.SwitchScreenOn(true);
        //MonitorsHelper.ToggleExtraMonitorsPower(true);
    }
}