﻿using NuGet.PackagingCore;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NuGet.Client.V2
{
    public class V2MetadataResource : MetadataResource
    {
        private readonly IPackageRepository V2Client;
         public V2MetadataResource(IPackageRepository repo)
        {
            V2Client = repo;
        }

        public V2MetadataResource(V2Resource resource)
        {
            V2Client = resource.V2Client;
        }     

        public override async Task<IEnumerable<KeyValuePair<string, bool>>> ArePackagesSatellite(IEnumerable<string> packageId, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public override async Task<IEnumerable<KeyValuePair<string, NuGetVersion>>> GetLatestVersions(IEnumerable<string> packageIds, bool includePrerelease, bool includeUnlisted, CancellationToken token)
        {
            List<KeyValuePair<string, NuGetVersion>> results = new List<KeyValuePair<string, NuGetVersion>>();
            foreach (var id in packageIds)
            {
                    //check if a package by that Id exists.
                    IEnumerable<IPackage> packages = V2Client.FindPackagesById(id);
                    if (packages == null || packages.Count() == 0)
                    {
                        results.Add(new KeyValuePair<string, NuGetVersion>(id, null));
                    }
                    else
                    {
                        SemanticVersion latestVersion = packages.OrderByDescending(p => p.Version).FirstOrDefault().Version;
                        //  return new NuGetVersion(latestVersion.Version, latestVersion.SpecialVersion);
                        results.Add(new KeyValuePair<string, NuGetVersion>(id, new NuGetVersion(latestVersion.Version, latestVersion.SpecialVersion)));
                    }
            }
            return results.AsEnumerable();
        }

        public override async Task<IEnumerable<NuGetVersion>> GetVersions(string packageId, bool includePrerelease, bool includeUnlisted, CancellationToken token)
        {
            return V2Client.FindPackagesById(packageId).Where(p => includeUnlisted || p.Listed)
                .Select(p => new NuGetVersion(p.Version.Version, p.Version.SpecialVersion))
                .Where(v => includePrerelease || !v.IsPrerelease).ToArray();
        }
    }
}
