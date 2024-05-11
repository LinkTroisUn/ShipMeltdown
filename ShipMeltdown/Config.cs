using BepInEx.Configuration;
using UnityEngine.InputSystem;

namespace ShipMeltdown;

public class Config
{
    internal static Config? Instance;

    internal ConfigEntry<Key> toggleEmergencyLights;
    public Key getToggleEmergencyLights => toggleEmergencyLights.Value;

    internal ConfigEntry<bool> shipDoorMalfunction;
    public bool getShipDoorMalfunction => shipDoorMalfunction.Value;
    
    public Config(ConfigFile file)
    {
        Instance = this;
        toggleEmergencyLights = file.Bind("Main", "Toggle Emergency Lights", Key.F6);
        shipDoorMalfunction = file.Bind("Main", "Ship Doors Malfunction", true);
    }
}