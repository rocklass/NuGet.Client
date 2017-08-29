using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Protocol.Core.Types;

namespace NuGet.PackageManagement.UI
{
    internal interface INuGetUIWindow
    {
        void Search(string text);

        IEnumerable<PackageSourceMoniker> PackageSources { get; }

        PackageSourceMoniker ActiveSource { get; set; }

        PackageManagerModel Model { get; }

        ItemFilter ActiveFilter { get; set; }

        IEnumerable<PackageItemListViewModel> Packages { get; }

        PackageItemListViewModel FocusedListPackage { get; set; }
    }
}
