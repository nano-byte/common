# Undo system

The \ref NanoByte.Common.Undo namespace provides a framework for adding undo/redo functionality to your applications.

**Commands**

Commands that can be executed and also undone are represented using the \ref NanoByte.Common.Undo.IUndoCommand "IUndoCommand" interface.

This library provides a number of implementations for common use cases such as \ref NanoByte.Common.Undo.SetValueCommand "SetValueCommand" and \ref NanoByte.Common.Undo.AddToCollection "AddToCollection", but you can also implement your own.

**Executors**

Methods that wish to execute `IUndoCommand`s should take an \ref NanoByte.Common.Undo.ICommandExecutor "ICommandExecutor" as an input parameter.  
To execute an `IUndoCommand` pass it to the `ICommandExecutor.Execute()` method. This will then internally call `IUndoCommand.Execute()` and record the command for later undo operations.

The class \ref NanoByte.Common.Undo.CommandManager "CommandManager" implements the `ICommandExecutor` interface. You can expose this classes `.Undo()` and `.Redo()` methods to your users in your GUI.
