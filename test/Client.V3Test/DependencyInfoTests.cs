﻿using NuGet.Client;
using NuGet.Frameworks;
using NuGet.PackagingCore;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Client.V3Test
{
    public class DependencyInfoTests : TestBase
    {
        private const string RegBaseUrl = "https://az320820.vo.msecnd.net/registrations-1/";

        [Fact]
        public async Task DependencyInfo_Mvc()
        {
            V3ResolverPackageIndexResource reg = new V3ResolverPackageIndexResource(DataClient, new Uri(RegBaseUrl));

            V3DependencyInfoResource depResource = new V3DependencyInfoResource(DataClient, reg);

            List<PackageIdentity> packages = new List<PackageIdentity>()
            {
                new PackageIdentity("Microsoft.AspNet.Mvc.Razor", NuGetVersion.Parse("6.0.0-beta1")),
            };

            NuGetFramework projectFramework = NuGetFramework.Parse("aspnet50");

            var results = await depResource.ResolvePackages(packages, projectFramework, true);

            Assert.True(results.Count() > 0);
        }
    }
}
