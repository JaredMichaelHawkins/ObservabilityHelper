using Xunit;

namespace ObservabilityHelper.Tests;

public class TagBuilderTests
{
    [Fact]
    public void Create_ReturnsEmptyBuilder()
    {
        // Act
        var builder = TagBuilder.Create();
        
        // Assert
        Assert.NotNull(builder);
        Assert.Empty(builder);
    }

    [Fact]
    public void AddServiceName_AddsCorrectTag()
    {
        // Act
        var builder = TagBuilder.Create().AddServiceName("test-service");
        
        // Assert
        Assert.Single(builder);
        var tag = builder.First();
        Assert.Equal(TagKeys.ServiceName, tag.Key);
        Assert.Equal("test-service", tag.Value);
    }

    [Fact]
    public void AddServiceVersion_AddsCorrectTag()
    {
        // Act
        var builder = TagBuilder.Create().AddServiceVersion("1.2.3");
        
        // Assert
        Assert.Single(builder);
        var tag = builder.First();
        Assert.Equal(TagKeys.ServiceVersion, tag.Key);
        Assert.Equal("1.2.3", tag.Value);
    }

    [Fact]
    public void AddServiceInstance_AddsCorrectTag()
    {
        // Act
        var builder = TagBuilder.Create().AddServiceInstance("instance-123");
        
        // Assert
        Assert.Single(builder);
        var tag = builder.First();
        Assert.Equal(TagKeys.ServiceInstance, tag.Key);
        Assert.Equal("instance-123", tag.Value);
    }

    [Fact]
    public void AddHttpMethod_AddsCorrectTag()
    {
        // Act
        var builder = TagBuilder.Create().AddHttpMethod("POST");
        
        // Assert
        Assert.Single(builder);
        var tag = builder.First();
        Assert.Equal(TagKeys.HttpMethod, tag.Key);
        Assert.Equal("POST", tag.Value);
    }

    [Fact]
    public void AddHttpUrl_AddsCorrectTag()
    {
        // Act
        var builder = TagBuilder.Create().AddHttpUrl("https://api.example.com/orders");
        
        // Assert
        Assert.Single(builder);
        var tag = builder.First();
        Assert.Equal(TagKeys.HttpUrl, tag.Key);
        Assert.Equal("https://api.example.com/orders", tag.Value);
    }

    [Fact]
    public void AddHttpStatusCode_AddsCorrectTag()
    {
        // Act
        var builder = TagBuilder.Create().AddHttpStatusCode(201);
        
        // Assert
        Assert.Single(builder);
        var tag = builder.First();
        Assert.Equal(TagKeys.HttpStatusCode, tag.Key);
        Assert.Equal(201, tag.Value);
    }

    [Fact]
    public void AddHttpUserAgent_AddsCorrectTag()
    {
        // Act
        var builder = TagBuilder.Create().AddHttpUserAgent("Mozilla/5.0");
        
        // Assert
        Assert.Single(builder);
        var tag = builder.First();
        Assert.Equal(TagKeys.HttpUserAgent, tag.Key);
        Assert.Equal("Mozilla/5.0", tag.Value);
    }

    [Fact]
    public void AddDbSystem_AddsCorrectTag()
    {
        // Act
        var builder = TagBuilder.Create().AddDbSystem("postgresql");
        
        // Assert
        Assert.Single(builder);
        var tag = builder.First();
        Assert.Equal(TagKeys.DbSystem, tag.Key);
        Assert.Equal("postgresql", tag.Value);
    }

    [Fact]
    public void AddDbName_AddsCorrectTag()
    {
        // Act
        var builder = TagBuilder.Create().AddDbName("orders_db");
        
        // Assert
        Assert.Single(builder);
        var tag = builder.First();
        Assert.Equal(TagKeys.DbName, tag.Key);
        Assert.Equal("orders_db", tag.Value);
    }

    [Fact]
    public void AddDbOperation_AddsCorrectTag()
    {
        // Act
        var builder = TagBuilder.Create().AddDbOperation("SELECT");
        
        // Assert
        Assert.Single(builder);
        var tag = builder.First();
        Assert.Equal(TagKeys.DbOperation, tag.Key);
        Assert.Equal("SELECT", tag.Value);
    }

    [Fact]
    public void AddDbStatement_AddsCorrectTag()
    {
        // Act
        var builder = TagBuilder.Create().AddDbStatement("SELECT * FROM orders WHERE id = ?");
        
        // Assert
        Assert.Single(builder);
        var tag = builder.First();
        Assert.Equal(TagKeys.DbStatement, tag.Key);
        Assert.Equal("SELECT * FROM orders WHERE id = ?", tag.Value);
    }

    [Fact]
    public void AddMessagingSystem_AddsCorrectTag()
    {
        // Act
        var builder = TagBuilder.Create().AddMessagingSystem("kafka");
        
        // Assert
        Assert.Single(builder);
        var tag = builder.First();
        Assert.Equal(TagKeys.MessagingSystem, tag.Key);
        Assert.Equal("kafka", tag.Value);
    }

    [Fact]
    public void AddMessagingDestination_AddsCorrectTag()
    {
        // Act
        var builder = TagBuilder.Create().AddMessagingDestination("order-events");
        
        // Assert
        Assert.Single(builder);
        var tag = builder.First();
        Assert.Equal(TagKeys.MessagingDestination, tag.Key);
        Assert.Equal("order-events", tag.Value);
    }

    [Fact]
    public void AddMessagingOperation_AddsCorrectTag()
    {
        // Act
        var builder = TagBuilder.Create().AddMessagingOperation("publish");
        
        // Assert
        Assert.Single(builder);
        var tag = builder.First();
        Assert.Equal(TagKeys.MessagingOperation, tag.Key);
        Assert.Equal("publish", tag.Value);
    }

    [Fact]
    public void AddMessagingKafkaTopic_AddsCorrectTag()
    {
        // Act
        var builder = TagBuilder.Create().AddMessagingKafkaTopic("order-created");
        
        // Assert
        Assert.Single(builder);
        var tag = builder.First();
        Assert.Equal(TagKeys.MessagingKafkaTopic, tag.Key);
        Assert.Equal("order-created", tag.Value);
    }

    [Fact]
    public void AddMessagingKafkaPartition_AddsCorrectTag()
    {
        // Act
        var builder = TagBuilder.Create().AddMessagingKafkaPartition(3);
        
        // Assert
        Assert.Single(builder);
        var tag = builder.First();
        Assert.Equal(TagKeys.MessagingKafkaPartition, tag.Key);
        Assert.Equal(3, tag.Value);
    }

    [Fact]
    public void AddMessagingKafkaOffset_AddsCorrectTag()
    {
        // Act
        var builder = TagBuilder.Create().AddMessagingKafkaOffset(12345L);
        
        // Assert
        Assert.Single(builder);
        var tag = builder.First();
        Assert.Equal(TagKeys.MessagingKafkaOffset, tag.Key);
        Assert.Equal(12345L, tag.Value);
    }

    [Fact]
    public void AddUserId_AddsCorrectTag()
    {
        // Act
        var builder = TagBuilder.Create().AddUserId("user-123");
        
        // Assert
        Assert.Single(builder);
        var tag = builder.First();
        Assert.Equal(TagKeys.UserId, tag.Key);
        Assert.Equal("user-123", tag.Value);
    }

    [Fact]
    public void AddUserEmail_AddsCorrectTag()
    {
        // Act
        var builder = TagBuilder.Create().AddUserEmail("user@example.com");
        
        // Assert
        Assert.Single(builder);
        var tag = builder.First();
        Assert.Equal(TagKeys.UserEmail, tag.Key);
        Assert.Equal("user@example.com", tag.Value);
    }

    [Fact]
    public void AddUserName_AddsCorrectTag()
    {
        // Act
        var builder = TagBuilder.Create().AddUserName("john.doe");
        
        // Assert
        Assert.Single(builder);
        var tag = builder.First();
        Assert.Equal(TagKeys.UserName, tag.Key);
        Assert.Equal("john.doe", tag.Value);
    }

    [Fact]
    public void AddErrorType_AddsCorrectTag()
    {
        // Act
        var builder = TagBuilder.Create().AddErrorType("ValidationException");
        
        // Assert
        Assert.Single(builder);
        var tag = builder.First();
        Assert.Equal(TagKeys.ErrorType, tag.Key);
        Assert.Equal("ValidationException", tag.Value);
    }

    [Fact]
    public void AddErrorMessage_AddsCorrectTag()
    {
        // Act
        var builder = TagBuilder.Create().AddErrorMessage("Invalid order data");
        
        // Assert
        Assert.Single(builder);
        var tag = builder.First();
        Assert.Equal(TagKeys.ErrorMessage, tag.Key);
        Assert.Equal("Invalid order data", tag.Value);
    }

    [Fact]
    public void AddCorrelationId_AddsCorrectTag()
    {
        // Act
        var builder = TagBuilder.Create().AddCorrelationId("corr-123");
        
        // Assert
        Assert.Single(builder);
        var tag = builder.First();
        Assert.Equal(TagKeys.CorrelationId, tag.Key);
        Assert.Equal("corr-123", tag.Value);
    }

    [Fact]
    public void AddTenantId_AddsCorrectTag()
    {
        // Act
        var builder = TagBuilder.Create().AddTenantId("tenant-456");
        
        // Assert
        Assert.Single(builder);
        var tag = builder.First();
        Assert.Equal(TagKeys.TenantId, tag.Key);
        Assert.Equal("tenant-456", tag.Value);
    }

    [Fact]
    public void AddRequestId_AddsCorrectTag()
    {
        // Act
        var builder = TagBuilder.Create().AddRequestId("req-789");
        
        // Assert
        Assert.Single(builder);
        var tag = builder.First();
        Assert.Equal(TagKeys.RequestId, tag.Key);
        Assert.Equal("req-789", tag.Value);
    }

    [Fact]
    public void AddTag_AddsCustomTag()
    {
        // Act
        var builder = TagBuilder.Create().AddTag("custom.field", "custom-value");
        
        // Assert
        Assert.Single(builder);
        var tag = builder.First();
        Assert.Equal("custom.field", tag.Key);
        Assert.Equal("custom-value", tag.Value);
    }

    [Fact]
    public void FluentAPI_WorksCorrectly()
    {
        // Act
        var builder = TagBuilder.Create()
            .AddServiceName("test-service")
            .AddUserId("user-123")
            .AddHttpMethod("POST")
            .AddCorrelationId("corr-456");
        
        // Assert
        Assert.Equal(4, builder.Count());
        
        var tagDict = builder.ToDictionary(t => t.Key, t => t.Value);
        Assert.Equal("test-service", tagDict[TagKeys.ServiceName]);
        Assert.Equal("user-123", tagDict[TagKeys.UserId]);
        Assert.Equal("POST", tagDict[TagKeys.HttpMethod]);
        Assert.Equal("corr-456", tagDict[TagKeys.CorrelationId]);
    }

    [Fact]
    public void ToTagList_ReturnsCorrectTagList()
    {
        // Arrange
        var builder = TagBuilder.Create()
            .AddServiceName("test-service")
            .AddUserId("user-123");
        
        // Act
        var tagList = builder.ToTagList();
        
        // Assert
        Assert.Equal(2, tagList.Count);
        
        var tagDict = tagList.ToDictionary(t => t.Key, t => t.Value);
        Assert.Equal("test-service", tagDict[TagKeys.ServiceName]);
        Assert.Equal("user-123", tagDict[TagKeys.UserId]);
    }

    [Fact]
    public void ToArray_ReturnsCorrectArray()
    {
        // Arrange
        var builder = TagBuilder.Create()
            .AddServiceName("test-service")
            .AddUserId("user-123");
        
        // Act
        var array = builder.ToArray();
        
        // Assert
        Assert.Equal(2, array.Length);
        
        var tagDict = array.ToDictionary(t => t.Key, t => t.Value);
        Assert.Equal("test-service", tagDict[TagKeys.ServiceName]);
        Assert.Equal("user-123", tagDict[TagKeys.UserId]);
    }

    [Fact]
    public void ToSpan_ReturnsCorrectSpan()
    {
        // Arrange
        var builder = TagBuilder.Create()
            .AddServiceName("test-service")
            .AddUserId("user-123");
        
        // Act
        var span = builder.ToSpan();
        
        // Assert
        Assert.Equal(2, span.Length);
        
        var tagDict = span.ToArray().ToDictionary(t => t.Key, t => t.Value);
        Assert.Equal("test-service", tagDict[TagKeys.ServiceName]);
        Assert.Equal("user-123", tagDict[TagKeys.UserId]);
    }

    [Fact]
    public void MultipleTags_AllAddedCorrectly()
    {
        // Act
        var builder = TagBuilder.Create()
            .AddServiceName("test-service")
            .AddUserId("user-123")
            .AddHttpMethod("POST")
            .AddHttpStatusCode(201)
            .AddDbSystem("postgresql")
            .AddMessagingSystem("kafka");
        
        // Assert
        Assert.Equal(6, builder.Count());
        
        var tagDict = builder.ToDictionary(t => t.Key, t => t.Value);
        Assert.Equal("test-service", tagDict[TagKeys.ServiceName]);
        Assert.Equal("user-123", tagDict[TagKeys.UserId]);
        Assert.Equal("POST", tagDict[TagKeys.HttpMethod]);
        Assert.Equal(201, tagDict[TagKeys.HttpStatusCode]);
        Assert.Equal("postgresql", tagDict[TagKeys.DbSystem]);
        Assert.Equal("kafka", tagDict[TagKeys.MessagingSystem]);
    }
}