﻿// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Threading.Tasks;
using Spectre.Console;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Container for one or more ANSI console progress bars.
    /// </summary>
    public sealed class AnsiCliProgressContext : IDisposable
    {
        private readonly TaskCompletionSource<bool> _completion = new();
        private ProgressContext _context = default!;

        /// <summary>
        /// Starts a progress context.
        /// </summary>
        public AnsiCliProgressContext()
            => AnsiCli.Stderr
                      .Progress()
                      .AutoClear(true)
                      .HideCompleted(true)
                      .Columns(new ProgressColumn[] {new TaskDescriptionColumn(), new ProgressBarColumn(), new PercentageColumn(), new RemainingTimeColumn()})
                      .StartAsync(async ctx =>
                       {
                           _context = ctx;
                           await _completion.Task;
                       });

        public bool IsFinished => _context.IsFinished;

        /// <summary>
        /// Ends the progress context.
        /// </summary>
        public void Dispose()
            => _completion.SetResult(true);

        /// <summary>
        /// Adds a new progress bar to the context.
        /// </summary>
        /// <param name="description">A descriptive text to show next to the progress bar.</param>
        /// <returns>A handle for updating the state of the progress bar.</returns>
        public IProgress<TaskSnapshot> Add(string description)
            => new AnsiCliProgress(_context.AddTask(description, new() {AutoStart = false}));
    }
}