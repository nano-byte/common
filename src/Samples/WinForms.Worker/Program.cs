// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Tasks;

namespace NanoByte.Common.Samples.WinForms.Worker;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        using var handler = new DialogTaskHandler();
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
