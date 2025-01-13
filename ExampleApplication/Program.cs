using System;
using System.Windows.Forms;

namespace SharpCdda.ExampleApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Engine.Startup();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());

            Engine.Shutdown();
        }
    }
}
