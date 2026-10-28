namespace DynamicCsvFileGeneration.DownloadQueue
{
    public interface IBackgroundTaskQueue
    {
        ValueTask<Func<IServiceProvider, CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
        ValueTask QueueBackgroundWorkItemAsync(Func<IServiceProvider, CancellationToken, ValueTask> workItem);
    }
}