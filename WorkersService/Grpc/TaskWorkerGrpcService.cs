using Grpc.Core;
using WorkersService.Services;

namespace WorkersService.Grpc;

public class TaskWorkerGrpcService(TaskWorker worker) : TaskWorkerService.TaskWorkerServiceBase
{
    public override async Task<ProcessTaskResponse> ProcessTask(
        ProcessTaskRequest request,
        ServerCallContext context)
    {
        await worker.ProcessTaskAsync(request.TaskId, context.CancellationToken);

        return new ProcessTaskResponse
        {
            Accepted = true
        };
    }
}