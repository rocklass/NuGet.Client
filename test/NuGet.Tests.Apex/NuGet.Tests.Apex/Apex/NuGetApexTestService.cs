using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.Test.Apex;
using Microsoft.Test.Apex.VisualStudio;
using Microsoft.Test.Apex.VisualStudio.Solution;
using Microsoft.VisualStudio.Shell.Interop;
using NuGet.PackageManagement.VisualStudio;
using NuGet.ProjectManagement;
using NuGet.VisualStudio;
using NuGetVSExtension;

namespace NuGet.Tests.Apex
{
    [Export(typeof(NuGetApexTestService))]
    public class NuGetApexTestService : VisualStudioTestService<NuGetApexVerifier>
    {

        protected internal IVsUIShell VsUIShell => VisualStudioObjectProviders.GetService<SVsUIShell, IVsUIShell>();
        /// <summary>
        /// Gets the NuGet IVsPackageInstallerServices
        /// </summary>
        protected internal IVsPackageInstallerServices InstallerServices
        {
            get
            {
                return this.VisualStudioObjectProviders.GetComponentModelService<IVsPackageInstallerServices>();
            }
        }

        /// <summary>
        /// Gets the NuGet IVsPackageInstaller
        /// </summary>
        protected internal IVsPackageInstaller PackageInstaller
        {
            get
            {
                return this.VisualStudioObjectProviders.GetComponentModelService<IVsPackageInstaller>();
            }
        }

        protected internal DTE Dte
        {
            get
            {
                return this.VisualStudioObjectProviders.DTE;
            }
        }


        /// <summary>
        /// Gets the NuGet IVsPackageUninstaller
        /// </summary>
        protected internal IVsPackageUninstaller PackageUninstaller
        {
            get
            {
                return this.VisualStudioObjectProviders.GetComponentModelService<IVsPackageUninstaller>();
            }
        }

        /// <summary>
        /// Installs the specified NuGet package into the specified project
        /// </summary>
        /// <param name="project">Project name</param>
        /// <param name="packageName">NuGet package name</param>
        public void InstallPackage(string projectName, string packageName)
        {
            this.InstallPackage(projectName, packageName, null);
        }

        /// <summary>
        /// Installs the specified NuGet package into the specified project
        /// </summary>
        /// <param name="project">Project name</param>
        /// <param name="packageName">NuGet package name</param>
        /// <param name="packageVersion">NuGet package version</param>
        public void InstallPackage(string projectName, string packageName, string packageVersion)
        {
            Logger.WriteMessage("Now installing NuGet package [{0} {1}] into project [{2}]", packageName, packageVersion, packageName);

            this.InstallPackage(null, projectName, packageName, packageVersion);
        }

        /// <summary>
        /// Installs the specified NuGet package into the specified project
        /// </summary>
        /// <param name="source">Project source</param>
        /// <param name="project">Project name</param>
        /// <param name="packageName">NuGet package name</param>
        /// <param name="packageVersion">NuGet package version</param>
        public void InstallPackage(string source, string projectName, string packageName, string packageVersion)
        {
            Logger.WriteMessage("Now installing NuGet package [{0} {1} {2}] into project [{3}]", source, packageName, packageVersion, projectName);

            var project = Dte.Solution.Projects.Item(projectName);

            try
            {
                this.PackageInstaller.InstallPackage(source, project, packageName, packageVersion, false);
            }
            catch (InvalidOperationException e)
            {
                Logger.WriteException(EntryType.Warning, e, string.Format("An error occured while attempting to install package {0}", packageName));
            }
        }

        /// <summary>
        /// Uninstalls only the specified NuGet package from the project.
        /// </summary>
        /// <param name="project">Project name</param>
        /// <param name="packageName">NuGet package name</param>
        public void UninstallPackage(string projectName, string packageName)
        {
            this.UninstallPackage(projectName, packageName, false);
        }

        /// <summary>
        /// Uninstalls the specified NuGet package from the project
        /// </summary>
        /// <param name="project">Project name</param>
        /// <param name="packageName">NuGet package name</param>
        /// <param name="removeDependencies">Whether to uninstall any package dependencies</param>
        public void UninstallPackage(string projectName, string packageName, bool removeDependencies)
        {
            Logger.WriteMessage("Now uninstalling NuGet package [{0}] from project [{1}]", packageName, projectName);

            var project = Dte.Solution.Projects.Item(projectName);

            try
            {
                this.PackageUninstaller.UninstallPackage(project, packageName, removeDependencies);
            }
            catch (InvalidOperationException e)
            {
                Logger.WriteException(EntryType.Warning, e, string.Format("An error occured while attempting to uninstall package {0}", packageName));
            }
        }

        /// <summary>
        /// Open NuGet Manager UI for project level
        /// </summary>
        public void OpenNuGetPackageManagerUIForProject()
        {
            NuGetUIThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await NuGetUIThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                Dte.ExecuteCommand("Project.ManageNuGetPackages");
            });
        }

        /// <summary>
        /// Returns a PackageManagerControl for a  NuGet package manager open at solution level if exists
        /// </summary>
        public INuGetUIWindow GetManagerControlForSolution()
        {
            NuGetUIThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await NuGetUIThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                foreach (var windowFrame in VsUtility.GetDocumentWindows(VsUIShell))
                {
                    var packageManagerControl = VsUtility.GetPackageManagerControl(windowFrame);
                    if (packageManagerControl.Model.IsSolution)
                    {
                        return packageManagerControl;
                    }
                }
                return null;
            });
        }

        /// <summary>
        /// Returns the PackageManagerControl if the given project NuGet package manager window is open
        /// </summary>
        public INuGetUIWindow GetManagerControlForProject(Project project)
        {
            NuGetUIThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await NuGetUIThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                foreach (var windowFrame in VsUtility.GetDocumentWindows(VsUIShell))
                {
                    var packageManagerControl = VsUtility.GetPackageManagerControl(windowFrame);

                    if (packageManagerControl.Model.IsSolution)
                    {
                        continue;
                    }

                    var projects = packageManagerControl.Model.Context.Projects;
                    if (projects.Count() != 1)
                    {
                        continue;
                    }

                    var existingProject = projects.First();
                    var projectName = existingProject.GetMetadata<string>(NuGetProjectMetadataKeys.Name);
                    if (string.Equals(projectName, project.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        return packageManagerControl;
                    }
                }
                return null;
            });
        }

        /// <summary>
        /// Open NuGet Powershell Management Console
        /// </summary>
        public void OpenPowershellConsole()
        {
            NuGetUIThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await NuGetUIThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                Dte.ExecuteCommand("View.PackageManagerConsole");
            });
        }
    }
}
