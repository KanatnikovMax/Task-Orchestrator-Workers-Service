using Serilog;
using WorkersService.Grpc;
using WorkersService.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureSerilog();
builder.Services.AddPostgres(builder.Configuration);
builder
    .AddRepositories()
    .AddKafkaConsumer()
    .AddWorkerProgressNotifier()
    .AddTaskWorker();
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
});
var app = builder.Build();

app.UseSerilogRequestLogging();
app.ConfigurePostgres();
app.MapGrpcService<TaskWorkerGrpcService>();
app.MapGet("/check", () => "healthy");
app.Run();