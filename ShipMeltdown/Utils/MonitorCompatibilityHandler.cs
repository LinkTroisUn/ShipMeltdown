using System.Text;
using Dissonance;
using ShipMeltdown.Utils.Monitors;

namespace ShipMeltdown.Utils;

// Class to use to add your own monitor support (actually, could be any effects following a similar logic)
// You could call this one an API
public static class MonitorCompatibilityHandler
{
    private static Queue<MonitorCompatibility> handlers = new Queue<MonitorCompatibility>();
    
    // Use this function to register your own handler.
    // If you expect the default behaviour of the monitors to be applied along with your owns,
    // do not forget to put removeDefaultHandler as false.
    // You can only register one instance of a given type
    public static void AddMonitorCompatibilityHandler(MonitorCompatibility mc, bool removeDefaultHandler = true)
    {
        foreach (MonitorCompatibility mcInternal in handlers)
        {
            if (mc.GetType() == mcInternal.GetType())
            {
                LogQueueState(false, mc.GetType().ToString());
                return;
            }
        }
        
        MonitorCompatibility? test = null;

        // The default one should be the first element of the queue
        if (removeDefaultHandler && handlers.TryPeek(out test) && test.GetType() == typeof(DefaultMonitor))
            handlers.Dequeue();
        
        handlers.Enqueue(mc);
        
        LogQueueState();
    }

    private static void LogQueueState(bool success = true, string? type = null)
    {
        StringBuilder s;
        if (success)
        {
            s = new StringBuilder("A new monitor handler has been registered. ");
        }
        else
        {
            s = new StringBuilder("Not registering handler of type ");
            s.Append(type);
            s.Append(" because one of the same type has already been registered. ");
        }

        s.AppendLine($"There are now {handlers.Count} handlers loaded");
        
        int i = 0;
        foreach (MonitorCompatibility mcInternal in handlers)
        {
            s.Append(i++);
            s.Append(" :");
            s.AppendLine(mcInternal.GetType().ToString());
        } 
        ShipMeltdown.mls.LogInfo(s);
    }
    
    internal static void MaintainScreenOff()
    {
        foreach (MonitorCompatibility mc in handlers)
        {
            mc.MaintainScreenOff().Run();
        }
    }

    internal static void MaintainScreenOffReset()
    {
        foreach (MonitorCompatibility mc in handlers)
        {
            mc.MaintainScreenOff().Reset();
        }
    }

    internal static void MostSystemsDead()
    {
        foreach (MonitorCompatibility mc in handlers)
        {
            mc.MostSystemsDead();
        }
    }

    internal static void ReviveSystems()
    {
        foreach (MonitorCompatibility mc in handlers)
        {
            mc.ReviveSystems();
        }
    }
}