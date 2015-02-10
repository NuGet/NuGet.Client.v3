﻿using NuGet;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using NuGet.PackagingCore;
using System.Threading;

namespace NuGet.Client.V2
{
    public class V2DownloadResource : DownloadResource
    {
        private readonly IPackageRepository V2Client;
        public V2DownloadResource(IPackageRepository repo)
        {
            V2Client = repo;
        }

        public V2DownloadResource(V2Resource resource)
        {
            V2Client = resource.V2Client;
        }

        public override async Task<bool> PackageExists(PackageIdentity identity, CancellationToken token)
        {
            bool exists = false;
            SemanticVersion version = SemanticVersion.Parse(identity.Version.ToString());

            if (V2Client is LocalPackageRepository)
            {
                LocalPackageRepository lrepo = V2Client as LocalPackageRepository;
                //Using Path resolver doesnt work. It doesnt consider the subfolders present inside the source directory. Hence using PackageLookupPaths.
                //return new Uri(Path.Combine(V2Client.Source, lrepo.PathResolver.GetPackageFileName(identity.Id, semVer)));
                //Using version.ToString() as version.Version gives the normalized string even if the nupkg has unnormalized version in its path.
                List<string> paths = lrepo.GetPackageLookupPaths(identity.Id, new SemanticVersion(identity.Version.ToString())).ToList();

                exists = paths.Any(path => File.Exists(Path.Combine(V2Client.Source, path)));
            }
            else if (V2Client is UnzippedPackageRepository)
            {
                UnzippedPackageRepository repo = V2Client as UnzippedPackageRepository;

                // only works for exact version string matches
                if (repo.Exists(identity.Id, version))
                {
                    exists = true;
                }
                else
                {
                    // check for non-exact version string matches
                    exists = repo.FindPackagesById(identity.Id).Any(p => p.Version == version);
                }
            }
            else
            {
                // perform a normal exists check
                exists = V2Client.Exists(identity.Id, version);
            }

            return exists;

        }

        public override async Task<Uri> GetDownloadUrl(PackageIdentity identity, CancellationToken token)
        {
            //*TODOs: Temp implementation. Need to do erorr handling and stuff.
            if (V2Client is DataServicePackageRepository)
            {
                if (V2Client.Exists(identity.Id, new SemanticVersion(identity.Version.ToString())))
                {
                    //TODOs:Not sure if there is some other standard way to get the Url from a dataservice repo. DataServicePackage has downloadurl property but not sure how to get it.
                    return new Uri(Path.Combine(V2Client.Source, identity.Id + "." + identity.Version + ".nupkg"));
                }
                return null;
            }
            else if (V2Client is LocalPackageRepository)
            {
                LocalPackageRepository lrepo = V2Client as LocalPackageRepository;
                //Using Path resolver doesnt work. It doesn't consider the subfolders present inside the source directory. Hence using PackageLookupPaths.
                //return new Uri(Path.Combine(V2Client.Source, lrepo.PathResolver.GetPackageFileName(identity.Id, semVer)));
                //Using version.ToString() as version.Version gives the normalized string even if the nupkg has unnormalized version in its path.
                List<string> paths = lrepo.GetPackageLookupPaths(identity.Id, new SemanticVersion(identity.Version.ToString())).ToList();
                foreach (var path in paths)
                {
                    if (File.Exists(Path.Combine(V2Client.Source, path)))
                    {
                        return new Uri(Path.Combine(V2Client.Source, path));
                    }
                }

                return null;
            }
            else
            {
                // TODO: move the string into a resoure file
                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    "Unable to get download metadata for package {0}", identity.Id));
            }
        }

        public override async Task<Stream> GetStream(PackageIdentity identity, CancellationToken token)
        {
            Stream result = null;
            IPackage package = null;

            SemanticVersion version = SemanticVersion.Parse(identity.Version.ToString());

            // attempt a normal lookup first
            if (!V2Client.TryFindPackage(identity.Id, version, out package))
            {
                // skip further look ups for online repos
                DataServicePackageRepository v2Online = V2Client as DataServicePackageRepository;

                if (v2Online == null)
                {
                    IVersionComparer versionComparer = VersionComparer.VersionRelease;

                    // otherwise search further to find the package - this is needed for v2 non-normalized versions
                    V2Client.FindPackagesById(identity.Id).Any(p => versionComparer.Equals(identity.Version, NuGetVersion.Parse(p.ToString())));
                }
            }

            if (package != null)
            {
                result = package.GetStream();
            }

            return result;
        }
    }
}
