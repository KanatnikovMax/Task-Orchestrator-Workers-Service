using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using WorkersService.Db.Models;
using WorkersService.Db.Repositories;
using WorkersService.Models;
using WorkersService.Options;

namespace WorkersService.Services;

public class KafkaConsumerService : BackgroundService
{
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly string _topic;
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public KafkaConsumerService(
        IOptions<KafkaOptions> options,
        ILogger<KafkaConsumerService> logger,
        IServiceProvider serviceProvider)
    {
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        var config = new ConsumerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            GroupId = options.Value.ConsumerGroup,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            EnableAutoOffsetStore = false
        };

        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        _topic = options.Value.Topic;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return ConsumeAsync(stoppingToken);
    }

    private async Task ConsumeAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_topic);
        _logger.LogInformation("Started Kafka consumer for topic {Topic}", _topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(TimeSpan.FromMilliseconds(500));
                    if (consumeResult?.Message == null)
                    {
                        continue;
                    }

                    _logger.LogInformation("Received task from Kafka. Partition: {Partition}, Offset: {Offset}",
                        consumeResult.Partition, consumeResult.Offset.Value);

                    await ProcessMessageAsync(consumeResult.Message.Value);

                    _consumer.Commit(consumeResult);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message from Kafka");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process message, offset not committed");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Kafka consumer stopping");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in Kafka consumer");
        }
        finally
        {
            _consumer.Close();
            _consumer.Dispose();
        }
    }
    
    private async Task ProcessMessageAsync(string messageJson)
    {
        try
        {
            var task = JsonSerializer.Deserialize<KafkaTaskRequest>(messageJson, _jsonSerializerOptions);

            if (task == null)
            {
                _logger.LogWarning("Failed to deserialize task message: {Message}", messageJson);
                return;
            }

            _logger.LogInformation("Processing task {TaskId}", task.TaskId);

            using var scope = _serviceProvider.CreateScope();
            var tasksRepository =  scope.ServiceProvider.GetRequiredService<ITasksRepository>();
            await tasksRepository.SaveTaskAsync(new TaskModel
            {
                TaskId = task.TaskId,
                CreatedAt = task.CreatedAt,
            });
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize Kafka message");
        }
    }
}