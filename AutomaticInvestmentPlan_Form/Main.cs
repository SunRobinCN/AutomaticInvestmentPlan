using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutomaticInvestmentPlan_Comm;
using AutomaticInvestmentPlan_Model;
using AutomaticInvestmentPlan_Model.Converter;
using AutomaticInvestmentPlan_Network;

namespace AutomaticInvestmentPlan_Form
{
    public partial class Main : Form
    {
        private SpecifyFundNameService _specifyFundNameService;
        private SpecifyFundHistoryJumpService _specifyFundHistoryPointService;
        private SpecifyFundJumpService _specifyFundPointService;
        private readonly List<Task> _tasks = new List<Task>();


        public Main()
        {
            InitializeComponent();
            this.picLoader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picLoader.Location = new System.Drawing.Point(0, 0);
            this.picLoader.Name = "picLoader";
            this.picLoader.Size = new System.Drawing.Size(796, 566);
            this.picLoader.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLoader.TabIndex = 16;
            this.picLoader.TabStop = false;
            this.picLoader.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(180, 29);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(319, 28);
            this.label7.TabIndex = 17;
            this.label7.Text = "定投计算器正在加载指数数据……";
            this.label7.Visible = false;

            this.Controls.Add(this.picLoader);
            this.Controls.Add(this.label7);

            this.picLoader.BringToFront();
            this.label7.BringToFront();

        }

        private void Main_Shown(object sender, EventArgs e)
        {
            DisplayData();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            double r = CalculateUtil.CalcuateInvestmentAmount(CacheUtil.GeneralPoint,
                CacheUtil.SpecifyEstimationJumpPoint, CacheUtil.SpecifyPointJumpHistory);
            this.txtEstimateInvestAmount.Text = Convert.ToString(Math.Round(r), CultureInfo.InvariantCulture);
        }


        private void SetLoading(bool displayLoader)
        {
            if (displayLoader)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    picLoader.Visible = true;
                    this.label7.Visible = true;
                    this.label7.BringToFront();
                    this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                });
            }
            else
            {
                this.Invoke((MethodInvoker)delegate
                {
                    picLoader.Visible = false;
                    this.label7.Visible = false;
                    this.Cursor = System.Windows.Forms.Cursors.Default;
                });
            }
        }

        private void DisplayData()
        {
            _specifyFundNameService = new SpecifyFundNameService();
            _specifyFundHistoryPointService = new SpecifyFundHistoryJumpService();
            _specifyFundPointService = new SpecifyFundJumpService();

            SetLoading(true);

            Task t1 = Task.Factory.StartNew(() =>
            {
                GeneralPointService generalPointService = new GeneralPointService();
                string r = generalPointService.ExecuteCrawl();
                GeneralPointModel m = NetworkValueConverter.ConvertToGeneralPointModel(r);
                if (Convert.ToDouble(m.Percentate.Substring(0, m.Percentate.Length - 1)) > 0)
                {
                    labGeneralPoint.ForeColor = Color.Red;
                    labGeneralPointPercentage.ForeColor = Color.Red;
                }
                else
                {
                    labGeneralPoint.ForeColor = Color.Green;
                    labGeneralPointPercentage.ForeColor = Color.Green;
                }
                this.labGeneralPoint.Text = m.Point;
                this.labGeneralPointPercentage.Text = m.Percentate;

                CacheUtil.GeneralPoint = Convert.ToDouble(m.Point);
            });
            _tasks.Add(t1);

            Task t2 = Task.Factory.StartNew(() =>
            {
                string fundId = this.txtFundCode.Text.Trim();
                string p = _specifyFundPointService.ExecuteCrawl(fundId);
                this.txtEstimatedPoint.Text = p;
                CacheUtil.SpecifyEstimationJumpPoint = Convert.ToDouble(p.Substring(0, p.Length - 1)) / 100;
            });
            _tasks.Add(t2);

            Task t3 = Task.Factory.StartNew(() =>
            {
                string r = _specifyFundHistoryPointService.ExecuteCrawl(this.txtFundCode.Text.Trim());
                List<HistoryPointModel> list = NetworkValueConverter.ConvertToHistoryPointModel(r);
                this.dataGridViewFundHistory.DataSource = list;
                int width = this.dataGridViewFundHistory.Width;
                int avgWidth = width / 2;
                for (int i = 0; i < this.dataGridViewFundHistory.Columns.Count; i++)
                {
                    this.dataGridViewFundHistory.Columns[i].Width = avgWidth;
                }
                foreach (DataGridViewRow row in this.dataGridViewFundHistory.Rows)
                {
                    row.Selected = false;
                    row.ReadOnly = true;
                }
                //int headerHeight = this.dataGridViewFundHistory.ColumnHeadersHeight;
                //double rowHeight = (this.dataGridViewFundHistory.Height-headerHeight)*1.0 / list.Count;
                //this.dataGridViewFundHistory.RowTemplate.Height = Convert.ToInt32(rowHeight);
                int count = 0;
                CacheUtil.SpecifyPointJumpHistory = new List<double>();
                foreach (HistoryPointModel historyPointModel in list)
                {
                    if (count++ < 2)
                    {
                        CacheUtil.SpecifyPointJumpHistory.Add
                            (Convert.ToDouble(historyPointModel.Point.Substring(0, historyPointModel.Point.Length - 1)) / 100);
                    }
                }
            });
            _tasks.Add(t3);

            Task t4 = Task.Factory.StartNew(() =>
            {
                string fundId = this.txtFundCode.Text.Trim();
                string name = _specifyFundNameService.ExecuteCrawl(fundId);
                this.txtSpeicfyFundName.Text = name;
            });
            _tasks.Add(t4);

            Task.Factory.StartNew(() =>
            {
                Task.WaitAll(_tasks.ToArray());
                SetLoading(false);
            });
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void btnBuy_Click(object sender, EventArgs e)
        {
            string amount;
            if (this.txtBuyAmount.Text.Trim() == "")
            {
                amount = this.txtEstimateInvestAmount.Text.Trim();
            }
            else
            {
                amount = this.txtBuyAmount.Text.Trim();
            }
            //int investAmount = Convert.ToInt32(amount);
            int investAmount = 11;
            CacheUtil.BuyAmount = "11";
            Task.Factory.StartNew(() =>
            {
                BuyService buyService = new BuyService();
                string r = buyService.ExecuteBuy();
                //MessageBox.Show(this, r);
            });
        }
    }
}
