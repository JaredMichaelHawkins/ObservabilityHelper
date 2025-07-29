using System.Diagnostics;
using System.Diagnostics.Metrics;


namespace ObservabilityHelper.Tests;

public class MeterHelperTests : IDisposable
{
    private readonly MeterListener _listener;
    private readonly List<(string name, object? value, KeyValuePair<string, object>[] tags)> _measurements = new();

    public MeterHelperTests()
    {
        ObservabilityConfig.Initialize("test-service", "1.0.0");

        _listener = new MeterListener();
        _listener.InstrumentPublished = (instrument, listener) => { listener.EnableMeasurementEvents(instrument); };

        _listener.SetMeasurementEventCallback<int>((instrument, measurement, tags, state) =>
        {
            _measurements.Add((instrument.Name, measurement, tags.ToArray()));
        });

        _listener.SetMeasurementEventCallback<double>((instrument, measurement, tags, state) =>
        {
            _measurements.Add((instrument.Name, measurement, tags.ToArray()));
        });

        _listener.SetMeasurementEventCallback<long>((instrument, measurement, tags, state) =>
        {
            _measurements.Add((instrument.Name, measurement, tags.ToArray()));
        });

        _listener.Start();
    }

    [Fact]
    public void RecordWithTags_TagBuilder_RecordsCorrectly()
    {
        // Arrange
        var counter = MeterHelper.CreateCounter<int>("test-counter");
        var tags = TagBuilder.Create()
            .AddServiceName("test-service")
            .AddUserId("user-123");

        // Act
        counter.RecordWithTags(5, tags);

        // Assert
        var measurement = _measurements.FirstOrDefault(m => m.name == "test-counter");
        Assert.NotNull(measurement.tags);
        Assert.Equal(5, measurement.value);

        var tagDict = measurement.tags.ToDictionary(t => t.Key, t => t.Value);
        Assert.Equal("test-service", tagDict[TagKeys.ServiceName]);
        Assert.Equal("user-123", tagDict[TagKeys.UserId]);
    }

    [Fact]
    public void RecordWithTags_TupleParams_RecordsCorrectly()
    {
        // Arrange
        var counter = MeterHelper.CreateCounter<int>("test-counter-2");

        // Act
        counter.RecordWithTags(10,
            ("service.name", "test-service"),
            ("user.id", "user-456"));

        // Assert
        var measurement = _measurements.FirstOrDefault(m => m.name == "test-counter-2");
        Assert.NotNull(measurement.tags);
        Assert.Equal(10, measurement.value);

        var tagDict = measurement.tags.ToDictionary(t => t.Key, t => t.Value);
        Assert.Equal("test-service", tagDict["service.name"]);
        Assert.Equal("user-456", tagDict["user.id"]);
    }

    [Fact]
    public void RecordWithTagList_RecordsCorrectly()
    {
        // Arrange
        var histogram = MeterHelper.CreateHistogram<double>("test-histogram");
        var tagList = new TagList
        {
            { TagKeys.ServiceName, "test-service" },
            { TagKeys.HttpMethod, "POST" }
        };

        // Act
        histogram.RecordWithTagList(123.45, tagList);

        // Assert
        var measurement = _measurements.FirstOrDefault(m => m.name == "test-histogram");
        Assert.NotNull(measurement.tags);
        Assert.Equal(123.45, measurement.value);

        var tagDict = measurement.tags.ToDictionary(t => t.Key, t => t.Value);
        Assert.Equal("test-service", tagDict[TagKeys.ServiceName]);
        Assert.Equal("POST", tagDict[TagKeys.HttpMethod]);
    }

    [Fact]
    public void HistogramRecordWithTags_TagBuilder_RecordsCorrectly()
    {
        // Arrange
        var histogram = MeterHelper.CreateHistogram<double>("test-histogram-2");
        var tags = TagBuilder.Create()
            .AddHttpStatusCode(200)
            .AddCorrelationId("corr-123");

        // Act
        histogram.RecordWithTags(250.0, tags);

        // Assert
        var measurement = _measurements.FirstOrDefault(m => m.name == "test-histogram-2");
        Assert.NotNull(measurement.tags);
        Assert.Equal(250.0, measurement.value);

        var tagDict = measurement.tags.ToDictionary(t => t.Key, t => t.Value);
        Assert.Equal(200, tagDict[TagKeys.HttpStatusCode]);
        Assert.Equal("corr-123", tagDict[TagKeys.CorrelationId]);
    }

    [Fact]
    public void CreateCounter_CreatesValidCounter()
    {
        // Act
        var counter = MeterHelper.CreateCounter<int>("simple-counter", "requests", "Number of requests");
        counter.Add(1);

        // Assert
        var measurement = _measurements.FirstOrDefault(m => m.name == "simple-counter");
        Assert.Equal(1, measurement.value);
    }

    [Fact]
    public void CreateHistogram_CreatesValidHistogram()
    {
        // Act
        var histogram = MeterHelper.CreateHistogram<double>("simple-histogram", "ms", "Request duration");
        histogram.Record(250.5);

        // Assert
        var measurement = _measurements.FirstOrDefault(m => m.name == "simple-histogram");
        Assert.Equal(250.5, measurement.value);
    }

    [Fact]
    public void MultipleTagsWithDifferentTypes_RecordsCorrectly()
    {
        // Arrange
        var counter = MeterHelper.CreateCounter<long>("mixed-tags-counter");
        var tags = TagBuilder.Create()
            .AddServiceName("test-service") // string
            .AddHttpStatusCode(200) // int
            .AddMessagingKafkaOffset(12345L) // long
            .AddTag("custom.boolean", true) // boolean
            .AddTag("custom.double", 123.45); // double

        // Act
        counter.RecordWithTags(1L, tags);

        // Assert
        var measurement = _measurements.FirstOrDefault(m => m.name == "mixed-tags-counter");
        Assert.NotNull(measurement.tags);
        Assert.Equal(1L, measurement.value);

        var tagDict = measurement.tags.ToDictionary(t => t.Key, t => t.Value);
        Assert.Equal("test-service", tagDict[TagKeys.ServiceName]);
        Assert.Equal(200, tagDict[TagKeys.HttpStatusCode]);
        Assert.Equal(12345L, tagDict[TagKeys.MessagingKafkaOffset]);
        Assert.Equal(true, tagDict["custom.boolean"]);
        Assert.Equal(123.45, tagDict["custom.double"]);
    }

    // ...existing code...

    [Fact]
    public void CreateObservableGauge_CreatesValidGauge()
    {
        // Arrange
        var testValue = 42;

        // Act
        var gauge = MeterHelper.CreateObservableGauge("test-gauge", () => testValue, "connections",
            "Active connections");

        // Assert
        Assert.NotNull(gauge);
        Assert.Equal("test-gauge", gauge.Name);
        Assert.Equal("connections", gauge.Unit);
        Assert.Equal("Active connections", gauge.Description);
    }

    [Fact]
    public void CreateCounterWithoutOptionalParameters_CreatesCorrectly()
    {
        // Act
        var counter = MeterHelper.CreateCounter<int>("basic-counter");
        counter.Add(1);

        // Assert
        var measurement = _measurements.FirstOrDefault(m => m.name == "basic-counter");
        Assert.Equal(1, measurement.value);
    }

    [Fact]
    public void CreateHistogramWithoutOptionalParameters_CreatesCorrectly()
    {
        // Act
        var histogram = MeterHelper.CreateHistogram<double>("basic-histogram");
        histogram.Record(3.14);

        // Allow time for measurements to be captured
        Thread.Sleep(10);

        // Assert
        var measurement = _measurements.FirstOrDefault(m => m.name == "basic-histogram");
        Assert.NotNull(measurement.value);
        Assert.Equal(3.14, measurement.value);
    }

    [Fact]
    public void RecordWithTags_EmptyTagBuilder_RecordsCorrectly()
    {
        // Arrange
        var counter = MeterHelper.CreateCounter<int>("empty-tags-counter");
        var tags = TagBuilder.Create();

        // Act
        counter.RecordWithTags(5, tags);

        // Allow time for measurement to be captured
        Thread.Sleep(10);

        // Assert
        var measurement = _measurements.FirstOrDefault(m => m.name == "empty-tags-counter");
        Assert.NotNull(measurement.value);
        Assert.Equal(5, measurement.value);
        Assert.Empty(measurement.tags);
    }

    [Fact]
    public void RecordWithTags_EmptyTupleParams_RecordsCorrectly()
    {
        // Arrange
        var counter = MeterHelper.CreateCounter<int>("no-params-counter");

        // Act
        counter.RecordWithTags(7);

        // Assert
        var measurement = _measurements.FirstOrDefault(m => m.name == "no-params-counter");
        Assert.Equal(7, measurement.value);
        Assert.Empty(measurement.tags);
    }

    [Fact]
    public void RecordWithTagList_EmptyTagList_RecordsCorrectly()
    {
        // Arrange
        var histogram = MeterHelper.CreateHistogram<double>("empty-taglist-histogram");
        var tagList = new TagList();

        // Act
        histogram.RecordWithTagList(99.99, tagList);

        // Assert
        var measurement = _measurements.FirstOrDefault(m => m.name == "empty-taglist-histogram");
        Assert.Equal(99.99, measurement.value);
        Assert.Empty(measurement.tags);
    }

    [Fact]
    public void RecordWithTags_NullValues_RecordsCorrectly()
    {
        // Arrange
        var counter = MeterHelper.CreateCounter<int>("null-tags-counter");

        // Act
        counter.RecordWithTags(3,
            ("key1", "value1"),
            ("key2", null),
            ("key3", "value3"));

        // Assert
        var measurement = _measurements.FirstOrDefault(m => m.name == "null-tags-counter");
        Assert.Equal(3, measurement.value);

        var tagDict = measurement.tags.ToDictionary(t => t.Key, t => t.Value);
        Assert.Equal("value1", tagDict["key1"]);
        Assert.Null(tagDict["key2"]);
        Assert.Equal("value3", tagDict["key3"]);
    }

    [Fact]
    public void HistogramRecordWithTags_TupleParams_RecordsCorrectly()
    {
        // Arrange
        var histogram = MeterHelper.CreateHistogram<double>("tuple-histogram");

        // Act
        histogram.RecordWithTags(456.78,
            ("operation", "process"),
            ("status", "success"));

        // Allow time for measurement to be captured
        Thread.Sleep(10);

        // Assert
        var measurement = _measurements.FirstOrDefault(m => m.name == "tuple-histogram");
        Assert.NotNull(measurement.value);
        Assert.Equal(456.78, (double)measurement.value, precision: 10); // Use precision for double comparison

        var tagDict = measurement.tags.ToDictionary(t => t.Key, t => t.Value);
        Assert.Equal("process", tagDict["operation"]);
        Assert.Equal("success", tagDict["status"]);
    }

    [Fact]
    public void CreateCounterWithDifferentNumericTypes_CreatesCorrectly()
    {
        // Act
        var intCounter = MeterHelper.CreateCounter<int>("int-counter");
        var longCounter = MeterHelper.CreateCounter<long>("long-counter");
        var doubleCounter = MeterHelper.CreateCounter<double>("double-counter");

        intCounter.Add(1);
        longCounter.Add(2L);
        doubleCounter.Add(4.0);

        // Allow time for measurements to be captured
        Thread.Sleep(10);

        // Assert
        Assert.Equal(1, _measurements.First(m => m.name == "int-counter").value);
        Assert.Equal(2L, _measurements.First(m => m.name == "long-counter").value);
        Assert.Equal(4.0, _measurements.First(m => m.name == "double-counter").value);
    }

    [Fact]
    public void CreateHistogramWithDifferentNumericTypes_CreatesCorrectly()
    {
        // Act
        var intHistogram = MeterHelper.CreateHistogram<int>("int-histogram");
        var longHistogram = MeterHelper.CreateHistogram<long>("long-histogram");
        var doubleHistogram = MeterHelper.CreateHistogram<double>("double-histogram");

        intHistogram.Record(10);
        longHistogram.Record(20L);
        doubleHistogram.Record(40.5);

        // Allow time for measurements to be captured
        Thread.Sleep(10);

        // Assert
        Assert.Equal(10, _measurements.First(m => m.name == "int-histogram").value);
        Assert.Equal(20L, _measurements.First(m => m.name == "long-histogram").value);
        Assert.Equal(40.5, _measurements.First(m => m.name == "double-histogram").value);
    }

    [Fact]
    public void RecordWithTagList_Counter_RecordsCorrectly()
    {
        // Arrange
        var counter = MeterHelper.CreateCounter<int>("taglist-counter");
        var tagList = new TagList
        {
            { "service.name", "test-service" },
            { "operation", "create" },
            { "status", "success" }
        };

        // Act
        counter.RecordWithTagList(42, tagList);

        // Allow time for measurement to be captured
        Thread.Sleep(10);

        // Assert
        var measurement = _measurements.FirstOrDefault(m => m.name == "taglist-counter");
        Assert.NotNull(measurement.value);
        Assert.Equal(42, measurement.value);

        var tagDict = measurement.tags.ToDictionary(t => t.Key, t => t.Value);
        Assert.Equal("test-service", tagDict["service.name"]);
        Assert.Equal("create", tagDict["operation"]);
        Assert.Equal("success", tagDict["status"]);
    }

    [Fact]
    public void RecordWithTagList_CounterWithEmptyTagList_RecordsCorrectly()
    {
        // Arrange
        var counter = MeterHelper.CreateCounter<long>("empty-taglist-counter");
        var tagList = new TagList();

        // Act
        counter.RecordWithTagList(100L, tagList);

        // Allow time for measurement to be captured
        Thread.Sleep(10);

        // Assert
        var measurement = _measurements.FirstOrDefault(m => m.name == "empty-taglist-counter");
        Assert.NotNull(measurement.value);
        Assert.Equal(100L, measurement.value);
        Assert.Empty(measurement.tags);
    }

    [Fact]
    public void RecordWithTagList_CounterWithMixedTypes_RecordsCorrectly()
    {
        // Arrange
        var counter = MeterHelper.CreateCounter<double>("mixed-taglist-counter");
        var tagList = new TagList
        {
            { "string.value", "text" },
            { "int.value", 123 },
            { "bool.value", true },
            { "double.value", 45.67 }
        };

        // Act
        counter.RecordWithTagList(99.99, tagList);

        // Allow time for measurement to be captured
        Thread.Sleep(10);

        // Assert
        var measurement = _measurements.FirstOrDefault(m => m.name == "mixed-taglist-counter");
        Assert.NotNull(measurement.value);
        Assert.Equal(99.99, (double)measurement.value, precision: 10);

        var tagDict = measurement.tags.ToDictionary(t => t.Key, t => t.Value);
        Assert.Equal("text", tagDict["string.value"]);
        Assert.Equal(123, tagDict["int.value"]);
        Assert.Equal(true, tagDict["bool.value"]);
        Assert.Equal(45.67, tagDict["double.value"]);
    }

    public void Dispose()
    {
        _listener?.Dispose();
    }
}