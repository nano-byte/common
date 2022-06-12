---
uid: undo-system
---

# Undo system

The <xref:NanoByte.Common.Undo> namespace provides a framework for adding undo/redo functionality to your applications.

## Commands

Commands that can be executed and also undone are represented using the <xref:NanoByte.Common.Undo.IUndoCommand> interface.

This library provides a number of implementations for common use cases such as <xref:NanoByte.Common.Undo.SetValueCommand> and <xref:NanoByte.Common.Undo.AddToCollection>, but you can also implement your own.

## Executors

Methods that want to execute <xref:NanoByte.Common.Undo.IUndoCommand>s should take an <xref:NanoByte.Common.Undo.ICommandExecutor> as an input parameter.  
To execute an <xref:NanoByte.Common.Undo.IUndoCommand>, pass it to the [ICommandExecutor.Execute()](xref:NanoByte.Common.Undo.ICommandExecutor#NanoByte_Common_Undo_ICommandExecutor_Execute_NanoByte_Common_Undo_IUndoCommand_) method. This will then internally call [IUndoCommand.Execute()](xref:NanoByte.Common.Undo.IUndoCommand#NanoByte_Common_Undo_IUndoCommand_Execute) and record the command for later undo operations.

The class <xref:NanoByte.Common.Undo.CommandManager`1> implements the <xref:NanoByte.Common.Undo.ICommandExecutor> interface. You can expose the `.Undo()` and `.Redo()` methods of this class to your users in your GUI.
