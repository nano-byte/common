// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Windows.Forms;
using NanoByte.Common.Undo;

namespace NanoByte.Common.StructureEditor
{
    /// <summary>
    /// Information and callbacks for a specific entry in the structure.
    /// </summary>
    internal class EntryInfo
    {
        public readonly string Name;
        public readonly string Description;
        public readonly object Target;
        public readonly Func<Undo.ICommandExecutor, Control> GetEditorControl;
        public readonly Func<string> ToXmlString;
        public readonly Func<string, IValueCommand> FromXmlString;
        public readonly IUndoCommand RemoveCommand;

        public EntryInfo(string name, string description, object target, Func<Undo.ICommandExecutor, Control> getEditorControl, Func<string> toXmlString, Func<string, IValueCommand> fromXmlString, IUndoCommand removeCommand)
        {
            Name = name;
            Description = description;
            Target = target;
            GetEditorControl = getEditorControl;
            ToXmlString = toXmlString;
            FromXmlString = fromXmlString;
            RemoveCommand = removeCommand;
        }

        public override string ToString() => $"{Name}: {Target}";
    }
}
