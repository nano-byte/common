// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common;
using NanoByte.Common.Tasks;

using var handler = new AnsiCliTaskHandler();
if (args.Contains("--verbose")) handler.Verbosity = Verbosity.Verbose;
if (args.Contains("--debug")) handler.Verbosity = Verbosity.Debug;
if (args.Contains("--batch")) handler.Verbosity = Verbosity.Batch;

Log.Debug("debug");
Log.Info("info");
Log.Warn("warn");
Log.Error("error");

handler.RunTask(new ActionTask("Waiting", () => Thread.Sleep(2000)));

Parallel.For(2, 5, x =>
{
    // ReSharper disable once AccessToDisposedClosure
    handler.RunTask(new PercentageTask("Doing stuff", callback =>
    {
        for (int i = 0; i < 100; i += x)
        {
            Thread.Sleep(250);
            callback(i);
        }
    }));
});

handler.Output("Table", new Data[]
{
    new ("a", "b"),
    new ("x", "y")
});

// ReSharper disable once CheckNamespace
record Data(string Column1, string Column2);
