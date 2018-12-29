//
//  Copyright 2015, Desert Software Solutions Inc.
//    ServiceKit.cs: https://gist.github.com/rostreim/47e63f01fbaf8556b847
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;

public class ServiceFactory
{
    const string GREETING = "A Desert Software solution <http://desertsoftware.com>";

    /// <summary>
    /// Runs or configures a service described in the specified configurator and command line arguments.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="args">The arguments.</param>
    /// <param name="configurator">The configurator.</param>
    /// <returns></returns>
    static public int Run<T>(string[] args, Action<ServiceConfigurator<T>> configurator) {
        var svcConfigurator = new ServiceConfigurator<T>();
        var logger = Gel.Logger.Log("service"); // get the error logger in case we fault to ensure that we can log 

        // give the user a chance to configure this service
        configurator(svcConfigurator);

        if (svcConfigurator.ConsoleGreeting == null)
            svcConfigurator.ConsoleGreeting = () => {
                Console.WriteLine("{0} service {1}", Setup.serviceInstaller.DisplayName, typeof(T).ToVersionString());
                Console.WriteLine(GREETING); //"A Desert Software solution <http://desertsoftware.com>");
            };

        // now parse the command line
        var state = RunState.ExecuteCommandLine<T>(args, svcConfigurator.ConsoleGreeting, Setup.serviceInstaller);

        if (state.TimeToExit)
            return 0;

        // attempt to log an informational message to ensure logging is configured properly
        if (svcConfigurator.ServiceGreeting == null) {
            logger.Info("{1} service {0}", typeof(T).ToVersionString(), Setup.serviceInstaller.DisplayName);
            logger.Info(GREETING); //"A Desert Software solution <http://www.desertsoftware.com>");
        } else
            svcConfigurator.ServiceGreeting();

        try {
            // TODO: revisit this
            if (svcConfigurator.ServiceInstance == null)
                svcConfigurator.ServiceInstance = new Service<T>();

            if (!state.RunAsService) {
                var svc = svcConfigurator.ServiceInstance;

                svc.Start(); // svc.Start(args);
                Console.WriteLine();
                Console.WriteLine("Service is running. Press enter to quit ... ");
                Console.WriteLine();
                Console.ReadLine();
                svc.Stop();
            } else
                ServiceBase.Run(svcConfigurator.ServiceInstance);
        } catch (Exception ex) {
            logger.Fatal(ex);
            return -1;
        }

        return 0;
    }

    /// <summary>
    /// The service configurator that is used in a ServiceFactory.Run operation to configure a service
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceConfigurator<T>
    {
        /// <summary>
        /// Sets the display name of the service. The name that is displayed in the service control manager.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName { set { Setup.serviceInstaller.DisplayName = value; } }

        /// <summary>
        /// Sets the name of the service. The name by which this service is started and stopped (eg. net start servicename)
        /// </summary>
        /// <value>
        /// The name of the service.
        /// </value>
        public string ServiceName { set { Setup.serviceInstaller.ServiceName = value; } }

        /// <summary>
        /// Sets the description of the service as diplayed in the service control manager.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { set { Setup.serviceInstaller.Description = value; } }

        /// <summary>
        /// Sets the account that the service will execute in.
        /// </summary>
        /// <value>
        /// The account.
        /// </value>
        public ServiceAccount Account { set { Setup.serviceProcessInstaller.Account = value; } }

        /// <summary>
        /// Sets the username of a local or domain account.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { set { Setup.serviceProcessInstaller.Username = value; } }

        /// <summary>
        /// Sets the password of the Username account.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { set { Setup.serviceProcessInstaller.Password = value; } }

        /// <summary>
        /// Gets or sets the service greeting routine to be executed.
        /// </summary>
        /// <value>
        /// The service greeting.
        /// </value>
        public Action ServiceGreeting { internal get; set; }

        /// <summary>
        /// Gets or sets the console greeting routine to be executed.
        /// </summary>
        /// <value>
        /// The console greeting.
        /// </value>
        public Action ConsoleGreeting { internal get; set; }

        /// <summary>
        /// Gets or sets the service instance.
        /// </summary>
        /// <value>
        /// The service instance.
        /// </value>
        internal Service<T> ServiceInstance { get; set; }

        /// <summary>
        /// Configures bindings to the service class such as starting and stopping.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        public void Service(Action<IServiceConfiguration<T>> constructor) {
            this.ServiceInstance = new Service<T>();

            constructor(this.ServiceInstance);
        }
    }

    /// <summary>
    /// Service configuration options that are made available when the Service method is executed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IServiceConfiguration<T>
    {
        /// <summary>
        /// Specifies how to constructs the T service class.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        void ConstructUsing(Func<string, T> constructor);

        /// <summary>
        /// Specifies how the service class is started.
        /// </summary>
        /// <param name="doStart">The do start.</param>
        void OnStart(Action<T> doStart);

        /// <summary>
        /// Specifies how the service class is stoppd.
        /// </summary>
        /// <param name="doStop">The do stop.</param>
        void OnStop(Action<T> doStop);
    }

    /// <summary>
    /// A service factory runnable class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Service<T> : ServiceBase, IServiceConfiguration<T>
    {
        private Action<T> start;
        private Action<T> stop;
        private T instance;
        private Func<string, T> constructor;

        /// <summary>
        /// Specifies how to constructs the T service class.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        public void ConstructUsing(Func<string, T> constructor) {
            this.constructor = constructor;
        }

        /// <summary>
        /// Specifies how the service class is started.
        /// </summary>
        /// <param name="doStart">The do start.</param>
        public void OnStart(Action<T> doStart) {
            this.start = doStart;
        }

        /// <summary>
        /// Specifies how the service class is stoppd.
        /// </summary>
        /// <param name="doStop">The do stop.</param>
        public void OnStop(Action<T> doStop) {
            this.stop = doStop;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start() {
            OnStart(new string[] { });
        }

        /// <summary>
        /// Stops the executing service.
        /// </summary>
        public new void Stop() {
            OnStop();
        }

        protected override void OnStart(string[] args) {
            if (this.instance == null) {
                if (this.constructor != null)
                    this.instance = this.constructor("");
                else
                    this.instance = Activator.CreateInstance<T>();
            }

            if (this.start != null)
                this.start(this.instance);
            else
                base.OnStart(args);
        }

        protected override void OnStop() {
            if (this.stop != null)
                this.stop(this.instance);
            else
                base.OnStop();
        }
    }


    private class RunState
    {
        static private RunState ExitNow { get { return new RunState { RunAsService = false, TimeToExit = true }; } }
        static private RunState RunService { get { return new RunState { RunAsService = true, TimeToExit = false }; } }
        static private RunState RunConsole { get { return new RunState { RunAsService = false, TimeToExit = false }; } }

        /// <summary>
        /// Executes the command line.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="greeting">The greeting. eg. Data Collection service</param>
        /// <param name="program">The program type.</param>
        /// <returns></returns>
        static public RunState ExecuteCommandLine<TService>(string[] args, Action greeting, ServiceInstaller setup) {
            var logger = Gel.Logger.Log("service"); // get the error logger in case we fault to ensure that we can log 

            foreach (var arg in args) {
                if (arg.StartsWith("/") || arg.StartsWith("-"))
                    switch (arg.ToLower()[1]) {
                        case '?':
                            if (greeting != null)
                                greeting();

                            Console.WriteLine("Usage:");
                            Console.WriteLine("    /i nstall[:service name]     Install this service optionally named (service name)");
                            Console.WriteLine("    /u install[:service name]    Uninstall this service optionally named (service name)");
                            Console.WriteLine("    /c onsole                    Run as a console application");
                            Console.WriteLine("    /d ebug                      Same as /c onsole");
                            Console.WriteLine("    /?                           Displays this help");
                            Console.WriteLine("    /h elp                       Same as /?");
                            return RunState.ExitNow;

                        case 'i':   // install      /i:"Name of this service"
                            if (!Privileges.HaveAdministratorPrivileges()) {
                                Console.Error.WriteLine("Access Denied. Administrator permissions are needed.");
                                Console.Error.WriteLine("Use 'run as administrator' to install this service.");
                                return RunState.ExitNow;
                            }

                            if (arg.Contains(':')) {
                                setup.DisplayName = arg.Substring(arg.IndexOf(':') + 1);
                                setup.ServiceName = arg.Substring(arg.IndexOf(':') + 1);
                            }

                            try {
                                using (var installer = new AssemblyInstaller(typeof(TService).Assembly, null)) {
                                    installer.UseNewContext = true;
                                    installer.Install(new Dictionary<object, object>());
                                }
                            } catch (Exception ex) {
                                logger.Fatal(ex.Message);
                            }
                            return RunState.ExitNow;

                        case 'u':   // uninstall    /u:"Name of this service"
                            if (!Privileges.HaveAdministratorPrivileges()) {
                                Console.Error.WriteLine("Access Denied. Administrator permissions are needed.");
                                Console.Error.WriteLine("Use 'run as administrator' to uninstall this service.");
                                return RunState.ExitNow;
                            }

                            if (arg.Contains(':')) {
                                setup.DisplayName = arg.Substring(arg.IndexOf(':') + 1);
                                setup.ServiceName = arg.Substring(arg.IndexOf(':') + 1);
                            }

                            try {
                                using (var installer = new AssemblyInstaller(typeof(TService).Assembly, null)) {
                                    installer.UseNewContext = true;
                                    installer.Uninstall(new Dictionary<object, object>());
                                }
                            } catch (Exception ex) {
                                logger.Fatal(ex.Message);
                            }
                            return RunState.ExitNow;

                        case 'c':   // console
                        case 'd':   // debug -> to console
                            return RunState.RunConsole;
                    }
            }

            return Environment.UserInteractive ? RunState.RunConsole : RunState.RunService;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [time to exit].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [time to exit]; otherwise, <c>false</c>.
        /// </value>
        public bool TimeToExit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [run as service].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [run as service]; otherwise, <c>false</c>.
        /// </value>
        public bool RunAsService { get; set; }
    }

    static private class Privileges
    {
        /// <summary>
        /// Determines whether the executing context has administrator privileges.
        /// </summary>
        /// <returns></returns>
        static public bool HaveAdministratorPrivileges() {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}


[RunInstaller(true)]
public sealed partial class Setup : Installer
{
    static public ServiceInstaller serviceInstaller = new ServiceInstaller
    {
        DisplayName = "DesertSoftware.Service",
        ServiceName = "DesertSoftware.Service",
        Description = "A desert software service solution"
    };

    static public ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller
    {
        Account = ServiceAccount.NetworkService,
        Password = null,
        Username = null
    };

    public Setup() {
        var installers = new Installer[] {
                serviceProcessInstaller,
                serviceInstaller
            };

        this.Installers.AddRange(installers);
    }
}

static public class TypeExtensions
{
    /// <summary>
    /// Returns a version string formatted as major.minor (build number) for a specified type.
    /// </summary>
    /// <param name="type">The type to retrieve the version for.</param>
    /// <returns></returns>
    static public string ToVersionString(this Type type) {
        return ToVersionString(type, (v) => { return string.Format("{0}.{1} (build {2})", v.Major, v.Minor, v.Build); });
    }

    /// <summary>
    /// Returns a version string using the supplied formatter for a specified type.
    /// </summary>
    /// <param name="type">The type to retrieve the version for.</param>
    /// <param name="formatter">The formatter.</param>
    /// <returns></returns>
    static public string ToVersionString(this Type type, Func<Version, string> formatter) {
        System.Reflection.Assembly assembly = type != null ? System.Reflection.Assembly.GetAssembly(type) : null;

        if (assembly == null)
            return formatter(new Version("0.0.0.0"));

        return formatter(assembly.GetName().Version ?? new Version("0.0.0.0"));
    }
}
