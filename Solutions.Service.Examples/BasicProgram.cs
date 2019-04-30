using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Gel;
using DesertSoftware.Solutions.Service.Cron;

namespace Solutions.Service.Examples
{
    class BasicProgram
    {
        class BasicService
        {
            CronDaemon<string> scheduler;
            CronDaemon<string> pollingScheduler;

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
                this.scheduler = CronDaemon.Start<string>(LoadActiveEquipment);
                this.pollingScheduler = CronDaemon.Start<string>(EquipmentPoller);

                //// determine the config file location and interval
                string configFile = (args.Length > 0
                    ? args[0] ?? ConfigurationManager.AppSettings["setting:ConfigFile"]     // C:\dmx\drhost\config.xml
                    : ConfigurationManager.AppSettings["setting:ConfigFile"]) ?? "default://config.xml";

                // HACK: Gel has a race condition issue when two or more threads access the same logger for the first time
                //       the following statement is a work around for the race condition issue
                Logger.Log("poller");
                Logger.Log("scheduler");

                Settings settings = Settings.Load(configFile, DefaultSettings);

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
                this.pollingScheduler.Stop();
                Logger.Log("service").Info("Stopped");
            }

            private void LoadActiveEquipment(string config) {
                int rqCount = 0;
                ILogger logger = Logger.Log("scheduler");

                logger.Info("Loading equipment poll requests");

                // queue up the equipment
                try {
                    //foreach (var equipment in SystemManager.GetEnabledEquipment())
                    //    if (!this.pollingList.ContainsKey(equipment.ID.ToString())) {
                    //        ++rqCount;
                    //        logger.Info("Adding poll request: {0}/{1}/{2}", equipment.CompanyName, equipment.PlantName, equipment.Name);
                    //        this.pollingList[equipment.ID.ToString()] = equipment;
                    //        this.pollingScheduler.Add(equipment.ID.ToString(), CronSchedule.Parse("Every Minute"), 0, false);
                    //    }
                } catch (Exception ex) {
                    logger.Error(ex.Message);
                    logger.Debug(ex);
                } finally {
                    logger.Info("{0} equipment poll requests loaded.", rqCount);
                }
            }

            private void PollerLogger(int severity, object message) {
                const int DEBUG = 0;
                const int INFO = 1;
                const int WARN = 2;
                const int ERROR = 3;
                const int FATAL = 4;

                ILogger logger = Logger.Log("poller");

                switch (severity) {
                    case DEBUG: logger.Debug(message); break;
                    case INFO: logger.Info(message); break;
                    case WARN: logger.Warn(message); break;
                    case ERROR: logger.Error(message); break;
                    case FATAL: logger.Fatal(message); break;
                }
            }

            private void EquipmentPoller(string requestID) {
                ILogger logger = Logger.Log("poller");
                bool allDone = true;

                logger.Info("Processing poll request: {0}", requestID);
                try {
                    //if (this.pollingList.ContainsKey(requestID)) {
                    //    Equipment equipment = this.pollingList[requestID];
                    //    logger.Info("polling {0}/{1}/{2}", equipment.CompanyName, equipment.PlantName, equipment.Name);
                    //    List<string> supportedConnectionTypes = new List<string>(new string[] { "SOE", "SOE3", "USB" });
                    //    //                        List<string> supportedConnectionTypes = new List<string>(new string[] { "SOE", "SOE3" });

                    //    // enumerate the monitors
                    //    List<Monitor> monitors = SystemManager.GetEnabledEquipmentMonitors(equipment.ID);

                    //    // find the Main monitor first. Do we have a main monitor? it's possible we don't
                    //    Monitor mainMonitor =
                    //        (from m in monitors
                    //         where m.MonitorType.Trim().Equals("Main", StringComparison.CurrentCultureIgnoreCase)
                    //         select m)
                    //        .FirstOrDefault();

                    //    // normalize all monitor connection information that are connecting via the main monitor
                    //    foreach (var monitor in monitors)
                    //        if (monitor.ConnectionType.Trim().Equals("Via Main", StringComparison.CurrentCultureIgnoreCase) && mainMonitor != null) {
                    //            monitor.ConnectionType = mainMonitor.ConnectionType;
                    //            monitor.IPAddress = mainMonitor.IPAddress;
                    //            monitor.PortNumber = mainMonitor.PortNumber;
                    //        }

                    //    // poll each monitor
                    //    foreach (var monitor in monitors)
                    //        if (supportedConnectionTypes.Contains(monitor.ConnectionType.Trim().ToUpper())) {
                    //            int recNum = 0;
                    //            DateTime pollStarted = DateTime.Now;

                    //            // build up the connection string
                    //            UriBuilder builder = new UriBuilder(string.Format("{0}://{1}:{2}/{3}",
                    //                monitor.ConnectionType.Trim(), monitor.IPAddress.Trim(), monitor.PortNumber, monitor.MonitorType.Trim()));

                    //            builder.Fragment = monitor.ModbusAddress.Trim();
                    //            logger.Info("polling monitor {0}:{1} via {2} since {3}",
                    //                monitor.MonitorType.Trim(), monitor.ModbusAddress.Trim(), builder.ToString(), monitor.DateOfLastDataDownload);

                    //            try {
                    //                using (Device device = Device.Open(builder.ToString(), PollerLogger)) {

                    //                    // TODO: we need to get the current configuration of the monitor which the device object does not 
                    //                    // currently support
                    //                    // MonitorManager.SaveMonitorConfiguration(monitor, device.Setup);

                    //                    // save the current device health
                    //                    MonitorManager.SaveMonitorAlarmStatus(monitor, device.Health);

                    //                    foreach (var reading in device.Readings(monitor.DateOfLastDataDownload)) {
                    //                        ++recNum;
                    //                        //logger.Debug("Saving {0} reading {1}", device.Context.DeviceType, recNum);
                    //                        MonitorManager.SaveMonitorReading(monitor, reading);
                    //                        //logger.Debug("Reading {0} saved", recNum);
                    //                    }

                    //                    device.Close();
                    //                    MonitorManager.UpdateMonitorDownloadInformation(monitor);
                    //                }

                    //            } catch (Exception ex) {
                    //                allDone = false;
                    //                logger.Error(ex.Message);
                    //                logger.Debug(ex);
                    //            } finally {
                    //                logger.Info("polled {0} readings from monitor {1}:{2} ({3}) /{4} seconds/",
                    //                    recNum, monitor.MonitorType.Trim(), monitor.ModbusAddress.Trim(), monitor.DateOfLastDataDownload,
                    //                    (DateTime.Now - pollStarted).TotalSeconds);
                    //            }

                    //        } else
                    //            logger.Info("ignoring monitor {0}:{1}. Connection type {2}", monitor.MonitorType.Trim(), monitor.ModbusAddress.Trim(), monitor.ConnectionType.Trim());

                    //    if (allDone)
                    //        // remove the current poll request from the polling list
                    //        this.pollingList.Remove(requestID);
                    //    else {
                    //        // we encountered some errors. device may be busy etc. re-poll this equipment in about five minutes
                    //        logger.Warn("Polling was interrupted due to excessive errors. Will re-try in about five minutes");
                    //        this.pollingScheduler.Add(requestID, CronSchedule.Parse("Every 5 Minutes"), 0, false);
                    //    }
                    //}
                } catch (Exception ex) {
                    logger.Error(ex.Message);
                    logger.Debug(ex);
                } finally {
                    logger.Info("Poll request processed: {0}", requestID);
                }
            }

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


