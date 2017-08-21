// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.ProjectManagement.Projects;
using NuGet.Versioning;

namespace NuGet.PackageManagement.VisualStudio
{
    public class NuGetProjectDependencyVersionLookup
    {
        private Dictionary<string, DependencyVersionLookup> _projectDependencyVersionLookup;

        public NuGetProjectDependencyVersionLookup()
        {
            _projectDependencyVersionLookup = new Dictionary<string, DependencyVersionLookup>();
        }

        // This method reads the assets file and constructs the appropriate object with references
        // to the installed dependencies found there. It should NOT be called often because it's
        // an expensive operation
        public static async Task<NuGetProjectDependencyVersionLookup> GetLookupFromAssetsFileForProjectsAsync(IEnumerable<NuGetProject> projects)
        {
            var lookup = new NuGetProjectDependencyVersionLookup();
            foreach (var project in projects)
            {
                if (project is BuildIntegratedNuGetProject)
                {
                    // If restore hasn't run this will return an empty list
                    var dependencies = await BuildIntegratedProjectUtility.GetProjectPackageDependencies(project as BuildIntegratedNuGetProject, false);
                    if (dependencies != null || dependencies.Any())
                    {
                        // If we are targeting multiple frameworks we should get the Min version to show (WIP: Add to spec and ask for feedback)
                        var projectDependency = new DependencyVersionLookup(dependencies.GroupBy(item => item.Id).ToDictionary(x => x.Key, x => x.Min(y => y.Version)));
                        lookup._projectDependencyVersionLookup[NuGetProject.GetUniqueNameOrName(project)] = projectDependency;
                    }
                }
            }
            return lookup;
        }

        // Replaces project.GetInstalledPackagesAsync. This methods gets all the installed packages for
        // a given project and updates them with the resolved version if available
        public async Task<IEnumerable<PackageReference>> GetResolvedPackagesIfAvailableAsync(NuGetProject project, CancellationToken cancellationToken)
        {
            var packages = await project.GetInstalledPackagesAsync(cancellationToken);

            DependencyVersionLookup projectLookup = null;
            // Only resolve versions if project is build integrated and restore has run
            if (project is BuildIntegratedNuGetProject && TryGet(project, out projectLookup))
            {
                // We need to update the direct dependencies with the version they actually resolved to
                // when project ran restore.

                var resolvedPackages = new List<PackageReference>();
                //  Update the identity of the dependencies to use the actual resolved version
                foreach (var package in packages)
                {
                    // Update the dependency identity to the actual target dependency version
                    NuGetVersion version = null;
                    if (projectLookup.TryGet(package.PackageIdentity.Id, out version))
                    {
                        var identity = new PackageIdentity(package.PackageIdentity.Id, version);
                        resolvedPackages.Add(PackageReference.CloneWithNewIdentity(package, identity));
                    }
                    else
                    {
                        resolvedPackages.Add(package);
                    }
                }
                return resolvedPackages;
            }
            // If restore hasn't run then fallback to data from package references
            return packages;
        }

        public bool TryGet(NuGetProject project, out DependencyVersionLookup projectLookup)
        {
            projectLookup = null;
            if (_projectDependencyVersionLookup != null && _projectDependencyVersionLookup.Any())
            {
                projectLookup = _projectDependencyVersionLookup[NuGetProject.GetUniqueNameOrName(project)];
                return projectLookup != null;
            }
            return false;
        }

        public bool TryGet(NuGetProject project, string packageId, out NuGetVersion version)
        {
            version = null;
            DependencyVersionLookup projectLookup;
            if (TryGet(project, out projectLookup))
            {
                return projectLookup.TryGet(packageId, out version);
            }
            return false;
        }
    }
}
