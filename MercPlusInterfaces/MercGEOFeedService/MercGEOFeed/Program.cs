using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace MercGEOFeed
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
                new MercGEOFeedService() 
            };
            ServiceBase.Run(ServicesToRun);

            //-- for debugging --
            //MercGEOFeedService service = new MercGEOFeedService();

            //Type service1Type = typeof(MercGEOFeedService);

            //MethodInfo onStart = service1Type.GetMethod("OnStart", BindingFlags.NonPublic | BindingFlags.Instance); //retrieve the OnStart method so it can be called from here

            //onStart.Invoke(service, new object[] { null });

        }
    }
}
