// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common
{
    /// <summary>
    /// An entity that has a unique name that can be used for identification in lists and trees.
    /// </summary>
    /// <see cref="Collections.NamedCollection{T}"/>
    public interface INamed
    {
        /// <summary>
        /// A unique name for the object.
        /// </summary>
        [Description("A unique name for the object.")]
        string Name { get; set; }
    }

    /// <summary>
    /// Static companion for <see cref="INamed"/>.
    /// </summary>
    public static class Named
    {
        /// <summary>
        /// The default separator to use in <see cref="INamed.Name"/> for tree hierarchies.
        /// </summary>
        public const char TreeSeparator = '|';
    }
}
