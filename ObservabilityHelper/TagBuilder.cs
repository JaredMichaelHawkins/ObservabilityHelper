using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ObservabilityHelper;

public class TagBuilder : IEnumerable<KeyValuePair<string, object?>>
{
    private readonly List<KeyValuePair<string, object?>> _tags = new();

    public static TagBuilder Create() => new TagBuilder();

    // Service attributes
    public TagBuilder AddServiceName(string serviceName)
    {
        _tags.Add(new KeyValuePair<string, object?>(TagKeys.ServiceName, serviceName));
        return this;
    }
    
    public TagBuilder AddServiceVersion(string version)
    {
        _tags.Add(new KeyValuePair<string, object?>(TagKeys.ServiceVersion, version));
        return this;
    }
    
    public TagBuilder AddServiceInstance(string instanceId)
    {
        _tags.Add(new KeyValuePair<string, object?>(TagKeys.ServiceInstance, instanceId));
        return this;
    }

    // HTTP attributes
    public TagBuilder AddHttpMethod(string method)
    {
        _tags.Add(new KeyValuePair<string, object?>(TagKeys.HttpMethod, method));
        return this;
    }
    
    public TagBuilder AddHttpUrl(string url)
    {
        _tags.Add(new KeyValuePair<string, object?>(TagKeys.HttpUrl, url));
        return this;
    }
    
    public TagBuilder AddHttpStatusCode(int statusCode)
    {
        _tags.Add(new KeyValuePair<string, object?>(TagKeys.HttpStatusCode, statusCode));
        return this;
    }
    
    public TagBuilder AddHttpUserAgent(string userAgent)
    {
        _tags.Add(new KeyValuePair<string, object?>(TagKeys.HttpUserAgent, userAgent));
        return this;
    }

    // Database attributes
    public TagBuilder AddDbSystem(string system)
    {
        _tags.Add(new KeyValuePair<string, object?>(TagKeys.DbSystem, system));
        return this;
    }
    
    public TagBuilder AddDbName(string name)
    {
        _tags.Add(new KeyValuePair<string, object?>(TagKeys.DbName, name));
        return this;
    }
    
    public TagBuilder AddDbOperation(string operation)
    {
        _tags.Add(new KeyValuePair<string, object?>(TagKeys.DbOperation, operation));
        return this;
    }
    
    public TagBuilder AddDbStatement(string statement)
    {
        _tags.Add(new KeyValuePair<string, object?>(TagKeys.DbStatement, statement));
        return this;
    }

    // Messaging attributes (for Kafka)
    public TagBuilder AddMessagingSystem(string system)
    {
        _tags.Add(new KeyValuePair<string, object?>(TagKeys.MessagingSystem, system));
        return this;
    }
    
    public TagBuilder AddMessagingDestination(string destination)
    {
        _tags.Add(new KeyValuePair<string, object?>(TagKeys.MessagingDestination, destination));
        return this;
    }
    
    public TagBuilder AddMessagingOperation(string operation)
    {
        _tags.Add(new KeyValuePair<string, object?>(TagKeys.MessagingOperation, operation));
        return this;
    }
    
    public TagBuilder AddMessagingKafkaTopic(string topic)
    {
        _tags.Add(new KeyValuePair<string, object?>(TagKeys.MessagingKafkaTopic, topic));
        return this;
    }
    
    public TagBuilder AddMessagingKafkaPartition(int partition)
    {
        _tags.Add(new KeyValuePair<string, object?>(TagKeys.MessagingKafkaPartition, partition));
        return this;
    }
    
    public TagBuilder AddMessagingKafkaOffset(long offset)
    {
        _tags.Add(new KeyValuePair<string, object?>(TagKeys.MessagingKafkaOffset, offset));
        return this;
    }

    // User attributes
    public TagBuilder AddUserId(string userId)
    {
        _tags.Add(new KeyValuePair<string, object?>(TagKeys.UserId, userId));
        return this;
    }
    
    public TagBuilder AddUserEmail(string email)
    {
        _tags.Add(new KeyValuePair<string, object?>(TagKeys.UserEmail, email));
        return this;
    }
    
    public TagBuilder AddUserName(string username)
    {
        _tags.Add(new KeyValuePair<string, object?>(TagKeys.UserName, username));
        return this;
    }

    // Error attributes
    public TagBuilder AddErrorType(string errorType)
    {
        _tags.Add(new KeyValuePair<string, object?>(TagKeys.ErrorType, errorType));
        return this;
    }
    
    public TagBuilder AddErrorMessage(string message)
    {
        _tags.Add(new KeyValuePair<string, object?>(TagKeys.ErrorMessage, message));
        return this;
    }

    // Business attributes
    public TagBuilder AddCorrelationId(string correlationId)
    {
        _tags.Add(new KeyValuePair<string, object?>(TagKeys.CorrelationId, correlationId));
        return this;
    }
    
    public TagBuilder AddTenantId(string tenantId)
    {
        _tags.Add(new KeyValuePair<string, object?>(TagKeys.TenantId, tenantId));
        return this;
    }
    
    public TagBuilder AddRequestId(string requestId)
    {
        _tags.Add(new KeyValuePair<string, object?>(TagKeys.RequestId, requestId));
        return this;
    }

    // Generic tag addition
    public TagBuilder AddTag(string key, object? value)
    {
        _tags.Add(new KeyValuePair<string, object?>(key, value));
        return this;
    }

    // IEnumerable implementation
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        return _tags.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // Convert to formats that metrics can use
    public TagList ToTagList()
    {
        var tagList = new TagList();
        foreach (var tag in _tags)
        {
            tagList.Add(tag);
        }
        return tagList;
    }

    public ReadOnlySpan<KeyValuePair<string, object?>> ToSpan()
    {
        return _tags.ToArray().AsSpan();
    }

    public KeyValuePair<string, object?>[] ToArray()
    {
        return _tags.ToArray();
    }
}