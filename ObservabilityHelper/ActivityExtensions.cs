using System.Diagnostics;

namespace ObservabilityHelper;

public static class ActivityExtensions
{
    // Service attributes
    public static void AddServiceName(this Activity? activity, string serviceName)
        => activity?.SetTag(TagKeys.ServiceName, serviceName);
    
    public static void AddServiceVersion(this Activity? activity, string version)
        => activity?.SetTag(TagKeys.ServiceVersion, version);
    
    public static void AddServiceInstance(this Activity? activity, string instanceId)
        => activity?.SetTag(TagKeys.ServiceInstance, instanceId);

    // HTTP attributes
    public static void AddHttpMethod(this Activity? activity, string method)
        => activity?.SetTag(TagKeys.HttpMethod, method);
    
    public static void AddHttpUrl(this Activity? activity, string url)
        => activity?.SetTag(TagKeys.HttpUrl, url);
    
    public static void AddHttpStatusCode(this Activity? activity, int statusCode)
        => activity?.SetTag(TagKeys.HttpStatusCode, statusCode);
    
    public static void AddHttpUserAgent(this Activity? activity, string userAgent)
        => activity?.SetTag(TagKeys.HttpUserAgent, userAgent);

    // Database attributes
    public static void AddDbSystem(this Activity? activity, string system)
        => activity?.SetTag(TagKeys.DbSystem, system);
    
    public static void AddDbName(this Activity? activity, string name)
        => activity?.SetTag(TagKeys.DbName, name);
    
    public static void AddDbOperation(this Activity? activity, string operation)
        => activity?.SetTag(TagKeys.DbOperation, operation);
    
    public static void AddDbStatement(this Activity? activity, string statement)
        => activity?.SetTag(TagKeys.DbStatement, statement);

    // Messaging attributes (for Kafka)
    public static void AddMessagingSystem(this Activity? activity, string system)
        => activity?.SetTag(TagKeys.MessagingSystem, system);
    
    public static void AddMessagingDestination(this Activity? activity, string destination)
        => activity?.SetTag(TagKeys.MessagingDestination, destination);
    
    public static void AddMessagingOperation(this Activity? activity, string operation)
        => activity?.SetTag(TagKeys.MessagingOperation, operation);
    
    public static void AddMessagingKafkaTopic(this Activity? activity, string topic)
        => activity?.SetTag(TagKeys.MessagingKafkaTopic, topic);
    
    public static void AddMessagingKafkaPartition(this Activity? activity, int partition)
        => activity?.SetTag(TagKeys.MessagingKafkaPartition, partition);
    
    public static void AddMessagingKafkaOffset(this Activity? activity, long offset)
        => activity?.SetTag(TagKeys.MessagingKafkaOffset, offset);

    // User attributes
    public static void AddUserId(this Activity? activity, string userId)
        => activity?.SetTag(TagKeys.UserId, userId);
    
    public static void AddUserEmail(this Activity? activity, string email)
        => activity?.SetTag(TagKeys.UserEmail, email);
    
    public static void AddUserName(this Activity? activity, string username)
        => activity?.SetTag(TagKeys.UserName, username);

    // Error attributes
    public static void AddErrorType(this Activity? activity, string errorType)
        => activity?.SetTag(TagKeys.ErrorType, errorType);
    
    public static void AddErrorMessage(this Activity? activity, string message)
        => activity?.SetTag(TagKeys.ErrorMessage, message);

    // Business attributes
    public static void AddCorrelationId(this Activity? activity, string correlationId)
        => activity?.SetTag(TagKeys.CorrelationId, correlationId);
    
    public static void AddTenantId(this Activity? activity, string tenantId)
        => activity?.SetTag(TagKeys.TenantId, tenantId);
    
    public static void AddRequestId(this Activity? activity, string requestId)
        => activity?.SetTag(TagKeys.RequestId, requestId);
}