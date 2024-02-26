﻿// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Reflection;
using NanoByte.Common.Net;
using Spectre.Console.Rendering;

namespace NanoByte.Common;

/// <summary>
/// Helper methods for ANSI console rendering.
/// </summary>
public static class AnsiCli
{
    /// <summary>
    /// Used to write to the standard error stream.
    /// </summary>
    public static IAnsiConsole Error { get; } = AnsiConsole.Create(new AnsiConsoleSettings {Out = new AnsiConsoleOutput(Console.Error)});

    /// <summary>
    /// Displays a prompt to the user.
    /// </summary>
    /// <typeparam name="T">The prompt result type.</typeparam>
    /// <param name="prompt">The prompt to display.</param>
    /// <param name="cancellationToken">Used to cancel the prompt.</param>
    /// <returns>The prompt input result.</returns>
    public static T Prompt<T>(TextPrompt<T> prompt, CancellationToken cancellationToken)
    {
        var task = Task.Run(() => Error.Prompt(prompt), cancellationToken);
        task.Wait(cancellationToken);
        return task.Result;
    }

    /// <summary>
    /// Formats text as a title.
    /// </summary>
    public static IRenderable Title(string title)
        => new Paragraph(title, new Style(decoration: Decoration.Bold | Decoration.Underline)).Append(Environment.NewLine);

    /// <summary>
    /// Formats data as a table.
    /// </summary>
    /// <param name="data">The data to format.</param>
    public static IRenderable Table<T>(IEnumerable<T> data)
    {
        var table = new Table().RoundedBorder();
        var getters = new List<MethodInfo>();

        foreach (var property in typeof(T).GetProperties())
        {
            if (property.GetIndexParameters().Length != 0) continue;
            if (property.Name == nameof(IHighlightColor.HighlightColor)) continue;
            if (property.GetMethod == null) continue;
            var browsable = property.GetCustomAttribute<BrowsableAttribute>(inherit: true);
            if (browsable?.Browsable == false) continue;
            var displayName = property.GetCustomAttribute<DisplayNameAttribute>(inherit: true);

            table.AddColumn(new TableColumn(displayName?.DisplayName ?? property.Name));
            getters.Add(property.GetMethod);
        }

        foreach (var row in data)
        {
            var color = (row as IHighlightColor)?.HighlightColor ?? default;
            var foreground = (color == default)
                ? Color.Default
                : new Color(color.R, color.G, color.B);

            table.AddRow(getters.Select(getter =>
            {
                var value = getter.Invoke(row, []);
                return new Text(
                    value?.ToString() ?? "",
                    new Style(foreground, link: (value as Uri)?.ToStringRfc()));
            }));
        }

        return table;
    }

    /// <summary>
    /// Formats data as a tree.
    /// </summary>
    /// <param name="data">The data to show as nodes in the tree.</param>
    /// <param name="separator">The character used to split <see cref="INamed.Name"/>s into tree levels.</param>
    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
    public static IRenderable Tree<T>(NamedCollection<T> data, char separator = Named.TreeSeparator)
        where T : INamed
    {
        static IRenderable Node(T entry) => Table(new NamedCollection<T> {entry});

        // Always use first entry as root
        var first = data[0];
        data.RemoveAt(0);
        var tree = new Tree(Node(first));
        var nodeLookup = new Dictionary<string, IHasTreeNodes> {[first.Name] = tree};

        foreach (var entry in data)
        {
            // Start off at the top-level
            IHasTreeNodes subTree = tree;

            // Try to use existing nodes for parents, create simple text nodes if missing
            string[] nameSplit = entry.Name.Split(separator);
            string partialName = "";
            for (int i = 0; i < nameSplit.Length; i++)
            {
                partialName += nameSplit[i];
                subTree = nodeLookup.GetOrAdd(partialName, () => (i == nameSplit.Length - 1)
                    ? subTree.AddNode(Node(entry)) // Node for actual entry
                    : subTree.AddNode(nameSplit[i])); // Intermediate node
                partialName += separator;
            }
        }

        return tree;
    }
}
