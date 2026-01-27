using Microsoft.AspNetCore.Server.Kestrel.Core;
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
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5208, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
});
var app = builder.Build();

app.UseSerilogRequestLogging();
app.ConfigurePostgres();
app.MapGrpcService<TaskWorkerGrpcService>();
app.Run();