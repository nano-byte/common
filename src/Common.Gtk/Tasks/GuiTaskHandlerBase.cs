using System;
using Gtk;
using NanoByte.Common.Net;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Common base class for GTK# <see cref="ITaskHandler"/> implementations.
    /// </summary>
    public abstract class GuiTaskHandlerBase : TaskHandlerBase
    {
        /// <inheritdoc/>
        protected override void LogHandler(LogSeverity severity, string message)
        {
            // TODO: Implement
        }

        /// <inheritdoc/>
        protected override ICredentialProvider BuildCrendentialProvider()
        {
            // TODO: Implement
            return null;
        }

        /// <inheritdoc/>
        public override bool Ask(string question)
        {
            #region Sanity checks
            if (question == null) throw new ArgumentNullException(nameof(question));
            #endregion

            Log.Debug("Question: " + question);
            switch (Msg.YesNoCancel(null, question, MsgSeverity.Warn))
            {
                case ResponseType.Yes:
                    Log.Debug("Answer: Yes");
                    return true;
                case ResponseType.No:
                    Log.Debug("Answer: No");
                    return false;
                case ResponseType.Cancel:
                default:
                    Log.Debug("Answer: Cancel");
                    throw new OperationCanceledException();
            }
        }

        /// <inheritdoc/>
        public override void Output(string title, string message)
        {
            #region Sanity checks
            if (title == null) throw new ArgumentNullException(nameof(title));
            if (message == null) throw new ArgumentNullException(nameof(message));
            #endregion

            Msg.Inform(null, title + Environment.NewLine + message.TrimEnd(Environment.NewLine.ToCharArray()), MsgSeverity.Info);
        }

        /// <inheritdoc/>
        public override void Error(Exception exception)
        {
            #region Sanity checks
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            #endregion

            Log.Error(exception);
            Msg.Inform(null, exception.Message, MsgSeverity.Error);
        }
    }
}