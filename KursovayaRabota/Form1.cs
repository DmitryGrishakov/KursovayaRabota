using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KursovayaRabota
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void фирмыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Firms firms = new Firms();
            firms.ShowDialog(this);
        }

        private void магазиныToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stores stores = new Stores();
            stores.ShowDialog(this);
        }

        private void поставкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Deliveries deliveries = new Deliveries();
            deliveries.ShowDialog(this);
        }

        private void товарыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Products products = new Products();
            products.ShowDialog(this);
        }
    }
}
