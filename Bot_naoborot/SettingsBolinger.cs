using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bot_naoborot
{
    public partial class SettingsBolinger : Form
    {
        private BolingerBands bb;
        public SettingsBolinger(BolingerBands boling)
        {
            InitializeComponent();
            bb = boling;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            bb.depart = Convert.ToDouble(departure.Text);

            bb.top = Convert.ToDouble(top.Text);
            bb.bottom = Convert.ToDouble(bottom.Text);

            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
