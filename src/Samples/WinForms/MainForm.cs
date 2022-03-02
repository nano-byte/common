// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Controls;
using NanoByte.Common.Tasks;

namespace NanoByte.Common.Samples.WinForms;

public class MainForm : Form
{
    public MainForm()
    {
        var progressBar1 = new TaskProgressBar {Location = new(10, 10)};
        var progressBar2 = new TaskProgressBar {Location = new(10, 60)};

        var outputButton = new Button {Text = "Output", Location = new(10, 100)};
        outputButton.Click += delegate
        {
            OutputBox.Show(this, "Test", "Test message");
        };

        var outputGridButton = new Button {Text = "Grid", Location = new(10, 140)};
        outputGridButton.Click += delegate
        {
            OutputGridBox.Show(this, "Test", new [] {"Test 1", "Test 2"});
        };

        Controls.AddRange(new Control[] {progressBar1, progressBar2, outputButton, outputGridButton});

        Shown += async delegate
        {
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(250);
                progressBar1.Report(new TaskSnapshot(TaskState.Data, unitsProcessed: i, unitsTotal: 10));
            }
            progressBar1.Report(new TaskSnapshot(TaskState.Complete));
        };

        Shown += async delegate
        {
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(500);
                progressBar2.Report(new TaskSnapshot(TaskState.Data, unitsProcessed: i, unitsTotal: 10));
            }
            progressBar2.Report(new TaskSnapshot(TaskState.Complete));
        };
    }
}
