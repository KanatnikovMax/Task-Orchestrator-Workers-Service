using WorkersService.Db.Repositories;
using WorkersService.Options;
using WorkersService.Services;

namespace WorkersService.IoC;

public static class ServicesConfigurator
{
    public static WebApplicationBuilder AddWorkerProgressNotifier(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<WorkerProgressNotifierOptions>(
            builder.Configuration.GetSection("WorkerProgressNotifier"));
        
        builder.Services.AddScoped<WorkerProgressNotifier>();
        
        return builder;
    }

    public static WebApplicationBuilder AddTaskWorker(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<TaskWorker>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ITasksRepository, TasksRepository>();
        
        return builder;
    }

    public static WebApplicationBuilder AddKafkaConsumer(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<KafkaOptions>(
            builder.Configuration.GetSection("Kafka"));
        
        builder.Services.AddHostedService<KafkaConsumerService>();
        
        return builder;
    }
}