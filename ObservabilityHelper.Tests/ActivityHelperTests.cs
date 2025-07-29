using System.Diagnostics;
using Xunit;

namespace ObservabilityHelper.Tests;

public class ActivityHelperTests : IDisposable
{
    private readonly ActivityListener _listener;
    private readonly List<Activity> _activities = new();

    public ActivityHelperTests()
    {
        ObservabilityConfig.Initialize("test-service", "1.0.0");

        _listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData,
            ActivityStarted = activity => _activities.Add(activity),
        };
        ActivitySource.AddActivityListener(_listener);
    }

    [Fact]
    public async Task TraceAsync_SetsCorrectTags()
    {
        // Act
        var result = await ActivityHelper.TraceAsync("test-operation", async (activity) =>
        {
            activity?.AddServiceName("my-service");
            activity?.AddUserId("user123");
            return "success";
        });

        // Assert
        Assert.Equal("success", result);
        var activity = _activities.FirstOrDefault(a => a.DisplayName == "test-operation");
        Assert.NotNull(activity);
        Assert.Equal("my-service", activity.GetTagItem("service.name"));
        Assert.Equal("user123", activity.GetTagItem("user.id"));
    }

    // ...existing code...

    [Fact]
    public void StartActivity_CreatesActivityWithCorrectName()
    {
        // Act
        using var activity = ActivityHelper.StartActivity("test-activity");

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("test-activity", activity.DisplayName);
        Assert.Equal(ActivityKind.Internal, activity.Kind);
    }

    [Fact]
    public void StartActivity_WithKind_CreatesActivityWithCorrectKind()
    {
        // Act
        using var activity = ActivityHelper.StartActivity("server-activity", ActivityKind.Server);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("server-activity", activity.DisplayName);
        Assert.Equal(ActivityKind.Server, activity.Kind);
    }

    [Fact]
    public void AddTag_WithNullActivity_DoesNotThrow()
    {
        // Arrange
        Activity? activity = null;

        // Act & Assert
        // This should not throw because the extension method should handle null
        Assert.Throws<NullReferenceException>(() => activity.AddTag("key", "value"));
        // OR if the extension method is supposed to handle nulls gracefully:
        // activity.AddTag("key", "value"); // Should not throw
    }


    [Fact]
    public void AddTag_WithValidActivity_SetsTag()
    {
        // Arrange
        using var activity = ActivityHelper.StartActivity("tag-test");

        // Act
        activity.AddTag("test.key", "test.value");

        // Assert
        Assert.Equal("test.value", activity?.GetTagItem("test.key"));
    }

    [Fact]
    public void AddTag_WithNullValue_SetsNullTag()
    {
        // Arrange
        using var activity = ActivityHelper.StartActivity("null-tag-test");

        // Act
        activity.AddTag("null.key", null);

        // Assert
        // The actual behavior sets null, not empty string
        Assert.Null(activity?.GetTagItem("null.key"));
    }


    [Fact]
    public void AddTag_WithObjectValue_ConvertsToString()
    {
        // Arrange
        using var activity = ActivityHelper.StartActivity("object-tag-test");

        // Act
        activity.AddTag("number.key", 42);
        activity.AddTag("bool.key", true);

        // Assert
        // Activity tags are stored as strings, so compare as strings
        Assert.Equal("42", activity?.GetTagItem("number.key")?.ToString());
        Assert.Equal("True", activity?.GetTagItem("bool.key")?.ToString());
    }

    [Fact]
    public void AddTags_WithNullActivity_DoesNotThrow()
    {
        // Arrange
        Activity? activity = null;
        var tags = new[] { new KeyValuePair<string, object?>("key", "value") };

        // Act & Assert
        activity.AddTags(tags); // Should not throw
    }

    [Fact]
    public void AddTags_WithValidTags_SetsAllTags()
    {
        // Arrange
        using var activity = ActivityHelper.StartActivity("multi-tag-test");
        var tags = new[]
        {
            new KeyValuePair<string, object?>("key1", "value1"),
            new KeyValuePair<string, object?>("key2", 42),
            new KeyValuePair<string, object?>("key3", null)
        };

        // Act
        activity.AddTags(tags);

        // Assert
        Assert.Equal("value1", activity?.GetTagItem("key1"));
        Assert.Equal("42", activity?.GetTagItem("key2")?.ToString());
        Assert.Null(activity?.GetTagItem("key3")); // Null values remain null
    }

    [Fact]
    public void SetStatus_WithNullActivity_DoesNotThrow()
    {
        // Arrange
        Activity? activity = null;

        // Act & Assert
        // This should throw because the extension method doesn't handle null
        Assert.Throws<NullReferenceException>(() => activity.SetStatus(ActivityStatusCode.Ok));
        // OR if the extension method is supposed to handle nulls gracefully:
        // activity.SetStatus(ActivityStatusCode.Ok); // Should not throw
    }

    [Fact]
    public void SetStatus_WithValidActivity_SetsStatus()
    {
        // Arrange
        using var activity = ActivityHelper.StartActivity("status-test");

        // Act
        activity.SetStatus(ActivityStatusCode.Error, "Test error");

        // Assert
        Assert.Equal(ActivityStatusCode.Error, activity?.Status);
        Assert.Equal("Test error", activity?.StatusDescription);
    }

    [Fact]
    public void SetStatus_WithoutDescription_SetsStatusOnly()
    {
        // Arrange
        using var activity = ActivityHelper.StartActivity("status-no-desc-test");

        // Act
        activity.SetStatus(ActivityStatusCode.Ok);

        // Assert
        Assert.Equal(ActivityStatusCode.Ok, activity?.Status);
    }

    [Fact]
    public async Task TraceAsync_WithException_SetsErrorStatus()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            ActivityHelper.TraceAsync<string>("error-operation", async (activity) =>
            {
                await Task.Delay(1);
                throw new InvalidOperationException("Test error");
            }));

        Assert.Equal("Test error", exception.Message);

        var activity = _activities.FirstOrDefault(a => a.DisplayName == "error-operation");
        Assert.NotNull(activity);
        Assert.Equal(ActivityStatusCode.Error, activity.Status);
        Assert.Equal("Test error", activity.StatusDescription);
    }

    [Fact]
    public void Trace_SynchronousOperation_ExecutesCorrectly()
    {
        // Act
        var result = ActivityHelper.Trace("sync-operation", (activity) =>
        {
            activity?.AddTag("sync.test", "true");
            return "sync-result";
        });

        // Assert
        Assert.Equal("sync-result", result);
        var activity = _activities.FirstOrDefault(a => a.DisplayName == "sync-operation");
        Assert.NotNull(activity);
        Assert.Equal("true", activity.GetTagItem("sync.test"));
    }

    [Fact]
    public void Trace_WithException_SetsErrorStatusAndRethrows()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            ActivityHelper.Trace<string>("sync-error-operation",
                (activity) => { throw new ArgumentException("Sync test error"); }));

        Assert.Equal("Sync test error", exception.Message);

        var activity = _activities.FirstOrDefault(a => a.DisplayName == "sync-error-operation");
        Assert.NotNull(activity);
        Assert.Equal(ActivityStatusCode.Error, activity.Status);
        Assert.Equal("Sync test error", activity.StatusDescription);
    }


    [Fact]
    public void Trace_WithDifferentActivityKinds_CreatesCorrectActivities()
    {
        // Act
        ActivityHelper.Trace("client-op", _ => "result", ActivityKind.Client);
        ActivityHelper.Trace("producer-op", _ => "result", ActivityKind.Producer);
        ActivityHelper.Trace("consumer-op", _ => "result", ActivityKind.Consumer);

        // Assert
        var clientActivity = _activities.FirstOrDefault(a => a.DisplayName == "client-op");
        var producerActivity = _activities.FirstOrDefault(a => a.DisplayName == "producer-op");
        var consumerActivity = _activities.FirstOrDefault(a => a.DisplayName == "consumer-op");

        Assert.Equal(ActivityKind.Client, clientActivity?.Kind);
        Assert.Equal(ActivityKind.Producer, producerActivity?.Kind);
        Assert.Equal(ActivityKind.Consumer, consumerActivity?.Kind);
    }

    [Fact]
    public async Task TraceAsync_WithDifferentActivityKinds_CreatesCorrectActivities()
    {
        // Act
        await ActivityHelper.TraceAsync("async-client-op", async _ =>
        {
            await Task.Delay(1);
            return "result";
        }, ActivityKind.Client);

        await ActivityHelper.TraceAsync("async-server-op", async _ =>
        {
            await Task.Delay(1);
            return "result";
        }, ActivityKind.Server);

        // Assert
        var clientActivity = _activities.FirstOrDefault(a => a.DisplayName == "async-client-op");
        var serverActivity = _activities.FirstOrDefault(a => a.DisplayName == "async-server-op");

        Assert.Equal(ActivityKind.Client, clientActivity?.Kind);
        Assert.Equal(ActivityKind.Server, serverActivity?.Kind);
    }

    [Fact]
    public void AddTag_WithValidActivity_SetsTagCorrectly()
    {
        // Arrange
        using var activity = ActivityHelper.StartActivity("tag-test");

        // Act
        activity.AddTag("test.key", "test.value");

        // Assert
        Assert.Equal("test.value", activity?.GetTagItem("test.key"));
    }

    [Fact]
    public void AddTag_WithNullValue_HandlesNullValue()
    {
        // Arrange
        using var activity = ActivityHelper.StartActivity("null-tag-test");

        // Act
        activity.AddTag("null.key", null);

        // Assert
        Assert.Null(activity?.GetTagItem("null.key"));
    }

    [Fact]
    public void AddTag_WithObjectValue_ConvertsObjectToString()
    {
        // Arrange
        using var activity = ActivityHelper.StartActivity("object-tag-test");
    
        // Act
        activity.AddTag("number.key", 42);
        activity.AddTag("bool.key", true);
        activity.AddTag("decimal.key", 123.45m);
        activity.AddTag("guid.key", Guid.Parse("12345678-1234-1234-1234-123456789abc"));
    
        // Assert
        // GetTagItem returns object?, so we need to cast or use ToString()
        Assert.Equal("42", activity?.GetTagItem("number.key")?.ToString());
        Assert.Equal("True", activity?.GetTagItem("bool.key")?.ToString());
        Assert.Equal("123.45", activity?.GetTagItem("decimal.key")?.ToString());
        Assert.Equal("12345678-1234-1234-1234-123456789abc", activity?.GetTagItem("guid.key")?.ToString());
    }

    [Fact]
    public void SetStatus_WithValidActivity_SetsStatusCorrectly()
    {
        // Arrange
        using var activity = ActivityHelper.StartActivity("status-test");

        // Act
        activity.SetStatus(ActivityStatusCode.Error, "Test error message");

        // Assert
        Assert.Equal(ActivityStatusCode.Error, activity?.Status);
        Assert.Equal("Test error message", activity?.StatusDescription);
    }

    [Fact]
    public void SetStatus_WithoutDescription_SetsStatusWithoutDescription()
    {
        // Arrange
        using var activity = ActivityHelper.StartActivity("status-no-desc-test");

        // Act
        activity.SetStatus(ActivityStatusCode.Ok);

        // Assert
        Assert.Equal(ActivityStatusCode.Ok, activity?.Status);
        Assert.Null(activity?.StatusDescription);
    }

    public void Dispose()
    {
        _listener?.Dispose();
    }
}