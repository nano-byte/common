// Copyright Bastian Eicher
// Licensed under the MIT License

using Eto;
using NanoByte.Common.Tasks;
using NanoByte.Common.Threading;

namespace NanoByte.Common.Samples.EtoForms.Worker;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        if (TryGetPlatform() is { } platform)
        {
            EtoApp.Run(platform, () =>
            {
                using var handler = new EtoDialogTaskHandler();
                Workload(handler);
            });
        }
        else
        {
            using var handler = new CliTaskHandler();
            Workload(handler);
        }
    }

    private static Platform? TryGetPlatform()
    {
        try
        {
            return Platform.Detect;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    private static void Workload(ITaskHandler handler)
    {
        handler.RunTask(new ActionTask("Sample task", cancellationToken =>
        {
            for (int i = 0; i < 30; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Thread.Sleep(100);
            }
        }));
    }
}
