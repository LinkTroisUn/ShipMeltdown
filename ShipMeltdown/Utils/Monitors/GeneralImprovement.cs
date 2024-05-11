using GeneralImprovements.Utilities;

namespace ShipMeltdown.Utils.Monitors;

public class GeneralImprovement : MonitorCompatibility
{
    private ControlledTask maintainScreenOff = new ControlledTask(() => throw new NotImplementedException(), false);
    public ControlledTask MaintainScreenOff()
    {
        return maintainScreenOff;
    }

    public void MostSystemsDead()
    {
        StartOfRound.Instance.mapScreen.SwitchScreenOn(false);
        throw new NotImplementedException();
    }

    public void ReviveSystems()
    {
        StartOfRound.Instance.mapScreen.SwitchScreenOn(true);
        throw new NotImplementedException();
    }
}