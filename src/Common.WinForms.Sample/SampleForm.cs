using System.Windows.Forms;

namespace Common.WinForms.Sample
{
    public partial class SampleForm : Form
    {
        public SampleForm()
        {
            InitializeComponent();

            resettablePropertyGrid.SelectedObject = taskControl;
        }
    }
}
