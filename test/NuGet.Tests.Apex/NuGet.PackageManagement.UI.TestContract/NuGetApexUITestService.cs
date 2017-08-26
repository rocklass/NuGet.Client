using System;
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

        public void UISearch(string searchText)
        {
            _nuGetUIService.SearchPackage(searchText);
        }

    }
}
