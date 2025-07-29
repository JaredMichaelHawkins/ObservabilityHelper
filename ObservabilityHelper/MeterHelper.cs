using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ObservabilityHelper;

public static class MeterHelper
{
    public static Counter<T> CreateCounter<T>(string name, string? unit = null, string? description = null) where T : struct
    {
        return ObservabilityConfig.Meter.CreateCounter<T>(name, unit, description);
    }
    
    public static Histogram<T> CreateHistogram<T>(string name, string? unit = null, string? description = null) where T : struct
    {
        return ObservabilityConfig.Meter.CreateHistogram<T>(name, unit, description);
    }
    
    public static ObservableGauge<T> CreateObservableGauge<T>(string name, Func<T> observeValue, string? unit = null, string? description = null) where T : struct
    {
        return ObservabilityConfig.Meter.CreateObservableGauge<T>(name, observeValue, unit, description);
    }

    // Convenience methods with TagBuilder
    public static void RecordWithTags<T>(this Counter<T> counter, T value, TagBuilder tags) where T : struct
    {
        counter.Add(value, tags.ToTagList());
    }
    
    public static void RecordWithTags<T>(this Histogram<T> histogram, T value, TagBuilder tags) where T : struct
    {
        histogram.Record(value, tags.ToTagList());
    }
    
    // For simple tag scenarios using params array
    public static void RecordWithTags<T>(this Counter<T> counter, T value, params (string key, object? value)[] tags) where T : struct
    {
        var tagArray = tags.Select(t => new KeyValuePair<string, object?>(t.key, t.value)).ToArray();
        counter.Add(value, tagArray);
    }
    
    public static void RecordWithTags<T>(this Histogram<T> histogram, T value, params (string key, object? value)[] tags) where T : struct
    {
        var tagArray = tags.Select(t => new KeyValuePair<string, object?>(t.key, t.value)).ToArray();
        histogram.Record(value, tagArray);
    }

    // Alternative using TagList directly
    public static void RecordWithTagList<T>(this Counter<T> counter, T value, TagList tags) where T : struct
    {
        counter.Add(value, tags);
    }
    
    public static void RecordWithTagList<T>(this Histogram<T> histogram, T value, TagList tags) where T : struct
    {
        histogram.Record(value, tags);
    }
}