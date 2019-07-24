//
//  Copyright 2019 Desert Software Solutions Inc.
//    Refactored from ServiceKit.cs: 
//      https://github.com/DesertSoftware/Solutions
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
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace DesertSoftware.Solutions.Service
{
    /// <summary>
    /// Inherit from this class or the Setup class to denote support of the 
    /// /i[nstall] and /u[ninstall] command line switches
    /// </summary>
    public class Installable : Setup { }

    /// <summary>
    /// Inherit from this class or the Installable class to denote support of the 
    /// /i[nstall] and /u[ninstall] command line switches
    /// </summary>
    [RunInstaller(true)]
    public partial class Setup : Installer
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
}
