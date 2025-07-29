namespace ObservabilityHelper;


public static class TagKeys
{
    // Service attributes
    public const string ServiceName = "service.name";
    public const string ServiceVersion = "service.version";
    public const string ServiceInstance = "service.instance.id";

    // HTTP attributes
    public const string HttpMethod = "http.method";
    public const string HttpUrl = "http.url";
    public const string HttpStatusCode = "http.status_code";
    public const string HttpUserAgent = "http.user_agent";

    // Database attributes
    public const string DbSystem = "db.system";
    public const string DbName = "db.name";
    public const string DbOperation = "db.operation";
    public const string DbStatement = "db.statement";

    // Messaging attributes (for Kafka)
    public const string MessagingSystem = "messaging.system";
    public const string MessagingDestination = "messaging.destination";
    public const string MessagingOperation = "messaging.operation";
    public const string MessagingKafkaTopic = "messaging.kafka.topic";
    public const string MessagingKafkaPartition = "messaging.kafka.partition";
    public const string MessagingKafkaOffset = "messaging.kafka.offset";

    // User attributes
    public const string UserId = "user.id";
    public const string UserEmail = "user.email";
    public const string UserName = "user.name";

    // Error attributes
    public const string ErrorType = "error.type";
    public const string ErrorMessage = "error.message";

    // Business attributes
    public const string CorrelationId = "correlation.id";
    public const string TenantId = "tenant.id";
    public const string RequestId = "request.id";
}