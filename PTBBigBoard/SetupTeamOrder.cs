using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PTBBigBoard
{
    public partial class SetupTeamOrder : Form
    {
        public SetupTeamOrder()
        {
            InitializeComponent();
        }

        private void BtnCreateOrderTestList_Click(object sender, EventArgs e)
        {
            File.Delete("SetupDraftOrder.txt");
            for (int i = 1; i < 17; i++)
            {
                TextBox currentTeamText = this.Controls["textBox" + i.ToString()] as TextBox;
                string strTeamName = currentTeamText.Text.ToUpper();
                try
                {
                    using (StreamWriter writer =
                    new StreamWriter("SetupDraftOrder.txt", true))
                    {
                        writer.WriteLine(strTeamName);

                    }
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error writing to SetupDraftOrder.txt :: " + ex.Message);
                }
            }
        }
    }
}
