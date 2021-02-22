// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Tasks
{
    /// <seealso cref="ITaskHandler.Verbosity"/>
    public enum Verbosity
    {
        /// <summary>Automatically answer questions with defaults when possible. Avoid non-essential output and questions.</summary>
        Batch = -1,

        /// <summary>Normal interactive operation.</summary>
        Normal = 0,

        /// <summary>Display additional information for troubleshooting.</summary>
        Verbose = 1,

        /// <summary>Display detailed information for debugging.</summary>
        Debug = 2
    }
}
