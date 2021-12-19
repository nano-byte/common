// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Controls;
using NanoByte.Common.Tasks;

namespace NanoByte.Common.Samples.WinForms
{
    public class MainForm : Form
    {
        private readonly TaskProgressBar
            _progressBar1 = new() {Location = new(10, 10)},
            _progressBar2 = new() {Location = new(10, 60)};

        public MainForm()
        {
            Controls.Add(_progressBar1);
            Controls.Add(_progressBar2);

            Shown += async delegate
            {
                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(250);
                    _progressBar1.Report(new TaskSnapshot(TaskState.Data, unitsProcessed: i, unitsTotal: 10));
                }
                _progressBar1.Report(new TaskSnapshot(TaskState.Complete));
            };

            Shown += async delegate
            {
                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(500);
                    _progressBar2.Report(new TaskSnapshot(TaskState.Data, unitsProcessed: i, unitsTotal: 10));
                }
                _progressBar2.Report(new TaskSnapshot(TaskState.Complete));
            };
        }
    }
}

