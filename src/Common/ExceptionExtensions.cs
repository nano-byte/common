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

using System;
using System.Runtime.Serialization;
using System.Security;

namespace NanoByte.Common
{
    /// <summary>
    /// Provides extension methods for <see cref="Exception"/>s.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Throws a previously thrown exception again, preserving the existing stack trace if possible.
        /// </summary>
        public static void Rethrow(this Exception exception)
        {
            #region Sanity checks
            if (exception == null) throw new ArgumentNullException("exception");
            #endregion

            var serializationInfo = new SerializationInfo(exception.GetType(), new FormatterConverter());
            var streamingContext = new StreamingContext(StreamingContextStates.CrossAppDomain);
            exception.GetObjectData(serializationInfo, streamingContext);

            try
            {
                var objectManager = new ObjectManager(null, streamingContext);
                objectManager.RegisterObject(exception, 1, serializationInfo);
                objectManager.DoFixups();
            }
                // Ignore if preserving stack trace is not possible
            catch (SecurityException)
            {}
            catch (SerializationException)
            {}

            throw exception;
        }
    }
}
