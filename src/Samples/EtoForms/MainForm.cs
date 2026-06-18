// Copyright Bastian Eicher
// Licensed under the MIT License

using Eto.Drawing;
using Eto.Forms;
using NanoByte.Common.EtoControls;
using NanoByte.Common.Tasks;

namespace NanoByte.Common.Samples.EtoForms;

public sealed class MainForm : Form
{
    public MainForm()
    {
        Title = "NanoByte.Common.EtoForms Sample";
        ClientSize = new Size(420, 460);

        var progressBar = new TaskProgressBar();
        var taskControl = new TaskControl {TaskName = "Background work"};

        var messageButton = new Button {Text = "Message boxes"};
        messageButton.Click += delegate
        {
            if (Msg.OkCancel(this, "Continue?", MsgSeverity.Info))
            {
                switch (Msg.YesNoCancel(this, "Good?", MsgSeverity.Info))
                {
                    case DialogResult.Yes:
                        Msg.Inform(this, "Good", MsgSeverity.Info);
                        break;
                    case DialogResult.No:
                        Msg.Inform(this, "Not good", MsgSeverity.Warn);
                        break;
                }
            }
        };

        var outputButton = new Button {Text = "Output"};
        outputButton.Click += delegate { OutputBox.Show(this, "Test", "Test message\nSecond line"); };

        var gridButton = new Button {Text = "Grid"};
        gridButton.Click += delegate { OutputGridBox.Show(this, "Test", ["Test 1", "Test 2", "Test 3"]); };

        var treeButton = new Button {Text = "Tree"};
        treeButton.Click += delegate
        {
            var data = new NamedCollection<TreeEntry>
            {
                new() {Name = "Fruit|Apple"},
                new() {Name = "Fruit|Banana"},
                new() {Name = "Vegetable|Carrot"},
                new() {Name = "Vegetable|Potato", HighlightColor = System.Drawing.Color.Green}
            };
            OutputTreeBox.Show(this, "Test", data);
        };

        var errorButton = new Button {Text = "Error"};
        errorButton.Click += delegate
        {
            var formattedText = new FormattedTextBuilder();
            formattedText.AppendLine(LogSeverity.Info, "Some info");
            formattedText.AppendLine(LogSeverity.Error, "Something went wrong");
            ErrorBox.Show(this, new Exception("Sample error\nWith some details"), formattedText);
        };

        var inputButton = new Button {Text = "Input"};
        inputButton.Click += delegate
        {
            string? text = InputBox.Show(this, "Input", "Enter something:", "default");
            if (text != null) Msg.Inform(this, $"You entered: {text}", MsgSeverity.Info);
        };

        var taskButton = new Button {Text = "Run task in dialog"};
        taskButton.Click += delegate
        {
            using var handler = new EtoDialogTaskHandler(this);
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

        Content = new StackLayout
        {
            Padding = 10,
            Spacing = 8,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            Items =
            {
                progressBar,
                taskControl,
                messageButton,
                outputButton,
                gridButton,
                treeButton,
                errorButton,
                inputButton,
                taskButton
            }
        };

        Shown += async delegate
        {
            for (int i = 0; i <= 10; i++)
            {
                progressBar.Report(new TaskSnapshot(TaskState.Data, UnitsProcessed: i, UnitsTotal: 10));
                taskControl.Report(new TaskSnapshot(TaskState.Data, UnitsProcessed: i, UnitsTotal: 10));
                await Task.Delay(250);
            }
            progressBar.Report(new TaskSnapshot(TaskState.Complete));
            taskControl.Report(new TaskSnapshot(TaskState.Complete));
        };
    }

    private sealed class TreeEntry : INamed, IHighlightColor
    {
        public string Name { get; set; } = "";
        public System.Drawing.Color HighlightColor { get; init; }
    }
}
