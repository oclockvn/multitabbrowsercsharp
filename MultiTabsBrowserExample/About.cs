using System;
using System.Windows.Forms;

namespace MultiTabsBrowserExample
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();

            btnOK.Click += btnOK_Click;
        }

        void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
