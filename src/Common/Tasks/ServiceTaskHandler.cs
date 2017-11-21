#if NETSTANDARD2_0
/*
 * Copyright 2006-2017 Bastian Eicher
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
using System.Net;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NanoByte.Common.Net;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Uses <see cref="ILogger{TCategoryName}"/> for output and <see cref="IConfigurationRoot"/> for credentials and suppresses any questions. Use this for non-interactive services.
    /// </summary>
    /// <remarks>Credentials must be stored in a configuration section named <c>Credentials</c>. Each URI must be a subsection with <c>User</c> and <c>Password</c> values.</remarks>
    [CLSCompliant(false)]
    public class ServiceTaskHandler : TaskHandlerBase
    {
        private readonly ILogger<ServiceTaskHandler> _logger;
        private readonly IConfiguration _configuration;

        public ServiceTaskHandler([NotNull] ILogger<ServiceTaskHandler> logger, [NotNull] IConfigurationRoot configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <inheritdoc/>
        protected override void LogHandler(LogSeverity severity, string message)
        {
            switch (severity)
            {
                case LogSeverity.Debug:
                    _logger.LogDebug(message);
                    break;
                case LogSeverity.Info:
                    _logger.LogInformation(message);
                    break;
                case LogSeverity.Warn:
                    _logger.LogWarning(message);
                    break;
                case LogSeverity.Error:
                    _logger.LogError(message);
                    break;
            }
        }

        /// <inheritdoc/>
        protected override ICredentialProvider BuildCredentialProvider()
            => new ExternalCredentialProvider(uri =>
            {
                var cred = _configuration.GetSection("Credentials").GetSection(uri.ToString());
                return new NetworkCredential(cred["User"], cred["Password"]);
            });

        /// <inheritdoc/>
        public override void RunTask(ITask task)
        {
            #region Sanity checks
            if (task == null) throw new ArgumentNullException(nameof(task));
            #endregion

            _logger.LogDebug("Task: " + task.Name);
            task.Run(CancellationToken, CredentialProvider);
        }

        /// <summary>
        /// Always returns <see cref="Tasks.Verbosity.Batch"/>.
        /// </summary>
        public override Verbosity Verbosity { get => Verbosity.Batch; set {} }

        /// <summary>
        /// Always returns <c>false</c>.
        /// </summary>
        protected override bool Ask(string question, MsgSeverity severity)
        {
            _logger.LogDebug("Question: {0}\nAutomatic answer: No", question);
            return false;
        }

        /// <inheritdoc/>
        public override void Output(string title, string message) => _logger.LogInformation("{0}\n{1}", title, message);

        /// <inheritdoc/>
        public override void Error(Exception exception) => _logger.LogError(exception, exception.Message);
    }
}
#endif
