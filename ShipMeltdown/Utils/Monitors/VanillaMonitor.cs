namespace ShipMeltdown.Utils.Monitors;

public class VanillaMonitor : MonitorCompatibility
{
    public void MostSystemsDead()
    {
        ShipPanic.TryWithoutNullpo((() => {StartOfRound.Instance.deadlineMonitorText.enabled = false;}));
        ShipPanic.TryWithoutNullpo((() => {StartOfRound.Instance.deadlineMonitorBGImage.enabled = false;}));
        ShipPanic.TryWithoutNullpo((() => {StartOfRound.Instance.profitQuotaMonitorText.enabled = false;}));
        ShipPanic.TryWithoutNullpo((() => {StartOfRound.Instance.profitQuotaMonitorBGImage.enabled = false;}));
    }

    public void ReviveSystems()
    {
        ShipPanic.TryWithoutNullpo((() => {StartOfRound.Instance.deadlineMonitorText.enabled = true;}));
        ShipPanic.TryWithoutNullpo((() => {StartOfRound.Instance.deadlineMonitorBGImage.enabled = true;}));
        ShipPanic.TryWithoutNullpo((() => {StartOfRound.Instance.profitQuotaMonitorText.enabled = true;}));
        ShipPanic.TryWithoutNullpo((() => {StartOfRound.Instance.profitQuotaMonitorBGImage.enabled = true;}));
    }
}