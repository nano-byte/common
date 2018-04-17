// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;
using System.IO;
using JetBrains.Annotations;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Downloads a file from a specific internet address to a local file.
    /// </summary>
    public class DownloadFile : DownloadTask
    {
        /// <summary>
        /// The local path to save the file to.
        /// </summary>
        [Description("The local path to save the file to.")]
        [NotNull]
        public string Target { get; }

        /// <summary>
        /// Creates a new download task.
        /// </summary>
        /// <param name="source">The URL the file is to be downloaded from.</param>
        /// <param name="target">The local path to save the file to. A preexisting file will be overwritten.</param>
        /// <param name="bytesTotal">The number of bytes the file to be downloaded is long. The file will be rejected if it does not have this length. -1 if the size is unknown.</param>
        public DownloadFile([NotNull] Uri source, [NotNull, Localizable(false)] string target, long bytesTotal = -1)
            : base(source, bytesTotal)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(target)) throw new ArgumentNullException(nameof(target));
            #endregion

            Target = target;
        }

        /// <inheritdoc/>
        protected override Stream CreateTargetStream() => File.Open(Target, FileMode.OpenOrCreate, FileAccess.Write);
    }
}
