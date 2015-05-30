/*
 * Copyright 2006-2015 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

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
