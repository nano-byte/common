// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Samples.WinForms;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
}

