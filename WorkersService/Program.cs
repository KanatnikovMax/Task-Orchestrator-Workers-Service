using Serilog;
using WorkersService.IoC;
using WorkersService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureSerilog();

builder
    .AddWorkerProgressNotifier()
    .AddTaskWorker();

var app = builder.Build();

app.UseSerilogRequestLogging();

// Для демонстрации
app.MapGet("/", async () =>
{
    var scope = app.Services.CreateScope();

    var worker = scope.ServiceProvider.GetRequiredService<TaskWorker>();

    await worker.ProcessTaskAsync(Guid.NewGuid().ToString());
});

app.Run();