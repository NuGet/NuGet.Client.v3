﻿using Newtonsoft.Json.Linq;
using NuGet.Data;
using NuGet.Versioning;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Client
{
    /// <summary>
    /// Retrieves and caches service index.json files
    /// V3ServiceIndexResource stores the json, all work is done in the provider
    /// </summary>
    [Export(typeof(INuGetResourceProvider))]
    [ResourceProviderMetadata(typeof(V3ServiceIndexResource))]
    public class V3ServiceIndexResourceProvider : INuGetResourceProvider
    {
        private readonly ConcurrentDictionary<string, V3ServiceIndexResource> _cache;
        private readonly DataClient _client;

        public V3ServiceIndexResourceProvider()
            : this(new DataClient())
        {

        }

        public V3ServiceIndexResourceProvider(DataClient client)
        {
            _cache = new ConcurrentDictionary<string, V3ServiceIndexResource>();
            _client = client;
        }

        public bool TryCreate(SourceRepository source, out INuGetResource resource)
        {
            V3ServiceIndexResource index = null;

            string url = source.Source.Url;

            // the file type can easily rule out if we need to request the url
            if (url.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                // check the cache before downloading the file
                if (!_cache.TryGetValue(url, out index))
                {
                    JObject json = _client.GetJObject(new Uri(url));

                    if (json != null)
                    {
                        // Use SemVer instead of NuGetVersion, the service index should always be
                        // in strict SemVer format
                        SemanticVersion version = null;
                        var status = json.Value<string>("version");
                        if (status != null && SemanticVersion.TryParse(status, out version))
                        {
                            if (version.Major == 3)
                            {
                                index = new V3ServiceIndexResource(json);
                            }
                        }
                    }
                }

                // cache the value even if it is null to avoid checking it again later
                _cache.TryAdd(url, index);
            }

            resource = index;
            return resource != null;
        }
    }
}
