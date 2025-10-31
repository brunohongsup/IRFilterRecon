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
        var productManager = ProductManager.Instance;
        for (int idx = 0; idx < 10; idx++)
        {
            productManager.AddProduct(RandomStringGenerator.GenerateRandomString(10));
        }
        
        var workersManager = WorkersManager.Instance;
        workersManager.RegisterWorker(EForeWorkerKey.LoadStage, new CFW_LoadStage(EForeWorkerKey.LoadStage));
       
        Application.Run(new Form1());
    }
}