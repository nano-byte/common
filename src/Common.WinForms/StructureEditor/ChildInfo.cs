// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using NanoByte.Common.Undo;

namespace NanoByte.Common.StructureEditor
{
    /// <summary>
    /// Information and callbacks for a potential new child node in the structure.
    /// </summary>
    internal class ChildInfo
    {
        public readonly string Name;
        public readonly string Description;
        public readonly Func<IValueCommand> Create;

        public ChildInfo(string name, string description, Func<IValueCommand> create)
        {
            Name = name;
            Description = description;
            Create = create;
        }
    }
}
