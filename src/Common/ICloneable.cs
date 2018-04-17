// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common
{
    /// <summary>
    /// Supports cloning.
    /// </summary>
    public interface ICloneable<out T>
    {
        /// <summary>Creates a new clone of this instance.</summary>
        T Clone();
    }
}
