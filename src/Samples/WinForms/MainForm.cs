// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Controls;
using NanoByte.Common.Tasks;
using NanoByte.Common.Threading;

namespace NanoByte.Common.Samples.WinForms;

public sealed class MainForm : Form
{
    public MainForm()
    {
        ClientSize = new(345, 280);

        var progressBar1 = new TaskProgressBar {Location = new(10, 10)};
        var progressBar2 = new TaskProgressBar {Location = new(10, 60)};

        var outputButton = new Button {Text = "Output", Location = new(10, 100)};
        outputButton.Click += delegate
        {
            OutputBox.Show(this, "Test", "Test message");
        };

        var outputGridButton = new Button {Text = "Grid", Location = new(90, 100)};
        outputGridButton.Click += delegate
        {
            OutputGridBox.Show(this, "Test", ["Test 1", "Test 2"]);
        };

        var treeButton = new Button {Text = "Tree", Location = new(170, 100)};
        treeButton.Click += delegate
        {
            var data = new NamedCollection<TreeEntry>
            {
                new() {Name = "Fruit|Apple"},
                new() {Name = "Fruit|Banana"},
                new() {Name = "Vegetable|Carrot"},
                new() {Name = "Vegetable|Potato", HighlightColor = Color.Green}
            };
            OutputTreeBox.Show(this, "Test", data);
        };

        var errorButton = new Button {Text = "Error", Location = new(250, 100)};
        errorButton.Click += delegate
        {
            var rtf = new RtfBuilder();
            rtf.AppendPar("Blue", RtfColor.Blue);
            rtf.AppendPar("Red", RtfColor.Red);
            ErrorBox.Show(this, new Exception("Sample error"), rtf);
        };

        var dropDownButton = new Button {Text = "Drop-down", Location = new(10, 130)};
        dropDownButton.Click += delegate
        {
            new DropDownContainer
            {
                Controls =
                {
                    new Button {Text = "Button 1", Location = new(10, 10)},
                    new Button {Text = "Button 2", Location = new(10, 40)}
                }
            }.Show(dropDownButton);
        };

        var messageBoxesButton = new Button {Text = "Message boxes", Location = new(10, 160)};
        messageBoxesButton.Click += delegate
        {
            if (Msg.OkCancel(this, "Continue?", MsgSeverity.Info, "OK\nContinue this", "Cancel\nAbort this"))
            {
                switch (Msg.YesNoCancel(this, "Good?", MsgSeverity.Info, "Yeah\nThis is good", "Nope\nThis is not good"))
                {
                    case DialogResult.Yes:
                        Msg.Inform(this, "Good", MsgSeverity.Info);
                        break;
                    case DialogResult.No:
                        Msg.Inform(this, "Not good", MsgSeverity.Info);
                        break;
                }
            }
        };

        var asyncFormButton = new Button {Text = "Async form", Location = new(10, 190), Width = 150};
        asyncFormButton.Click += async delegate
        {
            using var wrapper = new AsyncFormWrapper<MainForm>(() => new());
            wrapper.Post(form => form.Show()); // Show form in the message loop
            await Task.Delay(5000); // End message loop and close form after 5s
        };

        var taskButton = new Button {Text = "Run task in dialog", Location = new(10, 220), Width = 150};
        taskButton.Click += delegate
        {
            using var handler = new DialogTaskHandler(this);
            try
            {
                handler.RunTask(new ActionTask("Sample task", cancellationToken =>
                {
                    for (int i = 0; i < 30; i++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        Thread.Sleep(100);
                    }
                }));
                Msg.Inform(this, "Task completed", MsgSeverity.Info);
            }
            catch (OperationCanceledException)
            {
                Msg.Inform(this, "Task canceled", MsgSeverity.Warn);
            }
        };

        var touchButton = new Button {Text = "Touch gestures", Location = new(10, 250), Width = 150};
        touchButton.Click += delegate
        {
            new TouchSampleForm().Show(this);
        };

        Controls.AddRange([progressBar1, progressBar2, outputButton, outputGridButton, treeButton, errorButton, dropDownButton, messageBoxesButton, asyncFormButton, taskButton, touchButton]);

        Shown += async delegate
        {
            for (int i = 0; i <= 10; i++)
            {
                await Task.Delay(250);
                progressBar1.Report(new TaskSnapshot(TaskState.Data, UnitsProcessed: i, UnitsTotal: 10));
            }
            progressBar1.Report(new TaskSnapshot(TaskState.Complete));
        };

        Shown += async delegate
        {
            for (int i = 0; i <= 10; i++)
            {
                await Task.Delay(500);
                progressBar2.Report(new TaskSnapshot(TaskState.Data, UnitsProcessed: i, UnitsTotal: 10));
            }
            progressBar2.Report(new TaskSnapshot(TaskState.Complete));
        };
    }

    private sealed class TreeEntry : INamed, IHighlightColor
    {
        public string Name { get; set; } = "";
        public Color HighlightColor { get; init; }
    }
}
