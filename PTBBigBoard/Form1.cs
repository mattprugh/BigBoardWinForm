using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
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
        string[] arrTeamOrder = new string[16];
        DataTable _dtAllPlayer = new DataTable();
        DataTable _dtMatchedPlayers = new DataTable();
        string[] _arrNFLTeams = { "ARI", "ATL", "BAL", "BUF", "CAR", "CHI", "CIN", "CLE", "DAL", "DEN", "DET", "GB", "HOU", "IND", "JAC", "KC", "LAC", "LAR", "MIA", "MIN", "NE", "NO", "NYG", "NYJ", "OAK", "PHI", "PIT", "SEA", "SF", "TB", "TEN", "WAS" };
        string _strSelectPlayerExpression;
        string _strPosition;
        string _strNFLTeam;

        public frmBigBoardMain()
        {
            InitializeComponent();
            SetPickOrder();
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
            if (File.Exists("DepthChart.xlsx"))
            {
                _dtAllPlayer.Merge(LoadPlayers());
            }
            else{
                MessageBox.Show("Please put the DepthChart.xlsx file in the same folder as the BigBoard.exe file. \n This file is generated using google sheets and importing the Huddle depth chart.");
                Application.Exit();
            }
            MessageBox.Show("Don't close this wonderful app.");
            if(File.Exists("SetupDraftOrder.txt"))
            {
                SetPickOrder();
            }
            else {
                MessageBox.Show("No draft pick order defined. Use the Set Order button in lower left of the BigBoard window to set.");
            }
            foreach(string strNFLTeam in _arrNFLTeams)
            {
                comboNFLTeam.Items.Add(strNFLTeam);
            }
            

        }

        private void button1_Click(object sender, EventArgs e) //recover picks from last exit
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

        private void ListPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            listPlayersMatched.Items.Clear();
            _strPosition = listPosition.GetItemText(listPosition.SelectedItem);
            if(_strNFLTeam != null)
            {
                _strSelectPlayerExpression = "Team = '" + _strNFLTeam + "' AND " + "Position = '" + _strPosition+"'";
                _dtMatchedPlayers = _dtAllPlayer.Select(_strSelectPlayerExpression).CopyToDataTable();
                for (int i = 0; i < _dtMatchedPlayers.Rows.Count; i++)
                {
                    listPlayersMatched.Items.Add(_dtMatchedPlayers.Rows[i][1].ToString());
                }
            }

        }
        private void ComboNFLTeam_SelectedIndexChanged(object sender, EventArgs e)
        {
            listPlayersMatched.Items.Clear();
            _strNFLTeam = comboNFLTeam.Text;
            if(_strPosition != null)
            {
                _strSelectPlayerExpression = "Team = '" + _strNFLTeam + "' AND " + "Position = '" + _strPosition+"'";
                _dtMatchedPlayers = _dtAllPlayer.Select(_strSelectPlayerExpression).CopyToDataTable();
                for (int i = 0; i < _dtMatchedPlayers.Rows.Count; i++)
                {
                    listPlayersMatched.Items.Add(_dtMatchedPlayers.Rows[i][1].ToString());
                }
            }
        }
        private void ComboNFLTeam_DropDownClosed(object sender, EventArgs e)
        {
            listPlayersMatched.Items.Clear();
            _strNFLTeam = comboNFLTeam.Text;
            if (_strPosition != null)
            {
                _strSelectPlayerExpression = "Team = '" + _strNFLTeam + "' AND " + "Position = '" + _strPosition + "'";
                _dtMatchedPlayers = _dtAllPlayer.Select(_strSelectPlayerExpression).CopyToDataTable();
                for (int i = 0; i < _dtMatchedPlayers.Rows.Count; i++)
                {
                    listPlayersMatched.Items.Add(_dtMatchedPlayers.Rows[i][1].ToString());
                }
            }
        }
        private void BtnSetPickOrder_Click(object sender, EventArgs e)
        {//open form for entering draft order
            
            new SetupTeamOrder().ShowDialog();


            //now load the labels and team combo box
            SetPickOrder();


        }
        private void SetPickOrder()
        {
            try
            {

                arrTeamOrder = File.ReadAllLines("SetupDraftOrder.txt");
                string strCurrentPickLabel;
                for (int i = 1; i < 17; i++)
                {
                    strCurrentPickLabel = "lblTeam" + i.ToString();
                    Label lblTeamText = (Label)this.Controls.Find(strCurrentPickLabel, true)[0];
                    lblTeamText.Text = arrTeamOrder[i - 1].ToString();
                    comboBoxTeam.Items.Add(arrTeamOrder[(i - 1)].ToString());
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening file SetupDraftOrder.txt ::" + ex.Message);
            }
        }
        private DataTable LoadPlayers()
        {

            //read in the scraped depth chart excel file

            OleDbConnection con = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source='DepthChart.xlsx'; Extended Properties='Excel 12.0 Xml;HDR = YES;IMEX=1'"); //; Extended Properties='Excel 12.0 Xml;HDR = YES;IMEX=1'
            OleDbCommand olecmd = new OleDbCommand("select * from [Sheet1$]", con);
            DataTable dtPlayersExpanded = new DataTable();
            OleDbDataAdapter adp = new OleDbDataAdapter(olecmd);
            con.Open();
            adp.Fill(dtPlayersExpanded);
            con.Close();

            //grdPlayers.DataContext = dtPlayersExpanded.DefaultView;

            DataTable dtPlayersFlattened = new DataTable();
            dtPlayersFlattened.Columns.Add("Team");
            dtPlayersFlattened.Columns.Add("Player Name");
            dtPlayersFlattened.Columns.Add("Position");


            foreach (DataRow dr in dtPlayersExpanded.Rows)
            {
                string strTeamName = dr[0].ToString().Replace("*", "");
                string strQBs = dr[1].ToString();
                string strRBs = dr[2].ToString();
                string strWRs = dr[3].ToString();
                string strTEs = dr[4].ToString();
                string strKs = dr[5].ToString();

                string[] arrQBs = strQBs.Split("\n".ToCharArray());
                dtPlayersFlattened.Merge(GetPlayerRows(arrQBs, strTeamName, "QB"));

                string[] arrRBs = strRBs.Split("\n".ToCharArray());
                dtPlayersFlattened.Merge(GetPlayerRows(arrRBs, strTeamName, "RB"));

                string[] arrWRs = strWRs.Split("\n".ToCharArray());
                dtPlayersFlattened.Merge(GetPlayerRows(arrWRs, strTeamName, "WR"));

                string[] arrTEs = strTEs.Split("\n".ToCharArray());
                dtPlayersFlattened.Merge(GetPlayerRows(arrTEs, strTeamName, "TE"));

                string[] arrKs = strKs.Split("\n".ToCharArray());
                dtPlayersFlattened.Merge(GetPlayerRows(arrKs, strTeamName, "K"));

                //add the defense
                DataRow drD = dtPlayersFlattened.NewRow();
                drD["Team"] = dr[0].ToString().Replace("*", "");
                drD["Player Name"] = strTeamName + " Defense";
                drD["Position"] = "D/ST";
                dtPlayersFlattened.Rows.Add(drD);
            }
            //grdPlayers.DataContext = dtPlayersFlattened.DefaultView;
            return dtPlayersFlattened;
        }
        public DataTable GetPlayerRows(Array arrOfPosition, string strTeamName, string strPosition)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Team");
            dt.Columns.Add("Player Name");
            dt.Columns.Add("Position");
            foreach (string strNameOfPlayer in arrOfPosition)
            {

                DataRow drPlayer = dt.NewRow();

                drPlayer["Team"] = strTeamName;
                drPlayer["Player Name"] = strNameOfPlayer;
                drPlayer["Position"] = strPosition;
                dt.Rows.Add(drPlayer);
            }
            return dt;
        }

        private void ListPlayersMatched_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strPlayerNameToPost = listPlayersMatched.GetItemText(listPlayersMatched.SelectedItem);
            string strLastName;
            string strFullName;

            if (strPlayerNameToPost.Length > 15)
            {
                
                var strNames = strPlayerNameToPost.Split(' ');
                if (strNames.Length == 3)
                {
                    strLastName = strNames[1].ToString() + " " + strNames[2].ToString();
                }
                else
                {
                    strLastName = strNames[1];
                }
                string strInit = strNames[0].Substring(0, 1);
                strFullName = strInit + " " + strLastName;
                if(strFullName.Length > 20) { strFullName.Substring(0, 20); }
                
            }
            else
            {
                strFullName = strPlayerNameToPost;
            }
            txtPlayerToAdd.Text = strFullName;
        }
    }
    
}
