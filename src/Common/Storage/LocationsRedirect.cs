// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.IO;

namespace NanoByte.Common.Storage
{
    /// <summary>
    /// Disposable class to create a temporary directory where all <see cref="Locations"/> queries are temporarily redirected to.
    /// Useful for testing. Do not use when multi-threading is involved!
    /// </summary>
    public class LocationsRedirect : TemporaryDirectory
    {
        private readonly string _previousPortableBase;
        private readonly bool _previousIsPortable;

        /// <summary>
        /// Creates a uniquely named, empty temporary directory on disk and starts redirecting all <see cref="Locations"/> queries there.
        /// </summary>
        /// <param name="prefix">A short string the directory name should start with.</param>
        /// <exception cref="IOException">A problem occurred while creating a directory in <see cref="System.IO.Path.GetTempPath"/>.</exception>
        /// <exception cref="UnauthorizedAccessException">Creating a directory in <see cref="System.IO.Path.GetTempPath"/> is not permitted.</exception>
        public LocationsRedirect(string prefix)
            : base(prefix)
        {
            _previousPortableBase = Locations.PortableBase;
            _previousIsPortable = Locations.IsPortable;

            Locations.PortableBase = Path;
            Locations.IsPortable = true;
        }

        public override void Dispose()
        {
            try
            {
                Locations.IsPortable = _previousIsPortable;
                Locations.PortableBase = _previousPortableBase;
            }
            finally
            {
                base.Dispose();
            }
        }
    }
}
