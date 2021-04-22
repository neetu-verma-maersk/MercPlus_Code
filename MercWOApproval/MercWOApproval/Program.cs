using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MercWOAppRej
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new MercWoApproval() 
            };
            ServiceBase.Run(ServicesToRun);

          //  -- for debugging --
            //MercWoApproval service = new MercWoApproval();

            //Type service1Type = typeof(MercWoApproval);

            //MethodInfo onStart = service1Type.GetMethod("OnStart", BindingFlags.NonPublic | BindingFlags.Instance); //retrieve the OnStart method so it can be called from here

            //onStart.Invoke(service, new object[] { null });
        }
    }
}
