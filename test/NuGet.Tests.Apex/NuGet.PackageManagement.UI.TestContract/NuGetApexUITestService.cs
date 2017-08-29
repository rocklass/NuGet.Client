using System.ComponentModel.Composition;
using NuGet.VisualStudio;

namespace NuGet.PackageManagement.UI.TestContract
{
    [Export(typeof(NuGetApexUITestService))]
    public class NuGetApexUITestService
    {
        private INuGetUIService _nuGetUIService;

        public NuGetApexUITestService()
        {
            _nuGetUIService = ServiceLocator.GetInstance<INuGetUIService>();
        }

        public void UISearch(string searchText, string project)
        {
            var packageManagerControl = _nuGetUIService.GetProjectPackageManagerControl(project);

            NuGetUIThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await NuGetUIThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                packageManagerControl.ActiveFilter = ItemFilter.All;
                packageManagerControl.Search(searchText);
            });

            var packages = packageManagerControl.Packages;
        }

    }
}
