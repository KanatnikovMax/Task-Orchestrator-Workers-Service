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
app.MapGet("/{id}", async (string id = "1") =>
{
    var scope = app.Services.CreateScope();

    var worker = scope.ServiceProvider.GetRequiredService<TaskWorker>();

    await worker.ProcessTaskAsync(id);
});

app.Run();