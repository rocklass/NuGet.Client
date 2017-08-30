using System;
using System.Collections.Generic;
using NuGet.Packaging.Core;
using NuGet.Versioning;

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

        event EventHandler ActionCompleted;

        void InstallPackage(string packageId, NuGetVersion version);

        void UninstallPackage(string packageId);

        void UpdatePackage(List<PackageIdentity> packages);
    }
}
