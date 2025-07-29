using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ObservabilityHelper;

public static class ObservabilityConfig
{
    private static ActivitySource? _activitySource;
    private static Meter? _meter;
    
    public static void Initialize(string serviceName, string? serviceVersion = null)
    {
        _activitySource?.Dispose();
        _meter?.Dispose();
        
        _activitySource = new ActivitySource(serviceName, serviceVersion);
        _meter = new Meter(serviceName, serviceVersion);
    }
    
    internal static ActivitySource ActivitySource => _activitySource 
                                                     ?? throw new InvalidOperationException("ObservabilityConfig not initialized. Call Initialize() first.");
    
    internal static Meter Meter => _meter 
                                   ?? throw new InvalidOperationException("ObservabilityConfig not initialized. Call Initialize() first.");
}