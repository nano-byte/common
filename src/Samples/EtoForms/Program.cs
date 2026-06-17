// Copyright Bastian Eicher
// Licensed under the MIT License

using Eto;
using Eto.Forms;

namespace NanoByte.Common.Samples.EtoForms;

public static class Program
{
    [STAThread]
    public static void Main()
        => new Application(Platform.Detect).Run(new MainForm());
}
