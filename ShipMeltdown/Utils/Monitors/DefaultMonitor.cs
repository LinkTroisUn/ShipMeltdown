namespace ShipMeltdown.Utils.Monitors;

public class DefaultMonitor : MonitorCompatibility
{
    private ControlledTask maintainScreenOff = new ControlledTask(() => StartOfRound.Instance.mapScreen.SwitchScreenOn(false), false);
    public ControlledTask MaintainScreenOff()
    {
        return maintainScreenOff;
    }

    public void MostSystemsDead()
    {
        ShipPanic.TryWithoutNullPo((() => {StartOfRound.Instance.deadlineMonitorText.enabled = false;}));
        ShipPanic.TryWithoutNullPo((() => {StartOfRound.Instance.deadlineMonitorBGImage.enabled = false;}));
        ShipPanic.TryWithoutNullPo((() => {StartOfRound.Instance.profitQuotaMonitorText.enabled = false;}));
        ShipPanic.TryWithoutNullPo((() => {StartOfRound.Instance.profitQuotaMonitorBGImage.enabled = false;}));
    }

    public void ReviveSystems()
    {
        ShipPanic.TryWithoutNullPo((() => {StartOfRound.Instance.deadlineMonitorText.enabled = true;}));
        ShipPanic.TryWithoutNullPo((() => {StartOfRound.Instance.deadlineMonitorBGImage.enabled = true;}));
        ShipPanic.TryWithoutNullPo((() => {StartOfRound.Instance.profitQuotaMonitorText.enabled = true;}));
        ShipPanic.TryWithoutNullPo((() => {StartOfRound.Instance.profitQuotaMonitorBGImage.enabled = true;}));
    }
}