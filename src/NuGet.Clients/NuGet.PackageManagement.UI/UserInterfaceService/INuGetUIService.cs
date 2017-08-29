using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.PackageManagement.UI
{
    internal interface INuGetUIService
    {
        INuGetUIWindow GetProjectPackageManagerControl(string projectUniqueName);
    }
}
