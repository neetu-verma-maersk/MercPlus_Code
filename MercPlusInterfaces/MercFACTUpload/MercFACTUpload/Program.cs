using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MercFACTUpload
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            System.Diagnostics.Debugger.Launch();
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new MercFactUploadService() 
            };
            ServiceBase.Run(ServicesToRun);
            
           /* XMLBL test = new XMLBL();
            test.StartProcessWorkOrder();*/

            ////-- for debugging --
            //MercFactUploadService service = new MercFactUploadService();

            //Type service1Type = typeof(MercFactUploadService);

            //MethodInfo onStart = service1Type.GetMethod("OnStart", BindingFlags.NonPublic | BindingFlags.Instance); //retrieve the OnStart method so it can be called from here

            //onStart.Invoke(service, new object[] { null });
        }
    }
}
