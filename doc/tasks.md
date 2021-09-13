# Task system

The \ref NanoByte.Common.Tasks namespace provides a framework for managing long-running tasks and reporting progress to the user.

### Tasks

Tasks are represented using the \ref NanoByte.Common.Tasks.ITask "ITask" interface.

This library provides general-purpose implementations such as \ref NanoByte.Common.Tasks.SimpleTask and \ref NanoByte.Common.Tasks.ForEachTask, as well as use-case specific ones such as \ref NanoByte.Common.Net.DownloadFile and \ref NanoByte.Common.Storage.ReadFile. You can also implement your own.

### Handlers

The \ref NanoByte.Common.Tasks.ITaskHandler "ITaskHandler" interface represents a user interface for reporting task progress as well as displaying prompts and outputs to the user. This library provides a number of implementations:

- \ref NanoByte.Common.Tasks.CliTaskHandler "CliTaskHandler" for a basic command-line interface
- \ref NanoByte.Common.Tasks.AnsiCliTaskHandler "AnsiCliTaskHandler" for a more advanced command-line interface using ANSI codes
- \ref NanoByte.Common.Tasks.DialogTaskHandler "DialogTaskHandler" for a graphical interface using WinForms dialog boxes
- \ref NanoByte.Common.Tasks.SilentTaskHandler "SilentTaskHandler" for background execution or unit tests
- \ref NanoByte.Common.Tasks.ServiceTaskHandler "ServiceTaskHandler" for integration with [Microsoft.Extensions.DependencyInjection](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)

Methods that wish to run `ITask`s should take an `ITaskHandler` as an input parameter.  
To run an `ITask` pass it to the `ITaskHandler.RunTask()` method. This will then internally call `ITask.Run()` and take care of setting up progress tracking, cancellation, etc.. Additional methods such as `ITaskHandler.Ask()` can be used for user interaction.

### Threading

`ITask`s provide no threading or asynchronicity concept by themselves. Their `.Run()` methods block until the tasks is complete. However, they can be cancelled from other threads via `CancellationToken`s.

`ITaskHandler` implementations are thread-safe and support running multiple `ITask`s concurrently.  
`ITaskHandler.RunTask()` blocks until the tasks is complete, however some implementations may perform the actual task execution on a separate thread.  
`DialogTaskHandler.RunTask()` keeps the WinForms message loop pumping while a task is running, so calling this from the GUI thread will not freeze the GUI. However it does prevent user actions (other than canceling the task) via a modal dialog box.

### Comparison with async/await

While \ref NanoByte.Common.Tasks.ITask "ITask" has some superficial similarities with the `Task` class used by the C# `async`/`await` keywords, these two concepts should not be confused.

The `async`/`await` keywords are part of the [Task Asynchronous Programming model](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/task-asynchronous-programming-model) (TAP). The TAP provides an abstraction over asynchronous code, enabling the execution of continuations after tasks have completed. This is intended to increase the performance and responsiveness of applications. Many TAP methods accept `CancellationToken`s to signal that a task should be cancelled and `IProgress<>` to report a task's progress.

NanoByte.Common's Task system is intended for managing long-running tasks. It provides an abstraction over UIs for interacting with such tasks. It uses the same `CancellationToken` and `IProgress<>` as the TAP, but takes care of managing them internally for most use cases.

As a rule of thumb:

- Use `await` and `Task` if you want to trigger a short task from a GUI thread.
- Use `ITaskHandler.RunTask()` and `ITask` if you want to run a longer task on a non-GUI thread (potentially reporting back to a GUI thread).
