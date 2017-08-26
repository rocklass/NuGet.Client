using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.PackageManagement.UI
{
    public interface INuGetUIService
    {
        void AddPackageManagerControl(PackageManagerControl packageManagerControl);

        void SearchPackage(string searchText);
    }
}
