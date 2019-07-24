/* 
//  Desert Software Solutions, Inc 2019
//  Basic Cron Service example

// http://www.desertsoftware.com

// THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, 
// BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY, NON-INFRINGEMENT AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED.  

// IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT 
// OF THE USE OF THIS SOFTWARE, WHETHER OR NOT SUCH DAMAGES WERE FORESEEABLE AND EVEN IF THE AUTHOR IS ADVISED 
// OF THE POSSIBILITY OF SUCH DAMAGES. 
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Gel;
using DesertSoftware.Solutions.Service;
using DesertSoftware.Solutions.Service.Cron;

namespace Solutions.Service.Examples
{
    /// <summary>
    /// Basic service illustrating how to construct a program to run as a windows service.
    /// 
    /// To support command line /i[nstall] and /u[ninstall] switches, mark the BasicProgram 
    /// class as public and inherit from the Installable class 
    /// </summary>
    /// <seealso cref="Installable" />
    public class BasicProgram : Installable
    {
        class BasicService
        {
            CronDaemon<string> scheduler;

            // provide default configuration settings
            private string DefaultSettings() {
                return "<service frequency='every 10 seconds' runOnStartup='true' />";
            }

            /// <summary>
            /// Starts the service with the specified arguments.
            /// </summary>
            /// <param name="args">The arguments.</param>
            public void Start(string[] args) {
                ILogger logger = Logger.Log("service");

                logger.Info("Starting");

                // start up the scheduler
                this.scheduler = CronDaemon.Start<string>(Execute);

                //// determine the config file location and interval
                string configFile = (args.Length > 0
                    ? args[0] ?? ConfigurationManager.AppSettings["setting:ConfigFile"]     
                    : ConfigurationManager.AppSettings["setting:ConfigFile"]) ?? "default://config.xml";

                // get the scheduler log
                Logger.Log("scheduler");

                // load the settings
                Settings settings = Settings.Load(configFile, DefaultSettings);

                // emit the config file and scheduler frequenmcy we are running with 
                logger.Info("Configfile: {0}", configFile);
                logger.Info("Frequency: {0}", settings.Frequency);

                //// add job definition and run it now
                this.scheduler.Add(configFile, settings.Frequency, int.MaxValue, settings.RunOnStartup);
                logger.Info("Started");
            }

            /// <summary>
            /// Stops this instance.
            /// </summary>
            public void Stop() {
                Logger.Log("service").Info("Stopping");
                this.scheduler.Stop();
                Logger.Log("service").Info("Stopped");
            }

            private void Execute(string config) {
                ILogger logger = Logger.Log("scheduler");

                logger.Info("Service is executing from scheduler ...");

                // simulate some activity. Sleep for 1 second
                System.Threading.Thread.Sleep(1000);

                logger.Info("Service execution completed");
            }

            /// <summary>
            /// Defines the entry point of the application.
            /// </summary>
            /// <param name="args">The arguments.</param>
            static void Main(string[] args) {
                ServiceFactory.Run<BasicService>(
                    args, x => {
                        x.DisplayName = "Desert Software Basic Service";    // the name that is displayed in the service control manager
                        x.ServiceName = "BasicService";                     // the name by which this service is started and stopped (eg. net start servicename)

                        // the description of the service as diplayed in the service control manager
                        x.Description = @"Desert Software Basic Service example.";

                        x.Service(s => {
                            s.ConstructUsing(svc => new BasicService());       // allows for a sequence of steps to be executed in order to instantiate the service class
                            s.OnStart(svc => svc.Start(args));                 // what sequence of steps should be executed on a start request
                            s.OnStop(svc => svc.Stop());                       // what sequence of steps should be executed on a stop request
                        });
                    });
            }
        }
    }
}


