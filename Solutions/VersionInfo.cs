//
//  Copyright 2013, Desert Software Solutions Inc.
//    VersionInfo.cs: 
//      https://github.com/DesertSoftware/Solutions/blob/master/Solutions/VersionInfo.cs
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
using System.Linq;
using System.Reflection;
using System.Text;

namespace DesertSoftware.Solutions
{
    static public class VersionInfo
    {
        static public string ToVersionString(this Assembly assembly) {
            return assembly.ToVersionString( (v) => { return string.Format("{0}.{1} (build {2}.{3:0###})", v.Major, v.Minor, v.Build, v.Revision); });
        }

        /// <summary>
        /// Returns a version string using the supplied formatter for a specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly to retrieve the version for.</param>
        /// <param name="formatter">The formatter.</param>
        /// <returns></returns>
        static public string ToVersionString(this Assembly assembly, Func<Version, string> formatter) {
            if (assembly == null)
                return formatter(new Version("0.0.0.0"));

            return formatter(assembly.GetName().Version ?? new Version("0.0.0.0"));
        }

        /// <summary>
        /// Returns a version string formatted as major.minor (build number) for a specified type.
        /// </summary>
        /// <param name="type">The type to retrieve the version for.</param>
        /// <returns></returns>
        static public string ToVersionString(this Type type) {
            return ToVersionString(type, (v) => { return string.Format("{0}.{1} (build {2}.{3:0###})", v.Major, v.Minor, v.Build, v.Revision); });
        }

        /// <summary>
        /// Returns a version string using the supplied formatter for a specified type.
        /// </summary>
        /// <param name="type">The type to retrieve the version for.</param>
        /// <param name="formatter">The formatter.</param>
        /// <returns></returns>
        static public string ToVersionString(this Type type, Func<Version, string> formatter) {
            return (type != null ? Assembly.GetAssembly(type) : null).ToVersionString();
        }

        /// <summary>
        /// Returns the informational version (Product Version) string attribute value for a specified type.
        /// </summary>
        /// <param name="type">The type to retrieve the informational version for.</param>
        /// <returns></returns>
        static public string ProductVersion(this Type type) {
            return ProductVersion(type != null ? Assembly.GetAssembly(type) : null);
        }

        /// <summary>
        /// Returns the informational version (Product Version) string attribute value for a specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly to retrieve the informational version for.</param>
        /// <returns></returns>
        static public string ProductVersion(this Assembly assembly) {
            if (assembly == null)
                return ToVersionString(assembly);

            var versionInfo = assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false) as AssemblyInformationalVersionAttribute[];

            return versionInfo.Length > 0
                ? versionInfo[0].InformationalVersion
                : ToVersionString(assembly);
        }

    }
}
