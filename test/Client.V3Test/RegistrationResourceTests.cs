﻿using NuGet.Client;
using NuGet.PackagingCore;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Client.V3Test
{
    public class RegistrationResourceTests : TestBase
    {
        private const string RegBaseUrl = "https://az320820.vo.msecnd.net/registrations-1/";

        [Fact]
        public async Task RegistrationResource_NotFound()
        {
            V3ResolverPackageIndexResource resource = new V3ResolverPackageIndexResource(DataClient, new Uri(RegBaseUrl));

            var package = await resource.GetPackageMetadata(new PackageIdentity("notfound23lk4j23lk432j4l", new NuGetVersion(1, 0, 99)), CancellationToken.None);

            Assert.Null(package);
        }

        [Fact]
        public async Task RegistrationResource_Tree()
        {
            V3ResolverPackageIndexResource resource = new V3ResolverPackageIndexResource(DataClient, new Uri(RegBaseUrl));

            var packages = await resource.GetPackageMetadata("ravendb.client", true, false, CancellationToken.None);

            var results = packages.ToArray();

            Assert.True(results.Length > 500);
        }

        [Fact]
        public async Task RegistrationResource_TreeFilterOnPre()
        {
            V3ResolverPackageIndexResource resource = new V3ResolverPackageIndexResource(DataClient, new Uri(RegBaseUrl));

            var packages = await resource.GetPackageMetadata("ravendb.client", false, false, CancellationToken.None);

            var results = packages.ToArray();

            Assert.True(results.Length < 500);
        }

        [Fact]
        public async Task RegistrationResource_NonTree()
        {
            V3ResolverPackageIndexResource resource = new V3ResolverPackageIndexResource(DataClient, new Uri(RegBaseUrl));

            var packagesPre = await resource.GetPackageMetadata("newtonsoft.json", true, false, CancellationToken.None);
            var packages = await resource.GetPackageMetadata("newtonsoft.json", false, false, CancellationToken.None);

            var results = packages.ToArray();
            var resultsPre = packagesPre.ToArray();

            Assert.True(results.Length > 10);
            Assert.True(results.Length < resultsPre.Length);
        }
    }
}
