// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.EtoControls;

/// <summary>
/// Displays a list of <see cref="INamed"/>s objects in a <see cref="TreeGridView"/> with incremental search.
/// An automatic hierarchy is generated based on a <see cref="Separator"/> character.
/// </summary>
/// <typeparam name="T">The type of <see cref="INamed"/> object to list.
/// Special support for types implementing <see cref="IHighlightColor"/> and/or <see cref="IContextMenu"/>.</typeparam>
public sealed class FilteredTreeView<T> : Panel where T : INamed
{
    #region Events
    /// <summary>
    /// Occurs whenever <see cref="SelectedEntry"/> has been changed.
    /// </summary>
    public event EventHandler? SelectedEntryChanged;

    private void OnSelectedEntryChanged()
    {
        if (!_suppressEvents) SelectedEntryChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Occurs when the user has confirmed the <see cref="SelectedEntry"/> via double-clicking or pressing Enter.
    /// </summary>
    public event EventHandler? SelectionConfirmed;

    private void OnSelectionConfirmed()
    {
        // Only confirm if the user actually selected something
        if (!_suppressEvents && SelectionConfirmed != null && _selectedEntry != null) SelectionConfirmed(this, EventArgs.Empty);
    }

    /// <summary>
    /// Occurs whenever the content of <see cref="CheckedEntries"/> has changed.
    /// </summary>
    public event EventHandler? CheckedEntriesChanged;

    private void OnCheckedEntriesChanged()
    {
        if (!_suppressEvents) CheckedEntriesChanged?.Invoke(this, EventArgs.Empty);
    }
    #endregion

    #region Variables
    /// <summary>Suppress the execution of <see cref="SelectedEntryChanged"/>.</summary>
    private bool _suppressEvents;

    private readonly SearchBox _searchBox;
    private readonly TreeGridView _treeView;

    /// <summary>Maps accumulated partial names to tree nodes within the current <see cref="UpdateList"/> pass.</summary>
    private readonly Dictionary<string, Node> _lookup = [];

    private Font? _boldFont;
    #endregion

    #region Properties
    /// <summary>
    /// Toggle the visibility of the search box.
    /// </summary>
    public bool ShowSearchBox { get => _searchBox.Visible; set => _searchBox.Visible = value; }

    private NamedCollection<T>? _nodes;

    /// <summary>
    /// The <see cref="INamed"/> (and optionally <see cref="IContextMenu"/>) objects to be listed in the tree.
    /// </summary>
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
    /// The <see cref="INamed"/> object currently selected in the <see cref="TreeGridView"/>; <c>null</c> for no selection.
    /// </summary>
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

    private readonly HashSet<T> _checkedEntries = [];

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
    public char Separator
    {
        get => _separator;
        set
        {
            _separator = value;
            UpdateList();
        }
    }

    private bool _checkBoxes;

    /// <summary>
    /// Controls whether check boxes are displayed for every entry.
    /// </summary>
    /// <see cref="CheckedEntries"/>
    public bool CheckBoxes
    {
        get => _checkBoxes;
        set
        {
            if (_checkBoxes == value) return;
            _checkBoxes = value;
            SetupColumns();
        }
    }
    #endregion

    #region Constructor
    public FilteredTreeView()
    {
        _searchBox = new SearchBox {PlaceholderText = Resources.Search};
        _searchBox.TextChanged += (_, _) => UpdateList();

        _treeView = new TreeGridView {ShowHeader = false};
        _treeView.SelectedItemChanged += OnSelectedItemChanged;
        _treeView.Activated += (_, _) => OnSelectionConfirmed();
        _treeView.CellEdited += OnCellEdited;
        _treeView.CellFormatting += OnCellFormatting;
        _treeView.MouseDown += OnMouseDown;
        SetupColumns();

        Content = new TableLayout
        {
            Rows =
            {
                new TableRow(_searchBox),
                new TableRow(_treeView) {ScaleHeight = true}
            }
        };
    }
    #endregion

    //--------------------//

    #region Columns
    private void SetupColumns()
    {
        _treeView.Columns.Clear();

        if (_checkBoxes)
        {
            _treeView.Columns.Add(new GridColumn
            {
                Editable = true,
                DataCell = new CheckBoxCell {Binding = Binding.Property((Node n) => n.Checked)}
            });
        }

        _treeView.Columns.Add(new GridColumn
        {
            Expand = true,
            DataCell = new TextBoxCell {Binding = Binding.Property((Node n) => n.Text)}
        });
    }
    #endregion

    #region Selection
    private void OnSelectedItemChanged(object? sender, EventArgs e)
    {
        // Don't use the property, to prevent a loop
        _selectedEntry = _treeView.SelectedItem is Node node ? node.Entry : default;
        OnSelectedEntryChanged();
    }
    #endregion

    #region Update
    /// <summary>
    /// Updates the filtered <see cref="TreeGridView"/> representation of <see cref="Nodes"/>.
    /// </summary>
    /// <remarks>Called automatically internally.</remarks>
    public void UpdateList(object? sender = null)
    {
        // Suppress events to prevent infinite loops
        _suppressEvents = true;

        var root = new TreeGridItemCollection();
        _lookup.Clear();
        Node? selectedNode = null;

        if (_nodes != null)
        {
            foreach (T entry in _nodes)
            {
                // The currently selected entry and checked entries are always visible
                // Note: Compare name to handle cloned entries
                if ((_selectedEntry != null && entry.Name == _selectedEntry.Name) || _checkedEntries.Contains(entry))
                {
                    _selectedEntry = entry; // Fix problems that might arise from using clones
                    selectedNode = AddTreeNode(root, entry);
                }
                // List all nodes if there is no filter
                else if (string.IsNullOrEmpty(_searchBox.Text))
                    AddTreeNode(root, entry);
                // Only list nodes that match the filter
                else if (entry.Name.ContainsIgnoreCase(_searchBox.Text))
                    AddTreeNode(root, entry);
            }

            // Automatically expand nodes based on the filtering
            if (!string.IsNullOrEmpty(_searchBox.Text))
                ExpandNodes(root, fullNameExpand: true);
        }

        _treeView.DataStore = root;
        if (selectedNode != null) _treeView.SelectedItem = selectedNode;

        // Restore events at the end
        _suppressEvents = false;
    }
    #endregion

    #region Tree node helper
    /// <summary>
    /// Adds a new node to the tree.
    /// </summary>
    /// <param name="root">The root collection to add top-level nodes to.</param>
    /// <param name="entry">The <typeparamref name="T"/> to create the entry for.</param>
    /// <returns>The newly created <see cref="Node"/>.</returns>
    private Node AddTreeNode(TreeGridItemCollection root, T entry)
    {
        // Split into hierarchy levels
        string[] nameSplit = entry.Name.Split(_separator);

        // Start off at the top-level
        var subTree = root;
        Node? parentNode = null;

        // Try to use existing nodes for parents, create new ones if missing
        string partialName = "";
        for (int i = 0; i < nameSplit.Length - 1; i++)
        {
            partialName += nameSplit[i];
            if (!_lookup.TryGetValue(partialName, out var node))
            {
                node = new Node {Key = partialName, Text = nameSplit[i], ParentNode = parentNode};
                subTree.Add(node);
                _lookup[partialName] = node;
            }
            parentNode = node;
            subTree = node.Children;
            partialName += _separator;
        }

        // Create node for actual entry using last part as visible text
        var finalNode = new Node {Key = entry.Name, Text = nameSplit[^1], Entry = entry, ParentNode = parentNode};
        if (_checkedEntries.Contains(entry)) finalNode.Checked = true;
        subTree.Add(finalNode);
        _lookup[entry.Name] = finalNode;
        return finalNode;
    }
    #endregion

    #region Expand node helper
    /// <summary>
    /// Automatically expand nodes based on the <see cref="_searchBox"/> filtering.
    /// </summary>
    /// <param name="subTree">The current collection used in recursion.</param>
    /// <param name="fullNameExpand">Shall a search for the full name of a tag allow it to be expanded?</param>
    private void ExpandNodes(TreeGridItemCollection subTree, bool fullNameExpand)
    {
        foreach (var node in subTree.OfType<Node>())
        {
            // Checked nodes and nodes with matches in the last part of their name (the displayed text) shall always be visible
            if ((node.Checked ?? false) || node.Text.ContainsIgnoreCase(_searchBox.Text)) EnsureVisible(node);

            // Parent nodes with full name match shall be expanded
            if (fullNameExpand && node.Key.ContainsIgnoreCase(_searchBox.Text))
            {
                EnsureVisible(node);
                node.Expanded = true;

                // ... but not beyond the first recursion level
                ExpandNodes(node.Children, fullNameExpand: false);
            }
            else ExpandNodes(node.Children, fullNameExpand: fullNameExpand);
        }
    }

    /// <summary>
    /// Expands all ancestors of a node so that it becomes visible.
    /// </summary>
    private static void EnsureVisible(Node node)
    {
        for (var parent = node.ParentNode; parent != null; parent = parent.ParentNode)
            parent.Expanded = true;
    }
    #endregion

    #region Checkbox control
    private void OnCellEdited(object? sender, GridViewCellEventArgs e)
    {
        if (e.Item is not Node node) return;

        // Checking a parent will check all its children
        ApplyCheck(node, node.Checked ?? false);
        _treeView.ReloadItem(node);
        OnCheckedEntriesChanged();
    }

    /// <summary>
    /// Applies a check state to a node and all its descendants, maintaining <see cref="_checkedEntries"/>.
    /// </summary>
    private void ApplyCheck(Node node, bool value)
    {
        node.Checked = value;

        // Maintain a list of currently checked bottom-level entries
        if (node.Entry != null)
        {
            if (value) _checkedEntries.Add(node.Entry);
            else _checkedEntries.Remove(node.Entry);
        }

        foreach (var child in node.Children.OfType<Node>())
            ApplyCheck(child, value);
    }
    #endregion

    #region Highlight color
    private void OnCellFormatting(object? sender, GridCellFormatEventArgs e)
    {
        if (e.Item is not Node {Entry: IHighlightColor highlight}) return;

        var color = highlight.HighlightColor;
        if (color != default)
        {
            e.ForegroundColor = Color.FromArgb(color.R, color.G, color.B, color.A);
            e.Font = _boldFont ??= MakeBoldFont();
        }
    }

    private static Font MakeBoldFont()
    {
        var baseFont = SystemFonts.Default();
        return new Font(baseFont.Family, baseFont.Size, FontStyle.Bold);
    }
    #endregion

    #region Context menu
    private void OnMouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Buttons != MouseButtons.Alternate) return;
        if (_treeView.GetCellAt(e.Location).Item is not Node {Entry: IContextMenu provider} node) return;

        _treeView.SelectedItem = node;
        var menu = provider.GetContextMenu();
        if (menu != null)
        {
            e.Handled = true;
            menu.Show(_treeView);
        }
    }
    #endregion

    #region Disposal
    protected override void Dispose(bool disposing)
    {
        if (disposing) _boldFont?.Dispose();
        base.Dispose(disposing);
    }
    #endregion

    #region Node
    /// <summary>
    /// A single item in the <see cref="TreeGridView"/>.
    /// </summary>
    private sealed class Node : TreeGridItem
    {
        /// <summary>The accumulated full name used as a unique key.</summary>
        public string Key { get; init; } = "";

        /// <summary>The text displayed for this node.</summary>
        public string Text { get; set; } = "";

        /// <summary>The collection entry this node represents; <c>null</c> for intermediate hierarchy nodes.</summary>
        public T? Entry { get; init; }

        /// <summary>Whether this node's check box is checked.</summary>
        public bool? Checked { get; set; }

        /// <summary>The parent node; <c>null</c> for top-level nodes.</summary>
        public Node? ParentNode { get; init; }
    }
    #endregion
}
