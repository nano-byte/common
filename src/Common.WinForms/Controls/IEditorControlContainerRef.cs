/*
 * Copyright 2006-2014 Bastian Eicher
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

using System.ComponentModel;

namespace NanoByte.Common.Controls
{
    /// <summary>
    /// Provides an interface to a control that edits a single object and has a reference to the containing object.
    /// </summary>
    /// <typeparam name="T">The type of object to edit.</typeparam>
    /// <typeparam name="TContainer">The type of the container of <typeparamref name="T"/>.</typeparam>
    public interface IEditorControlContainerRef<T, TContainer> : IEditorControl<T>
    {
        /// <summary>
        /// The <see cref="IEditorControl{T}.Target"/>'s container.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        TContainer ContainerRef { get; set; }
    }
}
