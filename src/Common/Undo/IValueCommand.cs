// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Undo
{
    /// <summary>
    /// A command that exposes the value it will set.
    /// </summary>
    public interface IValueCommand : IUndoCommand
    {
        object? Value { get; }
    }
}
