// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Windows.Forms;

namespace NanoByte.Common.Controls
{
    /// <summary>
    /// Contains a single property grid for inspecting and manipulating the properties of an arbitrary object.
    /// </summary>
    public sealed partial class InspectionForm : Form
    {
        ///<summary>
        /// Creates a new inspection form.
        ///</summary>
        ///<param name="value">The object to be inspected.</param>
        public InspectionForm(object value)
        {
            InitializeComponent();

            propertyGrid.SelectedObject = value;
        }

        /// <summary>
        /// Displays a property grid for manipulating the properties of an object.
        /// </summary>
        /// <param name="value">The object to be inspected.</param>
        public static void Inspect(object value) => new InspectionForm(value).Show();
    }
}
