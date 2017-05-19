using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace eShop
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            tssEmpName.Text += Config.EmplyeeName;
            tssToday.Text += DateTime.Today.ToString();
        }

        private void saleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSale fs = new frmSale();
            fs.Show();
            fs.MdiParent = this;
            fs.Focus();
        }

        private void productToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmProduct fp = new frmProduct();
            fp.Show();
            fp.MdiParent = this;
            fp.Focus();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            productToolStripMenuItem_Click(sender, e);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            saleToolStripMenuItem_Click(sender, e);
        }

        private void cascaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.Cascade);
        }

        private void tileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileVertical);
        }

        private void tileHorizentalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileHorizontal);
        }
    }
}
