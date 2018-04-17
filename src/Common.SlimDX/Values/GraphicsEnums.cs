// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Values
{
    /// <summary>
    /// A generic enumeration for a three-level quality setting.
    /// </summary>
    public enum Quality
    {
        Low,
        Medium,
        High
    }

    /// <summary>
    /// The effects to be display on water (e.g. reflections).
    /// </summary>
    public enum WaterEffectsType
    {
        /// <summary>Don't apply any water effects (except simple alpha-blending)</summary>
        None,

        /// <summary>Refract objects below the water surface</summary>
        RefractionOnly,

        /// <summary>Refract objects below the water surface and reflect the terrain</summary>
        ReflectTerrain,

        /// <summary>Refract objects below the water surface and reflect objects above</summary>
        ReflectAll
    }
}
