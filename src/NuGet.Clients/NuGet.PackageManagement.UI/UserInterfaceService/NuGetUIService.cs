using System;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NuGet.ProjectManagement;
using NuGet.VisualStudio;

namespace NuGet.PackageManagement.UI
{
    [Export(typeof(INuGetUIService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class NuGetUIService : INuGetUIService
    {
        private IServiceProvider _serviceProvider;

        [ImportingConstructor]
        public NuGetUIService([Import(typeof(SVsServiceProvider))]
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public INuGetUIWindow GetProjectPackageManagerControl(string projectUniqueName)
        {
            return NuGetUIThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await NuGetUIThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var uiShell = _serviceProvider.GetService<SVsUIShell, IVsUIShell>();
                foreach (var windowFrame in VsUtility.GetDocumentWindows(uiShell))
                {
                    object docView;
                    var hr = windowFrame.GetProperty(
                        (int)__VSFPROPID.VSFPROPID_DocView,
                        out docView);
                    if (hr == VSConstants.S_OK
                        && docView is PackageManagerWindowPane)
                    {
                        var packageManagerWindowPane = (PackageManagerWindowPane)docView;
                        if (packageManagerWindowPane.Model.IsSolution)
                        {
                            // the window is the solution package manager
                            continue;
                        }

                        var projects = packageManagerWindowPane.Model.Context.Projects;
                        if (projects.Count() != 1)
                        {
                            continue;
                        }

                        var existingProject = projects.First();
                        var projectName = existingProject.GetMetadata<string>(NuGetProjectMetadataKeys.Name);
                        if (string.Equals(projectName, projectUniqueName, StringComparison.OrdinalIgnoreCase))
                        {
                            var packageManagerControl = VsUtility.GetPackageManagerControl(windowFrame);
                            if (packageManagerControl != null)
                            {
                                return packageManagerControl;
                            }
                        }
                    }
                }

                return null;
            });
        }
    }
}
