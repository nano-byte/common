// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Runtime.InteropServices;

namespace NanoByte.Common.Controls;

/// <summary>
/// Displays a list of <see cref="INamed"/>s objects in a <see cref="TreeView"/> with incremental search.
/// An automatic hierarchy is generated based on a <see cref="Separator"/> character.
/// </summary>
/// <typeparam name="T">The type of <see cref="INamed"/> object to list.
/// Special support for types implementing <see cref="IHighlightColor"/> and/or <see cref="IContextMenu"/>.</typeparam>
[Description("Displays a list of INamed in a TreeView with incremental search."), Guid("5065F310-D0B3-4AD3-BBE5-B41D00D5F036")]
public sealed partial class FilteredTreeView<T> : UserControl
    // ReSharper disable once RedundantNotNullConstraint
    where T : notnull, INamed
{
    #region Events
    /// <summary>
    /// Occurs whenever <see cref="SelectedEntry"/> has been changed.
    /// </summary>
    [Description("Occurs whenever SelectedEntry has been changed.")]
    public event EventHandler? SelectedEntryChanged;

    private void OnSelectedEntryChanged()
    {
        if (!_suppressEvents && Visible) SelectedEntryChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Occurs when the user has confirmed the <see cref="SelectedEntry"/> via double-clicking or pressing Enter.
    /// </summary>
    [Description("Occurs when the user has confirmed the current selection via double-clicking or pressing Enter.")]
    public event EventHandler? SelectionConfirmed;

    private void OnSelectionConfirmed()
    {
        // Only confirm if the user actually selected something
        if (!_suppressEvents && Visible && SelectionConfirmed != null && _selectedEntry != null) SelectionConfirmed(this, EventArgs.Empty);
    }

    /// <summary>
    /// Occurs whenever the content of <see cref="CheckedEntries"/> has changed.
    /// </summary>
    [Description("Occurs whenever the content of CheckedEntries has changed.")]
    public event EventHandler? CheckedEntriesChanged;

    private void OnCheckedEntriesChanged()
    {
        if (!_suppressEvents && Visible) CheckedEntriesChanged?.Invoke(this, EventArgs.Empty);
    }
    #endregion

    #region Variables
    /// <summary>Suppress the execution of <see cref="SelectedEntryChanged"/>.</summary>
    private bool _suppressEvents;
    #endregion

    #region Properties
    /// <summary>
    /// Toggle the visibility of the search box.
    /// </summary>
    [DefaultValue(true), Description("Toggle the visibility of the search box."), Category("Appearance")]
    public bool ShowSearchBox { get => textSearch.Visible; set => textSearch.Visible = value; }

    private NamedCollection<T>? _nodes;

    /// <summary>
    /// The <see cref="INamed"/> (and optionally <see cref="IContextMenu"/>) objects to be listed in the tree.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "This control is supposed to represent a live and mutable collection")]
    public NamedCollection<T>? Nodes
    {
        get => _nodes;
        set
        {
            // Keep track of any changes within the collection
            if (_nodes != null) _nodes.CollectionChanged -= UpdateList;
            _nodes = value;
            if (_nodes != null) _nodes.CollectionChanged += UpdateList;

            _checkedEntries.Clear();
            UpdateList();
        }
    }

    private T? _selectedEntry;

    /// <summary>
    /// The <see cref="INamed"/> object currently selected in the <see cref="TreeView"/>; <c>null</c> for no selection.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public T? SelectedEntry
    {
        get => _selectedEntry;
        set
        {
            _selectedEntry = value;
            UpdateList();
            OnSelectedEntryChanged();
        }
    }

    private readonly HashSet<T> _checkedEntries = new();

    /// <summary>
    /// Returns a list of all <see cref="INamed"/> objects currently marked with a check box.
    /// </summary>
    /// <remarks>Does NOT create a defensive copy. Take care to only add valid elements when modifying. Call <see cref="UpdateList"/> after changing.</remarks>
    /// <see cref="CheckBoxes"/>
    public ICollection<T> CheckedEntries => _checkedEntries;

    private char _separator = Named.TreeSeparator;

    /// <summary>
    /// The character used to split <see cref="INamed.Name"/>s into tree levels.
    /// </summary>
    [DefaultValue(Named.TreeSeparator), Description("The character used to split names into tree levels.")]
    public char Separator
    {
        get => _separator;
        set
        {
            _separator = value;
            UpdateList();
        }
    }

    /// <summary>
    /// Controls whether check boxes are displayed for every entry.
    /// </summary>
    /// <see cref="CheckedEntries"/>
    [DefaultValue(false), Description("Controls whether check boxes are displayed for every entry.")]
    public bool CheckBoxes { get => treeView.CheckBoxes; set => treeView.CheckBoxes = value; }
    #endregion

    #region Constructor
    public FilteredTreeView()
    {
        InitializeComponent();
        textSearch.HintText = Resources.Search;
    }
    #endregion

    //--------------------//

    #region Search control
    private void textSearch_TextChanged(object? sender, EventArgs e) => UpdateList();
    #endregion

    #region TreeView control
    private void treeView_DoubleClick(object? sender, EventArgs e) => OnSelectionConfirmed();

    private void treeView_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter) OnSelectionConfirmed();
    }

    private void treeView_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        string name = treeView.SelectedNode.Name;

        // Don't use the property, to prevent a loop
        _selectedEntry = _nodes != null && _nodes.Contains(name) ? _nodes[name] : default;
        OnSelectedEntryChanged();
    }

    /// <summary>
    /// Updates the filtered <see cref="TreeView"/> representation of <see cref="Nodes"/>.
    /// </summary>
    /// <remarks>Called automatically internally.</remarks>
    public void UpdateList(object? sender = null)
    {
        // Suppress events to prevent infinite loops
        _suppressEvents = true;

        treeView.Nodes.Clear();
        if (_nodes != null)
        {
            foreach (T entry in _nodes)
            {
                // The currently selected entry and checked entries are always visible
                // Note: Compare name to handle cloned entries
                if ((_selectedEntry != null && entry.Name == _selectedEntry.Name) || _checkedEntries.Contains(entry))
                {
                    _selectedEntry = entry; // Fix problems that might arise from using clones
                    treeView.SelectedNode = AddTreeNode(entry);
                }
                // List all nodes if there is no filter
                else if (string.IsNullOrEmpty(textSearch.Text))
                    AddTreeNode(entry);
                // Only list nodes that match the filter
                else if (entry.Name.ContainsIgnoreCase(textSearch.Text))
                    AddTreeNode(entry);
            }

            // Automatically expand nodes based on the filtering
            if (!string.IsNullOrEmpty(textSearch.Text))
                ExpandNodes(treeView.Nodes, fullNameExpand: true);
        }

        // Restore events at the end
        _suppressEvents = false;
    }
    #endregion

    #region TreeView node helper
    /// <summary>
    /// Adds a new node to <see cref="treeView"/>.
    /// </summary>
    /// <param name="entry">The <typeparamref name="T"/> to create the entry for.</param>
    /// <returns>The newly created <see cref="TreeNode"/>.</returns>
    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
    private TreeNode AddTreeNode(T entry)
    {
        // Split into hierarchy levels
        string[] nameSplit = entry.Name.Split(_separator);

        // Start off at the top-level
        TreeNodeCollection subTree = treeView.Nodes;

        // Try to use existing nodes for parents, create new ones if missing
        string partialName = "";
        for (int i = 0; i < nameSplit.Length - 1; i++)
        {
            partialName += nameSplit[i];
            TreeNode node = subTree[partialName] ?? subTree.Add(partialName, nameSplit[i]);
            subTree = node.Nodes;
            partialName += _separator;
        }

        // Create node for actual entry using last part as visible text
        TreeNode finalNode = subTree.Add(entry.Name, nameSplit.Last());
        if (_checkedEntries.Contains(entry)) finalNode.Checked = true;

        // Apply the highlighting color if one is set
        var color = (entry as IHighlightColor)?.HighlightColor ?? default;
        if (color != default)
        {
            finalNode.ForeColor = color;
            finalNode.NodeFont = new Font(treeView.Font, FontStyle.Bold);
        }

        // Attach the context menu if one is set
        var contextMenu = (entry as IContextMenu)?.GetContextMenu();
        if (contextMenu != null) finalNode.ContextMenuStrip = contextMenu;

        return finalNode;
    }
    #endregion

    #region TreeView expand node helper
    /// <summary>
    /// Automatically expand nodes based on the <see cref="textSearch"/> filtering
    /// </summary>
    /// <param name="subTree">The current <see cref="TreeNodeCollection"/> used in recursion</param>
    /// <param name="fullNameExpand">Shall a search for the full name of a tag allow it to be expanded?</param>
    private void ExpandNodes(TreeNodeCollection subTree, bool fullNameExpand)
    {
        foreach (var node in subTree.OfType<TreeNode>())
        {
            // Checked nodes and nodes with matches in the last part of their name (the displayed text) shall always be visible
            if (node.Checked || node.Text.ContainsIgnoreCase(textSearch.Text)) node.EnsureVisible();

            // Parent nodes with full name match shall be expanded
            if (fullNameExpand && node.Name.ContainsIgnoreCase(textSearch.Text))
            {
                node.EnsureVisible();
                node.Expand();

                // ... but not beyond the first recursion level
                ExpandNodes(node.Nodes, fullNameExpand: false);
            }
            else ExpandNodes(node.Nodes, fullNameExpand: fullNameExpand);
        }
    }
    #endregion

    #region Checkbox control
    private void treeView_AfterCheck(object? sender, TreeViewEventArgs e)
    {
        // Checking a parent will check all its children
        foreach (var node in e.Node!.Nodes.OfType<TreeNode>())
            node.Checked = e.Node.Checked;

        // Maintain a list of currently checked bottom-level entries
        if (Nodes != null && Nodes.Contains(e.Node.Name))
        {
            T entry = Nodes[e.Node.Name];
            if (e.Node.Checked) _checkedEntries.Add(entry);
            else _checkedEntries.Remove(entry);
            OnCheckedEntriesChanged();
        }
    }
    #endregion
}
