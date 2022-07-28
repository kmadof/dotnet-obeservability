using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Custom Metrics to count requests for each endpoint and the method
var counter = Metrics.CreateCounter("catalogapi_path_counter", "Counts requests to the Catalog API endpoints", new CounterConfiguration
{
    LabelNames = new[] { "method", "endpoint" }
});
app.Use((context, next) =>
{
    counter.WithLabels(context.Request.Method, context.Request.Path).Inc();
    return next();
});

// Use the Prometheus middleware
app.UseMetricServer();
app.UseHttpMetrics();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
