using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.ProjectManagement;
using NuGet.Versioning;
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

        public ApexTestUIProject GetApexTestUIProject(string project)
        {
            return new ApexTestUIProject(_nuGetUIService.GetProjectPackageManagerControl(project));
        } 
    }
}
