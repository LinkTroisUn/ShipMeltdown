namespace ShipMeltdown.Utils;

internal interface MonitorCompatibility
{

    internal ControlledTask MaintainScreenOff();
    internal void MostSystemsDead();

    internal void ReviveSystems();

}