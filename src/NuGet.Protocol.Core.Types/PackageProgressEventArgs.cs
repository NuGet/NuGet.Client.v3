﻿using NuGet.Configuration;
using NuGet.Packaging.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Protocol.Core.Types
{
    public class PackageProgressEventArgs : EventArgs
    {
        private readonly PackageIdentity _identity;
        private readonly PackageSource _source;
        private readonly double _complete;

        /// <summary>
        /// The status of a package action.
        /// </summary>
        /// <param name="identity">package identity</param>
        /// <param name="source">repository source or null</param>
        /// <param name="complete">0.0 - 1.0</param>
        public PackageProgressEventArgs(PackageIdentity identity, PackageSource source, double complete)
        {
            if (complete < 0 || complete > 1)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }

            _identity = identity;
            _source = source;
            _complete = complete;
        }

        public PackageIdentity PackageIdentity
        {
            get
            {
                return _identity;
            }
        }

        public PackageSource PackageSource
        {
            get
            {
                return _source;
            }
        }

        /// <summary>
        /// Completion - 0.0 - 1.0
        /// </summary>
        public double Complete
        {
            get
            {
                return _complete;
            }
        }

        /// <summary>
        /// True at 100% completion
        /// </summary>
        public bool IsComplete
        {
            get
            {
                return _complete == 1;
            }
        }

        public bool HasPackageSource
        {
            get
            {
                return PackageSource != null;
            }
        }
    }
}
