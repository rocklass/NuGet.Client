using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using NuGet.VisualStudio;

namespace NuGet.PackageManagement.UI.TestContract
{
    public class ApexTestUIProject
    {
        private INuGetUIWindow _packageManagerControl;
        private TaskCompletionSource<bool> _taskCompletionSource;

        internal ApexTestUIProject(INuGetUIWindow project)
        {
            _packageManagerControl = project;
        }

        public IEnumerable<PackageItemListViewModel> Search(string searchText)
        {
            NuGetUIThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await NuGetUIThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                _packageManagerControl.ActiveFilter = ItemFilter.All;
                _packageManagerControl.Search(searchText);
            });

            return _packageManagerControl.Packages;
        }

        public void InstallPackage(string packageId, string version)
        {
            _taskCompletionSource = new TaskCompletionSource<bool>();

            _packageManagerControl.ActionCompleted += HandleActionCompetedEvent;

            NuGetUIThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await NuGetUIThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                _packageManagerControl.InstallPackage(packageId, NuGetVersion.Parse(version));
            });

            _taskCompletionSource.Task.Wait();
            _packageManagerControl.ActionCompleted -= HandleActionCompetedEvent;
        }

        public void UninstallPackage(string packageId)
        {
            _taskCompletionSource = new TaskCompletionSource<bool>();

            _packageManagerControl.ActionCompleted += HandleActionCompetedEvent;

            NuGetUIThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await NuGetUIThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                _packageManagerControl.UninstallPackage(packageId);
            });

            _taskCompletionSource.Task.Wait();
            _packageManagerControl.ActionCompleted -= HandleActionCompetedEvent;
        }

        public void UpdatePackage(List<PackageIdentity> packages)
        {
            _taskCompletionSource = new TaskCompletionSource<bool>();

            _packageManagerControl.ActionCompleted += HandleActionCompetedEvent;

            NuGetUIThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await NuGetUIThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                _packageManagerControl.UpdatePackage(packages);
            });

            _taskCompletionSource.Task.Wait();
            _packageManagerControl.ActionCompleted -= HandleActionCompetedEvent;
        }

        private void HandleActionCompetedEvent(object sender, EventArgs e)
        {
            _taskCompletionSource.TrySetResult(true); 
        }
    }
}
