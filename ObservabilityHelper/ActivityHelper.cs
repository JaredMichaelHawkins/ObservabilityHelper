using System.Diagnostics;

namespace ObservabilityHelper;

public static class ActivityHelper
{
    public static Activity? StartActivity(string name, ActivityKind kind = ActivityKind.Internal)
    {
        return ObservabilityConfig.ActivitySource.StartActivity(name, kind);
    }

    public static void AddTags(this Activity? activity, IEnumerable<KeyValuePair<string, object?>> tags)
    {
        if (activity == null) return;
        
        foreach (var tag in tags)
        {
            activity.SetTag(tag.Key, tag.Value?.ToString());
        }
    }
    
    
    public static async Task<T> TraceAsync<T>(string activityName, Func<Activity?, Task<T>> operation, ActivityKind kind = ActivityKind.Internal)
    {
        using var activity = StartActivity(activityName, kind);
        try
        {
            return await operation(activity);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }
    
    public static T Trace<T>(string activityName, Func<Activity?, T> operation, ActivityKind kind = ActivityKind.Internal)
    {
        using var activity = StartActivity(activityName, kind);
        try
        {
            return operation(activity);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }
}