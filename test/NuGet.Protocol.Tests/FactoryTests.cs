﻿﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

//using NuGet.Protocol.Core.Types;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Xunit;

//namespace V2V3ResourcesTest
//{
//    public class FactoryTests
//    {

//        [Fact]
//        public async Task Factory_V2()
//        {
//            var v2repo = RepositoryFactory.CreateV2("https://api.nuget.org/v2/");

//            var resource = v2repo.GetResource<UISearchResource>();

//            Assert.NotNull(resource);
//        }

//        [Fact]
//        public async Task Factory_V3()
//        {
//            var repo = RepositoryFactory.CreateV3("https://api.nuget.org/v3/index.json");

//            var resource = repo.GetResource<UISearchResource>();

//            Assert.NotNull(resource);
//        }

//        [Fact]
//        public async Task Factory_All()
//        {
//            var repo = RepositoryFactory.Create("https://api.nuget.org/v3/index.json");

//            var resource = repo.GetResource<UISearchResource>();

//            Assert.NotNull(resource);
//        }
//    }
//}