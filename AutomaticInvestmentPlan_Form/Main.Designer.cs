namespace AutomaticInvestmentPlan_Form
{
    partial class Main
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.btnStart = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridViewFundHistory = new System.Windows.Forms.DataGridView();
            this.date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.point = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSpeicfyFundName = new System.Windows.Forms.TextBox();
            this.txtFundCode = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtEstimatedPoint = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtEstimateInvestAmount = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.picLoader = new System.Windows.Forms.PictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.labGeneralPoint = new System.Windows.Forms.Label();
            this.labGeneralPointPercentage = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtBuyAmount = new System.Windows.Forms.TextBox();
            this.btnBuy = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFundHistory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLoader)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(328, 77);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(195, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "开始计算";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(65, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "请输入基金代码";
            // 
            // dataGridViewFundHistory
            // 
            this.dataGridViewFundHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewFundHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.date,
            this.point});
            this.dataGridViewFundHistory.Location = new System.Drawing.Point(58, 237);
            this.dataGridViewFundHistory.Name = "dataGridViewFundHistory";
            this.dataGridViewFundHistory.RowTemplate.Height = 23;
            this.dataGridViewFundHistory.Size = new System.Drawing.Size(313, 229);
            this.dataGridViewFundHistory.TabIndex = 3;
            // 
            // date
            // 
            this.date.DataPropertyName = "date";
            this.date.HeaderText = "日期";
            this.date.Name = "date";
            // 
            // point
            // 
            this.point.DataPropertyName = "point";
            this.point.HeaderText = "涨跌";
            this.point.Name = "point";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(393, 299);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(145, 25);
            this.label2.TabIndex = 4;
            this.label2.Text = "本期应投放金额";
            // 
            // txtSpeicfyFundName
            // 
            this.txtSpeicfyFundName.Location = new System.Drawing.Point(136, 186);
            this.txtSpeicfyFundName.Name = "txtSpeicfyFundName";
            this.txtSpeicfyFundName.Size = new System.Drawing.Size(177, 21);
            this.txtSpeicfyFundName.TabIndex = 5;
            // 
            // txtFundCode
            // 
            this.txtFundCode.Location = new System.Drawing.Point(177, 79);
            this.txtFundCode.Name = "txtFundCode";
            this.txtFundCode.Size = new System.Drawing.Size(128, 21);
            this.txtFundCode.TabIndex = 6;
            this.txtFundCode.Text = "240014";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(65, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 21);
            this.label3.TabIndex = 7;
            this.label3.Text = "今日上证指数";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(324, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 21);
            this.label4.TabIndex = 9;
            this.label4.Text = "今日上证涨跌";
            // 
            // txtEstimatedPoint
            // 
            this.txtEstimatedPoint.Location = new System.Drawing.Point(469, 187);
            this.txtEstimatedPoint.Name = "txtEstimatedPoint";
            this.txtEstimatedPoint.Size = new System.Drawing.Size(100, 21);
            this.txtEstimatedPoint.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 134);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(629, 12);
            this.label5.TabIndex = 13;
            this.label5.Text = "_________________________________________________________________________________" +
    "_______________________";
            // 
            // txtEstimateInvestAmount
            // 
            this.txtEstimateInvestAmount.Location = new System.Drawing.Point(398, 340);
            this.txtEstimateInvestAmount.Name = "txtEstimateInvestAmount";
            this.txtEstimateInvestAmount.Size = new System.Drawing.Size(132, 21);
            this.txtEstimateInvestAmount.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(65, 187);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 20);
            this.label6.TabIndex = 14;
            this.label6.Text = "基金名称";
            // 
            // picLoader
            // 
            this.picLoader.Image = ((System.Drawing.Image)(resources.GetObject("picLoader.Image")));
            this.picLoader.Location = new System.Drawing.Point(475, 408);
            this.picLoader.Name = "picLoader";
            this.picLoader.Size = new System.Drawing.Size(100, 50);
            this.picLoader.TabIndex = 1;
            this.picLoader.TabStop = false;
            this.picLoader.Visible = false;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(0, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 23);
            this.label7.TabIndex = 0;
            // 
            // labGeneralPoint
            // 
            this.labGeneralPoint.AutoSize = true;
            this.labGeneralPoint.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labGeneralPoint.Location = new System.Drawing.Point(173, 29);
            this.labGeneralPoint.Name = "labGeneralPoint";
            this.labGeneralPoint.Size = new System.Drawing.Size(36, 21);
            this.labGeneralPoint.TabIndex = 18;
            this.labGeneralPoint.Text = "……";
            // 
            // labGeneralPointPercentage
            // 
            this.labGeneralPointPercentage.AutoSize = true;
            this.labGeneralPointPercentage.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labGeneralPointPercentage.Location = new System.Drawing.Point(436, 29);
            this.labGeneralPointPercentage.Name = "labGeneralPointPercentage";
            this.labGeneralPointPercentage.Size = new System.Drawing.Size(36, 21);
            this.labGeneralPointPercentage.TabIndex = 19;
            this.labGeneralPointPercentage.Text = "……";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(370, 187);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(93, 20);
            this.label8.TabIndex = 20;
            this.label8.Text = "今日涨跌估值";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(393, 384);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(126, 25);
            this.label9.TabIndex = 21;
            this.label9.Text = "自动买入金额";
            // 
            // txtBuyAmount
            // 
            this.txtBuyAmount.Location = new System.Drawing.Point(398, 423);
            this.txtBuyAmount.Name = "txtBuyAmount";
            this.txtBuyAmount.Size = new System.Drawing.Size(132, 21);
            this.txtBuyAmount.TabIndex = 22;
            // 
            // btnBuy
            // 
            this.btnBuy.Location = new System.Drawing.Point(398, 468);
            this.btnBuy.Name = "btnBuy";
            this.btnBuy.Size = new System.Drawing.Size(132, 23);
            this.btnBuy.TabIndex = 23;
            this.btnBuy.Text = "买入";
            this.btnBuy.UseVisualStyleBackColor = true;
            this.btnBuy.Click += new System.EventHandler(this.btnBuy_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(648, 529);
            this.Controls.Add(this.btnBuy);
            this.Controls.Add(this.txtBuyAmount);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtEstimateInvestAmount);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtEstimatedPoint);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtFundCode);
            this.Controls.Add(this.txtSpeicfyFundName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataGridViewFundHistory);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.labGeneralPoint);
            this.Controls.Add(this.labGeneralPointPercentage);
            this.Controls.Add(this.label8);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Main";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
            this.Shown += new System.EventHandler(this.Main_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFundHistory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLoader)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridViewFundHistory;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSpeicfyFundName;
        private System.Windows.Forms.TextBox txtFundCode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtEstimatedPoint;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtEstimateInvestAmount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox picLoader;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label labGeneralPoint;
        private System.Windows.Forms.Label labGeneralPointPercentage;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DataGridViewTextBoxColumn date;
        private System.Windows.Forms.DataGridViewTextBoxColumn point;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtBuyAmount;
        private System.Windows.Forms.Button btnBuy;
    }
}

