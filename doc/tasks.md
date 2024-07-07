---
uid: task-system
---

# Task system

The <xref:NanoByte.Common.Tasks> namespace provides a framework for managing long-running tasks and reporting progress to the user.

## Tasks

Tasks are represented using the <xref:NanoByte.Common.Tasks.ITask> interface.

This library provides general-purpose implementations such as <xref:NanoByte.Common.Tasks.ActionTask> and <xref:NanoByte.Common.Tasks.ForEachTask>, as well as use-case specific ones such as <xref:NanoByte.Common.Net.DownloadFile> and <xref:NanoByte.Common.Storage.ReadFile>. You can also implement your own.

## Handlers

The <xref:NanoByte.Common.Tasks.ITaskHandler> interface represents a user interface for reporting task progress as well as displaying prompts and outputs to the user. This library provides a number of implementations:

- <xref:NanoByte.Common.Tasks.CliTaskHandler> for a basic command-line interface
- <xref:NanoByte.Common.Tasks.AnsiCliTaskHandler> for a more advanced command-line interface using ANSI codes
- <xref:NanoByte.Common.Tasks.DialogTaskHandler> for a graphical interface using WinForms dialog boxes
- <xref:NanoByte.Common.Tasks.SilentTaskHandler> for background execution or unit tests
- <xref:NanoByte.Common.Tasks.ServiceTaskHandler> for integration with [Microsoft.Extensions.DependencyInjection](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)

Methods that want to run <xref:NanoByte.Common.Tasks.ITask>s should take an <xref:NanoByte.Common.Tasks.ITaskHandler> as an input parameter.  
To run an <xref:NanoByte.Common.Tasks.ITask>, pass it to the [ITaskHandler.RunTask()](xref:NanoByte.Common.Tasks.ITaskHandler#NanoByte_Common_Tasks_ITaskHandler_RunTask_NanoByte_Common_Tasks_ITask_) method. This will then internally call [ITask.Run()](xref:NanoByte.Common.Tasks.ITask#NanoByte_Common_Tasks_ITask_Run_System_Threading_CancellationToken_NanoByte_Common_Net_ICredentialProvider_System_IProgress_NanoByte_Common_Tasks_TaskSnapshot__) and take care of setting up progress tracking, cancellation, etc.. Additional methods such as [ITaskHandler.Ask()](xref:NanoByte.Common.Tasks.ITaskHandler#NanoByte_Common_Tasks_ITaskHandler_Ask_System_String_System_Nullable_System_Boolean__System_String_) can be used for user interaction.

## Threading

<xref:NanoByte.Common.Tasks.ITask>s provide no threading or asynchronicity concept by themselves. Their [.Run()](xref:NanoByte.Common.Tasks.ITask#NanoByte_Common_Tasks_ITask_Run_System_Threading_CancellationToken_NanoByte_Common_Net_ICredentialProvider_System_IProgress_NanoByte_Common_Tasks_TaskSnapshot__) methods block until the tasks is complete. However, they can be cancelled from other threads via <xref:System.Threading.CancellationToken>s.

<xref:NanoByte.Common.Tasks.ITaskHandler> implementations are thread-safe and support running multiple <xref:NanoByte.Common.Tasks.ITask>s concurrently.  
[ITaskHandler.RunTask()](xref:NanoByte.Common.Tasks.ITaskHandler#NanoByte_Common_Tasks_ITaskHandler_RunTask_NanoByte_Common_Tasks_ITask_) blocks until the tasks is complete, however some implementations may perform the actual task execution on a separate thread.  
<xref:NanoByte.Common.Tasks.DialogTaskHandler> keeps the WinForms message loop pumping while a task is running, so calling `.RunTask()` from the GUI thread will not freeze the GUI. However it does prevent user interaction (other than canceling the task) via a modal dialog box.

## Comparison with async/await

While <xref:NanoByte.Common.Tasks.ITask> has some superficial similarities with the <xref:System.Threading.Tasks.Task> class used by the C# `async`/`await` keywords, these two concepts should not be confused.

The `async`/`await` keywords are part of the [Task Asynchronous Programming model](https://docs.microsoft.com/dotnet/csharp/programming-guide/concepts/async/task-asynchronous-programming-model) (TAP). The TAP provides an abstraction over asynchronous code, enabling the execution of continuations after tasks have completed. This is intended to increase the performance and responsiveness of applications. Many TAP methods accept <xref:System.Threading.CancellationToken>s to signal that a task should be cancelled and <xref:System.IProgress`1> to report a task's progress.

NanoByte.Common's Task system is intended for managing long-running tasks. It provides an abstraction over UIs for interacting with such tasks. It uses the same <xref:System.Threading.CancellationToken> and <xref:System.IProgress`1> as the TAP, but takes care of managing them internally for most use cases.

As a rule of thumb:

- Use `await` and <xref:System.Threading.Tasks.Task> if you want to trigger a short task from a GUI thread.
- Use [ITaskHandler.RunTask()](xref:NanoByte.Common.Tasks.ITaskHandler#NanoByte_Common_Tasks_ITaskHandler_RunTask_NanoByte_Common_Tasks_ITask_) and <xref:NanoByte.Common.Tasks.ITask> if you want to run a longer task on a non-GUI thread (potentially reporting back to a GUI thread).
