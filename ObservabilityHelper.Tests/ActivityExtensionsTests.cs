using System.Diagnostics;
using Xunit;

namespace ObservabilityHelper.Tests;

public class ActivityExtensionsTests : IDisposable
{
    private readonly ActivitySource _activitySource;
    private readonly ActivityListener _listener;

    public ActivityExtensionsTests()
    {
        _activitySource = new ActivitySource("test-source", "1.0.0");
        
        _listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData,
        };
        ActivitySource.AddActivityListener(_listener);
    }

    [Fact]
    public void AddServiceName_SetsCorrectTag()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("test");
        
        // Act
        activity.AddServiceName("test-service");
        
        // Assert
        Assert.Equal("test-service", activity?.GetTagItem(TagKeys.ServiceName));
    }

    [Fact]
    public void AddServiceVersion_SetsCorrectTag()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("test");
        
        // Act
        activity.AddServiceVersion("1.2.3");
        
        // Assert
        Assert.Equal("1.2.3", activity?.GetTagItem(TagKeys.ServiceVersion));
    }

    [Fact]
    public void AddServiceInstance_SetsCorrectTag()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("test");
        
        // Act
        activity.AddServiceInstance("instance-123");
        
        // Assert
        Assert.Equal("instance-123", activity?.GetTagItem(TagKeys.ServiceInstance));
    }

    [Fact]
    public void AddHttpMethod_SetsCorrectTag()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("test");
        
        // Act
        activity.AddHttpMethod("POST");
        
        // Assert
        Assert.Equal("POST", activity?.GetTagItem(TagKeys.HttpMethod));
    }

    [Fact]
    public void AddHttpUrl_SetsCorrectTag()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("test");
        
        // Act
        activity.AddHttpUrl("https://api.example.com/orders");
        
        // Assert
        Assert.Equal("https://api.example.com/orders", activity?.GetTagItem(TagKeys.HttpUrl));
    }

    [Fact]
    public void AddHttpStatusCode_SetsCorrectTag()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("test");
        
        // Act
        activity.AddHttpStatusCode(201);
        
        // Assert
        Assert.Equal(201, activity?.GetTagItem(TagKeys.HttpStatusCode));
    }

    [Fact]
    public void AddHttpUserAgent_SetsCorrectTag()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("test");
        
        // Act
        activity.AddHttpUserAgent("Mozilla/5.0");
        
        // Assert
        Assert.Equal("Mozilla/5.0", activity?.GetTagItem(TagKeys.HttpUserAgent));
    }

    [Fact]
    public void AddDbSystem_SetsCorrectTag()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("test");
        
        // Act
        activity.AddDbSystem("postgresql");
        
        // Assert
        Assert.Equal("postgresql", activity?.GetTagItem(TagKeys.DbSystem));
    }

    [Fact]
    public void AddDbName_SetsCorrectTag()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("test");
        
        // Act
        activity.AddDbName("orders_db");
        
        // Assert
        Assert.Equal("orders_db", activity?.GetTagItem(TagKeys.DbName));
    }

    [Fact]
    public void AddDbOperation_SetsCorrectTag()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("test");
        
        // Act
        activity.AddDbOperation("SELECT");
        
        // Assert
        Assert.Equal("SELECT", activity?.GetTagItem(TagKeys.DbOperation));
    }

    [Fact]
    public void AddDbStatement_SetsCorrectTag()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("test");
        
        // Act
        activity.AddDbStatement("SELECT * FROM orders WHERE id = ?");
        
        // Assert
        Assert.Equal("SELECT * FROM orders WHERE id = ?", activity?.GetTagItem(TagKeys.DbStatement));
    }

    [Fact]
    public void AddMessagingSystem_SetsCorrectTag()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("test");
        
        // Act
        activity.AddMessagingSystem("kafka");
        
        // Assert
        Assert.Equal("kafka", activity?.GetTagItem(TagKeys.MessagingSystem));
    }

    [Fact]
    public void AddMessagingDestination_SetsCorrectTag()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("test");
        
        // Act
        activity.AddMessagingDestination("order-events");
        
        // Assert
        Assert.Equal("order-events", activity?.GetTagItem(TagKeys.MessagingDestination));
    }

    [Fact]
    public void AddMessagingOperation_SetsCorrectTag()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("test");
        
        // Act
        activity.AddMessagingOperation("publish");
        
        // Assert
        Assert.Equal("publish", activity?.GetTagItem(TagKeys.MessagingOperation));
    }

    [Fact]
    public void AddMessagingKafkaTopic_SetsCorrectTag()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("test");
        
        // Act
        activity.AddMessagingKafkaTopic("order-created");
        
        // Assert
        Assert.Equal("order-created", activity?.GetTagItem(TagKeys.MessagingKafkaTopic));
    }

    [Fact]
    public void AddMessagingKafkaPartition_SetsCorrectTag()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("test");
        
        // Act
        activity.AddMessagingKafkaPartition(3);
        
        // Assert
        Assert.Equal(3, activity?.GetTagItem(TagKeys.MessagingKafkaPartition));
    }

    [Fact]
    public void AddMessagingKafkaOffset_SetsCorrectTag()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("test");
        
        // Act
        activity.AddMessagingKafkaOffset(12345L);
        
        // Assert
        Assert.Equal(12345L, activity?.GetTagItem(TagKeys.MessagingKafkaOffset));
    }

    [Fact]
    public void AddUserId_SetsCorrectTag()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("test");
        
        // Act
        activity.AddUserId("user-123");
        
        // Assert
        Assert.Equal("user-123", activity?.GetTagItem(TagKeys.UserId));
    }

    [Fact]
    public void AddUserEmail_SetsCorrectTag()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("test");
        
        // Act
        activity.AddUserEmail("user@example.com");
        
        // Assert
        Assert.Equal("user@example.com", activity?.GetTagItem(TagKeys.UserEmail));
    }

    [Fact]
    public void AddUserName_SetsCorrectTag()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("test");
        
        // Act
        activity.AddUserName("john.doe");
        
        // Assert
        Assert.Equal("john.doe", activity?.GetTagItem(TagKeys.UserName));
    }

    [Fact]
    public void AddErrorType_SetsCorrectTag()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("test");
        
        // Act
        activity.AddErrorType("ValidationException");
        
        // Assert
        Assert.Equal("ValidationException", activity?.GetTagItem(TagKeys.ErrorType));
    }

    [Fact]
    public void AddErrorMessage_SetsCorrectTag()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("test");
        
        // Act
        activity.AddErrorMessage("Invalid order data");
        
        // Assert
        Assert.Equal("Invalid order data", activity?.GetTagItem(TagKeys.ErrorMessage));
    }

    [Fact]
    public void AddCorrelationId_SetsCorrectTag()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("test");
        
        // Act
        activity.AddCorrelationId("corr-123");
        
        // Assert
        Assert.Equal("corr-123", activity?.GetTagItem(TagKeys.CorrelationId));
    }

    [Fact]
    public void AddTenantId_SetsCorrectTag()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("test");
        
        // Act
        activity.AddTenantId("tenant-456");
        
        // Assert
        Assert.Equal("tenant-456", activity?.GetTagItem(TagKeys.TenantId));
    }

    [Fact]
    public void AddRequestId_SetsCorrectTag()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("test");
        
        // Act
        activity.AddRequestId("req-789");
        
        // Assert
        Assert.Equal("req-789", activity?.GetTagItem(TagKeys.RequestId));
    }

    [Fact]
    public void NullActivity_DoesNotThrow()
    {
        // Arrange
        Activity? activity = null;
        
        // Act & Assert - should not throw
        activity.AddServiceName("test");
        activity.AddUserId("user");
        activity.AddHttpMethod("GET");
    }

    [Fact]
    public void MultipleTags_AllAppliedCorrectly()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("test");
        
        // Act
        activity.AddServiceName("test-service");
        activity.AddUserId("user-123");
        activity.AddHttpMethod("POST");
        activity.AddCorrelationId("corr-456");
        
        // Assert
        Assert.Equal("test-service", activity?.GetTagItem(TagKeys.ServiceName));
        Assert.Equal("user-123", activity?.GetTagItem(TagKeys.UserId));
        Assert.Equal("POST", activity?.GetTagItem(TagKeys.HttpMethod));
        Assert.Equal("corr-456", activity?.GetTagItem(TagKeys.CorrelationId));
    }

    public void Dispose()
    {
        _listener?.Dispose();
        _activitySource?.Dispose();
    }
}