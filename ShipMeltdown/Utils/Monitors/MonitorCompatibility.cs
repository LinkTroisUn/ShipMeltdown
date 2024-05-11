namespace ShipMeltdown.Utils;

// Interface used to create new monitor support (actually, could be any effect following a similar logic)
// Keep in mind that only one instance of each interface implementation will be loaded using MonitorCompatibilityHandler
public interface MonitorCompatibility
{
    // Advice : if you need to patch things for your effects to take place, you can do it in the empty constructor
    // of your MonitorCompatibility implementation
    
    // Called just after MostSystemsDead(). Fill with something if you need to enforce something every frame
    public ControlledTask MaintainScreenOff();
    
    // Called at T-30s before explosion. Activates the effects
    public void MostSystemsDead();
    
    // Called during meltdown onDisable(). Revert everything you have altered to its original state (if required)
    public void ReviveSystems();

}