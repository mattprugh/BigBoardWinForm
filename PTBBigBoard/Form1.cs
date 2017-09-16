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
    public partial class frmBigBoardMain : Form
    {
        int _intRound = 1;
        int _intPickThisRound = 1;
        int _intTotalPickNbr = 1;

        public frmBigBoardMain()
        {
            InitializeComponent();
        }

        private void btnAddPick_Click(object sender, EventArgs e)
        {
            string strName = txtPlayerToAdd.Text;
            string strPosition = listPosition.GetItemText(listPosition.SelectedItem);

            writeThePick(strName, strPosition);

            writeRecovery(strName, strPosition);

            //moving right along .... increment counters
            _intPickThisRound++;
            _intTotalPickNbr++;
            // and add to round if it is pick 17 cause it is a new round now and reset pick this round to 1
            if (_intPickThisRound == 17)
            {
                _intRound++;
                _intPickThisRound = 1;
            }
        }
        private void writeRecovery(string strName, string strPosition)
        {             //write to text file for recovery cause 
            try
            {
                using (StreamWriter writer =
                new StreamWriter("draftorder.txt", true))
                {
                    writer.WriteLine(_intRound + "," + _intPickThisRound.ToString() + "," + strName + "," + strPosition);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error writing to draftorder.txt :: " + ex.Message);
            }
        }
        private void writeThePick(string strName, string strPosition)
        {
            int intLabelNbr = 0;

            if (_intRound % 2 != 1)
            {
                intLabelNbr = 16 * _intRound - (_intPickThisRound - 1);
            }
            else
            {
                intLabelNbr = _intTotalPickNbr;
            }
            var labels = Controls.Find("label" + intLabelNbr, true);
            if (labels.Length > 0)
            {
                var label = (Label)labels[0];
                label.Text = strName;
                switch (strPosition)
                {
                    case "QB":
                        label.BackColor = Color.LightYellow;
                        break;
                    case "RB":
                        label.BackColor = Color.LightSalmon;
                        break;
                    case "WR":
                        label.BackColor = Color.LightSkyBlue;
                        break;
                    case "TE":
                        label.BackColor = Color.LightGreen;
                        break;
                    case "D/ST":
                        label.BackColor = Color.LightGray;
                        break;
                    case "K":
                        label.BackColor = Color.LightPink;
                        break;
                }
            }
            ////highlight next team label with red border
            //int intNextLabelNbr = intLabelNbr++;
            //var nextLabel = Controls.Find("label" + intNextLabelNbr, true);
            //if (labels.Length > 0)
            //{
            //    var label = (Label)labels[0];
            //}
        }
        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {
            MessageBox.Show("Don't close this wonderful app.");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("You done messed up - reloading file");
            int intHighestRound = 0;
            int intHighestPickInRound = 0;
            DataTable dt = new DataTable();
            try
            {
                using (StreamReader sr = new StreamReader("draftorder.txt"))
                {
                    string[] headers = new string[] { "Round", "PickInRd", "Name", "Pos" };
                    foreach (string header in headers)
                    {
                        dt.Columns.Add(header);
                    }
                    while (!sr.EndOfStream)
                    {
                        string[] rows = sr.ReadLine().Split(',');
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            dr[i] = rows[i];
                        }
                        dt.Rows.Add(dr);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening file draftorder.txt ::" + ex.Message);
            }
            foreach (DataRow dr in dt.Rows)
            {
                _intRound = int.Parse(dr["Round"].ToString());
                _intPickThisRound = int.Parse(dr["PickInRd"].ToString());
                _intTotalPickNbr = (_intRound - 1) * 16 + _intPickThisRound;
                string strName = dr["Name"].ToString();
                string strPos = dr["Pos"].ToString();
                writeThePick(strName, strPos);
            }
            //moving right along .... increment counters
            _intPickThisRound++;
            _intTotalPickNbr++;
            // and add to round if it is pick 17 cause it is a new round now and reset pick this round to 1
            if (_intPickThisRound == 17)
            {
                _intRound++;
                _intPickThisRound = 1;
            }
            _intTotalPickNbr = (_intRound - 1) * 16 + _intPickThisRound;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int intReturnToRd = _intRound;
            int intReturnToPickThisRd = _intPickThisRound;

            _intRound = comboBoxRound.SelectedIndex + 1;

            if (_intRound % 2 != 0)
            {
                _intPickThisRound = comboBoxTeam.SelectedIndex + 1;
            }
            else {
                _intPickThisRound = 16 - comboBoxTeam.SelectedIndex;
            }

            _intTotalPickNbr = (_intRound - 1) * 16 + _intPickThisRound;
            string strName = txtPlayerToAdd.Text;
            string strPosition = listPosition.GetItemText(listPosition.SelectedItem);

            writeThePick(strName, strPosition);
            writeRecovery(strName, strPosition);

            _intRound = intReturnToRd;
            _intPickThisRound = intReturnToPickThisRd;
            _intTotalPickNbr = (_intRound - 1) * 16 + _intPickThisRound;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int intReturnToRd = _intRound;
            int intReturnToPickThisRd = _intPickThisRound;

            _intRound = comboBoxRound.SelectedIndex + 1;

            if (_intRound % 2 != 0)
            {
                _intPickThisRound = comboBoxTeam.SelectedIndex + 1;
            }
            else {
                _intPickThisRound = 16 - comboBoxTeam.SelectedIndex;
            }
            _intTotalPickNbr = (_intRound - 1) * 16 + _intPickThisRound;
            MessageBox.Show("Set the pick to number " + _intPickThisRound.ToString());

        }
    }
}
