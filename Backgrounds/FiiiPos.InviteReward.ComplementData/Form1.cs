using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FiiiPos.InviteReward.ComplementData
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            DataStartTime.MaxDate = DateTime.UtcNow;
            DataEndTime.MaxDate = DateTime.UtcNow;
            DataSaveTime.MaxDate = DateTime.UtcNow;

            DataStartTime.MinDate = new DateTime(2018, 10, 1);
            DataEndTime.MinDate = new DateTime(2018, 10, 1);
            DataSaveTime.MinDate = new DateTime(2018, 10, 1);

            LableTotalMessage.Visible = false;
            LableSuccessMessage.Visible = false;
            LableFaildMessage.Visible = false;
            LabelTotalCount.Visible = false;
            LabelSuccessCount.Visible = false;
            LabelFaildCount.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var startTime = DataStartTime.Value;
            var endTime = DataEndTime.Value;
            var saveTime = DataSaveTime.Value;

            LableTotalMessage.Text = "总计:";
            LableTotalMessage.Visible = true;
            LableSuccessMessage.Visible = true;
            LableFaildMessage.Visible = true;
            LabelTotalCount.Visible = true;
            LabelSuccessCount.Visible = true;
            LabelFaildCount.Visible = true;

            new InviteStart().Invite(startTime, endTime, saveTime, RefreshCount);

            LableTotalMessage.Text = "处理完毕";
            LableSuccessMessage.Visible = false;
            LableFaildMessage.Visible = false;
            LabelTotalCount.Visible = false;
            LabelSuccessCount.Visible = false;
            LabelFaildCount.Visible = false;
        }

        private void RefreshCount(Tuple<int, int, int> RunData)
        {
            LabelTotalCount.Text = RunData.Item1.ToString();
            LabelSuccessCount.Text = RunData.Item2.ToString();
            LabelFaildCount.Text = RunData.Item3.ToString();
            LabelTotalCount.Refresh();
            LabelSuccessCount.Refresh();
            LabelFaildCount.Refresh();
        }
    }
}
