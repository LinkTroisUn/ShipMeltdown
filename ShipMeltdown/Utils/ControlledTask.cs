namespace ShipMeltdown.Utils;

// Purpose of this class is to allow actions placed in the main Update loop to be performed only once
// instead of being called every frame
// Also a remnant of the first very quickly written version of the mod
public class ControlledTask(Action action, bool shouldBeDoneOnlyOnce)
{
    private Action action = action;
    private bool hasBeenPerformed;
    private readonly bool shouldBeDoneOnlyOnce = shouldBeDoneOnlyOnce;

    public void Reset() => this.hasBeenPerformed = false;

    public void Run()
    {
        if (shouldBeDoneOnlyOnce && !hasBeenPerformed || !shouldBeDoneOnlyOnce)
        {
            action.Invoke();
            hasBeenPerformed = true;
        }
    }
}