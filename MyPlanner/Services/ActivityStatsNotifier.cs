namespace MyPlanner.Services;

public sealed class ActivityStatsNotifier
{
    public event Action? Changed;

    public void Notify() => Changed?.Invoke();
}
