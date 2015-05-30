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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Contains extension methods for <see cref="ITaskHandler"/>s.
    /// </summary>
    public static class TaskHandlerExtensions
    {
        /// <summary>
        /// Displays multi-line text to the user only when <see cref="ITaskHandler.Verbosity"/> is <seealso cref="Verbosity.Normal"/> or higher.
        /// </summary>
        /// <param name="handler">Used for the underlying <see cref="ITaskHandler.Output"/>.</param>
        /// <param name="title">A title for the message. Will only be displayed in GUIs, not on the console. Must not contain critical information!</param>
        /// <param name="message">The string to display.</param>
        /// <remarks>Implementations may close the UI as a side effect. Therefore this should be your last call on the handler.</remarks>
        public static void OutputLow([NotNull] this ITaskHandler handler, [NotNull, Localizable(true)] string title, [NotNull, Localizable(true)] string message)
        {
            #region Sanity checks
            if (handler == null) throw new ArgumentNullException("handler");
            #endregion

            if (handler.Verbosity >= Verbosity.Normal) handler.Output(title, message);
        }

        /// <summary>
        /// Displays tabular data to the user only when <see cref="ITaskHandler.Verbosity"/> is <seealso cref="Verbosity.Normal"/> or higher.
        /// </summary>
        /// <param name="handler">Used for the underlying <see cref="ITaskHandler.Output"/>.</param>
        /// <param name="title">A title for the message. Will only be displayed in GUIs, not on the console. Must not contain critical information!</param>
        /// <param name="data">The data to display.</param>
        /// <remarks>Implementations may close the UI as a side effect. Therefore this should be your last call on the handler.</remarks>
        public static void OutputLow<T>([NotNull] this ITaskHandler handler, [NotNull, Localizable(true)] string title, [NotNull, ItemNotNull] IEnumerable<T> data)
        {
            #region Sanity checks
            if (handler == null) throw new ArgumentNullException("handler");
            #endregion

            if (handler.Verbosity >= Verbosity.Normal) handler.Output(title, data);
        }

        /// <summary>
        /// Asks the user a Yes/No/Cancel question.
        /// </summary>
        /// <param name="handler">Used for the underlying <see cref="ITaskHandler.Ask"/>.</param>
        /// <param name="question">The question and comprehensive information to help the user make an informed decision.</param>
        /// <param name="defaultAnswer">The answer to automatically use when <see cref="ITaskHandler.Verbosity"/> is <seealso cref="Verbosity.Batch"/> or lower.</param>
        /// <param name="alternateMessage">A message to output with <see cref="Log.Warn(string)"/> when the <paramref name="defaultAnswer"/> is used instead of asking the user.</param>
        /// <returns><see langword="true"/> if the user answered with 'Yes'; <see langword="false"/> if the user answered with 'No'.</returns>
        /// <exception cref="OperationCanceledException">The user selected 'Cancel'.</exception>
        public static bool Ask([NotNull] this ITaskHandler handler, [NotNull, Localizable(true)] string question, bool defaultAnswer, [CanBeNull, Localizable(true)] string alternateMessage = null)
        {
            #region Sanity checks
            if (handler == null) throw new ArgumentNullException("handler");
            #endregion

            if (handler.Verbosity <= Verbosity.Batch)
            {
                if (!string.IsNullOrEmpty(alternateMessage)) Log.Warn(alternateMessage);
                return defaultAnswer;
            }
            else return handler.Ask(question);
        }
    }
}
