# Observability Helper

Idea to streamline some logic for newer devs, not sure if I love it but wanted to build it out before I tried different options.  Meant to be paired with open telemetry operator auto-instrumentation - need to specify custom trace/meter sources in config.

```csharp
// In your Program.cs or startup code
ObservabilityConfig.Initialize("my-service", "1.0.0");
```

```csharp
using ObservabilityHelper;

public class OrderService
{
    public async Task<Order> ProcessOrderAsync(string orderId)
    {
        return await ActivityHelper.TraceAsync("process-order", async (activity) =>
        {
            // Your business logic here
            var order = await GetOrderAsync(orderId);
            await ValidateOrderAsync(order);
            await SaveOrderAsync(order);
            
            return order;
        });
    }
    
    public Order ProcessOrder(string orderId)
    {
        return ActivityHelper.Trace("process-order", (activity) =>
        {
            // Synchronous business logic
            var order = GetOrder(orderId);
            ValidateOrder(order);
            SaveOrder(order);
            
            return order;
        });
    }
}
```

```csharp
public async Task ProcessOrderAsync(string orderId, string userId)
{
    await ActivityHelper.TraceAsync("process-order", async (activity) =>
    {
        // Add tags using extension methods
        activity?.AddServiceName("order-service");
        activity?.AddUserId(userId);
        activity?.AddCorrelationId(orderId);
        activity?.AddHttpMethod("POST");
        
        // Add multiple tags at once
        var tags = new[]
        {
            new KeyValuePair<string, object?>("order.id", orderId),
            new KeyValuePair<string, object?>("order.type", "premium")
        };
        activity?.AddTags(tags);
        
        await ProcessOrderLogic(orderId);
    });
}
```


```csharp
// Client activity (outgoing HTTP call)
await ActivityHelper.TraceAsync("http-call", async (activity) =>
{
    activity?.AddHttpMethod("GET");
    activity?.AddHttpUrl("https://api.example.com/orders");
    
    return await httpClient.GetAsync("https://api.example.com/orders");
}, ActivityKind.Client);

// Server activity (incoming request)
await ActivityHelper.TraceAsync("handle-request", async (activity) =>
{
    activity?.AddHttpMethod("POST");
    activity?.AddHttpStatusCode(200);
    
    return await HandleRequest();
}, ActivityKind.Server);
```


```csharp
public class OrderService
{
    private readonly Counter<long> _orderCounter;
    private readonly Counter<long> _errorCounter;
    
    public OrderService()
    {
        _orderCounter = MeterHelper.CreateCounter<long>("orders_processed", "orders", "Number of orders processed");
        _errorCounter = MeterHelper.CreateCounter<long>("orders_errors", "errors", "Number of order processing errors");
    }
    
    public async Task ProcessOrderAsync(string orderId, string userId)
    {
        try
        {
            await ProcessOrderLogic(orderId);
            
            // Record success with tags
            var tags = TagBuilder.Create()
                .AddServiceName("order-service")
                .AddUserId(userId)
                .AddTag("status", "success");
                
            _orderCounter.RecordWithTags(1, tags);
        }
        catch (Exception ex)
        {
            // Record error with tags
            var errorTags = TagBuilder.Create()
                .AddServiceName("order-service")
                .AddUserId(userId)
                .AddErrorType(ex.GetType().Name)
                .AddErrorMessage(ex.Message);
                
            _errorCounter.RecordWithTags(1, errorTags);
            throw;
        }
    }
}
```

```csharp
public class OrderService
{
    private readonly Histogram<double> _processingTime;
    private readonly Histogram<double> _orderValue;
    
    public OrderService()
    {
        _processingTime = MeterHelper.CreateHistogram<double>("order_processing_duration", "ms", "Order processing time");
        _orderValue = MeterHelper.CreateHistogram<double>("order_value", "USD", "Order value in USD");
    }
    
    public async Task ProcessOrderAsync(Order order)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            await ProcessOrderLogic(order);
            
            stopwatch.Stop();
            
            // Record processing time
            var timingTags = TagBuilder.Create()
                .AddServiceName("order-service")
                .AddUserId(order.UserId)
                .AddTag("order.type", order.Type);
                
            _processingTime.RecordWithTags(stopwatch.Elapsed.TotalMilliseconds, timingTags);
            
            // Record order value
            var valueTags = TagBuilder.Create()
                .AddServiceName("order-service")
                .AddTag("order.currency", order.Currency)
                .AddTag("order.type", order.Type);
                
            _orderValue.RecordWithTags(order.TotalValue, valueTags);
        }
        catch (Exception)
        {
            stopwatch.Stop();
            
            // Record failed processing time
            var errorTags = TagBuilder.Create()
                .AddServiceName("order-service")
                .AddTag("status", "error");
                
            _processingTime.RecordWithTags(stopwatch.Elapsed.TotalMilliseconds, errorTags);
            throw;
        }
    }
}
```
```csharp
// Quick metrics without TagBuilder
_orderCounter.RecordWithTags(1, 
    ("service.name", "order-service"),
    ("user.id", userId),
    ("status", "success"));

_processingTime.RecordWithTags(duration,
    ("service.name", "order-service"),
    ("operation", "validate"),
    ("result", "success"));
```

```csharp
// HTTP request tags
var httpTags = TagBuilder.Create()
    .AddServiceName("api-gateway")
    .AddServiceVersion("2.1.0")
    .AddHttpMethod("POST")
    .AddHttpUrl("/api/orders")
    .AddHttpStatusCode(201)
    .AddHttpUserAgent("MyApp/1.0")
    .AddUserId("user-123")
    .AddCorrelationId("req-456");

// Database operation tags
var dbTags = TagBuilder.Create()
    .AddServiceName("order-service")
    .AddDbSystem("postgresql")
    .AddDbName("orders_db")
    .AddDbOperation("INSERT")
    .AddDbStatement("INSERT INTO orders (id, user_id) VALUES (?, ?)")
    .AddUserId("user-123");

// Kafka messaging tags
var kafkaTags = TagBuilder.Create()
    .AddServiceName("order-service")
    .AddMessagingSystem("kafka")
    .AddMessagingDestination("order-events")
    .AddMessagingOperation("publish")
    .AddMessagingKafkaTopic("order-created")
    .AddMessagingKafkaPartition(2)
    .AddMessagingKafkaOffset(12345);
```

```csharp
var tags = TagBuilder.Create()
    .AddServiceName("my-service")
    .AddUserId("user-123");

// For metrics
var tagList = tags.ToTagList();
counter.Add(1, tagList);

// For activities
var tagArray = tags.ToArray();
activity?.AddTags(tagArray);

// For other APIs that need ReadOnlySpan
var span = tags.ToSpan();
```
```csharp
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly Counter<long> _requestCounter;
    private readonly Histogram<double> _requestDuration;
    private readonly OrderService _orderService;
    
    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
        _requestCounter = MeterHelper.CreateCounter<long>("http_requests_total");
        _requestDuration = MeterHelper.CreateHistogram<double>("http_request_duration_ms");
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var correlationId = Guid.NewGuid().ToString();
        var stopwatch = Stopwatch.StartNew();
        
        return await ActivityHelper.TraceAsync("create-order", async (activity) =>
        {
            // Add activity tags
            activity?.AddServiceName("order-api");
            activity?.AddHttpMethod("POST");
            activity?.AddHttpUrl("/api/orders");
            activity?.AddUserId(request.UserId);
            activity?.AddCorrelationId(correlationId);
            
            try
            {
                var order = await _orderService.CreateOrderAsync(request);
                
                stopwatch.Stop();
                
                // Record success metrics
                var successTags = TagBuilder.Create()
                    .AddServiceName("order-api")
                    .AddHttpMethod("POST")
                    .AddHttpStatusCode(201)
                    .AddUserId(request.UserId);
                
                _requestCounter.RecordWithTags(1, successTags);
                _requestDuration.RecordWithTags(stopwatch.Elapsed.TotalMilliseconds, successTags);
                
                activity?.AddHttpStatusCode(201);
                
                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
            }
            catch (ValidationException ex)
            {
                stopwatch.Stop();
                
                // Record validation error metrics
                var errorTags = TagBuilder.Create()
                    .AddServiceName("order-api")
                    .AddHttpMethod("POST")
                    .AddHttpStatusCode(400)
                    .AddErrorType("ValidationException")
                    .AddErrorMessage(ex.Message);
                
                _requestCounter.RecordWithTags(1, errorTags);
                _requestDuration.RecordWithTags(stopwatch.Elapsed.TotalMilliseconds, errorTags);
                
                activity?.AddHttpStatusCode(400);
                
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                
                // Record server error metrics
                var errorTags = TagBuilder.Create()
                    .AddServiceName("order-api")
                    .AddHttpMethod("POST")
                    .AddHttpStatusCode(500)
                    .AddErrorType(ex.GetType().Name)
                    .AddErrorMessage(ex.Message);
                
                _requestCounter.RecordWithTags(1, errorTags);
                _requestDuration.RecordWithTags(stopwatch.Elapsed.TotalMilliseconds, errorTags);
                
                activity?.AddHttpStatusCode(500);
                
                return StatusCode(500, "Internal server error");
            }
        }, ActivityKind.Server);
    }
}
```

