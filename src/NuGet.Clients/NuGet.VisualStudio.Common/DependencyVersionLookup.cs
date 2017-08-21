// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using NuGet.Versioning;

namespace NuGet.PackageManagement.VisualStudio
{
    public class DependencyVersionLookup
    {
        private Dictionary<string, NuGetVersion> _versionLookup;

        public DependencyVersionLookup(Dictionary<string, NuGetVersion> lookupDict)
        {
            _versionLookup = lookupDict;
        }

        public bool TryGet(string packageId, out NuGetVersion version)
        {
            version = null;
            if (_versionLookup == null ||!_versionLookup.Any())
            {
                return false;
            }
            version = _versionLookup[packageId];
            return version != null;
        }
    }
}
