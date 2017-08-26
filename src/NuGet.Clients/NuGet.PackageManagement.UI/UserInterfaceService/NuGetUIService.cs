using System.ComponentModel.Composition;
using NuGet.VisualStudio;

namespace NuGet.PackageManagement.UI
{
    [Export(typeof(INuGetUIService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class NuGetUIService : INuGetUIService
    {
        private PackageManagerControl _packageManagerControl;
      
        public NuGetUIService()
        {

        }

        public void AddPackageManagerControl(PackageManagerControl packageManagerControl)
        {
            _packageManagerControl = packageManagerControl;
        }

        public void SearchPackage(string searchText)
        {
            NuGetUIThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await NuGetUIThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                _packageManagerControl.Search(searchText);
            });
        }
    }
}
