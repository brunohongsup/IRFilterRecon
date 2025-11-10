using System.Security.Permissions;
using WinFormsApp1.Models;

namespace WinFormsApp1;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        var workersManager = WorkersManager.Instance;
        var mainForm = new Form1();
        workersManager.RegisterWorker(EForeWorkerKey.LoadStage, new CFW_LoadStage(EForeWorkerKey.LoadStage, mainForm));
       
        Application.Run(mainForm);
    }
}