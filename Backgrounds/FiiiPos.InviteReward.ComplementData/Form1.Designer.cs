namespace FiiiPos.InviteReward.ComplementData
{
    partial class Form1
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
            this.DataStartTime = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.DataEndTime = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.DataSaveTime = new System.Windows.Forms.DateTimePicker();
            this.Label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.LableTotalMessage = new System.Windows.Forms.Label();
            this.LableSuccessMessage = new System.Windows.Forms.Label();
            this.LableFaildMessage = new System.Windows.Forms.Label();
            this.LabelTotalCount = new System.Windows.Forms.Label();
            this.LabelSuccessCount = new System.Windows.Forms.Label();
            this.LabelFaildCount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // DataStartTime
            // 
            this.DataStartTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.DataStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DataStartTime.Location = new System.Drawing.Point(148, 45);
            this.DataStartTime.Name = "DataStartTime";
            this.DataStartTime.Size = new System.Drawing.Size(200, 21);
            this.DataStartTime.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(65, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "数据开始时间";
            // 
            // DataEndTime
            // 
            this.DataEndTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.DataEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DataEndTime.Location = new System.Drawing.Point(148, 89);
            this.DataEndTime.Name = "DataEndTime";
            this.DataEndTime.Size = new System.Drawing.Size(200, 21);
            this.DataEndTime.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(67, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "数据结束时间";
            // 
            // DataSaveTime
            // 
            this.DataSaveTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.DataSaveTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DataSaveTime.Location = new System.Drawing.Point(148, 132);
            this.DataSaveTime.Name = "DataSaveTime";
            this.DataSaveTime.Size = new System.Drawing.Size(200, 21);
            this.DataSaveTime.TabIndex = 4;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(67, 138);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(77, 12);
            this.Label3.TabIndex = 5;
            this.Label3.Text = "数据保存时间";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(148, 185);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "开始执行";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // LableTotalMessage
            // 
            this.LableTotalMessage.AutoSize = true;
            this.LableTotalMessage.Location = new System.Drawing.Point(65, 237);
            this.LableTotalMessage.Name = "LableTotalMessage";
            this.LableTotalMessage.Size = new System.Drawing.Size(35, 12);
            this.LableTotalMessage.TabIndex = 7;
            this.LableTotalMessage.Text = "总计:";
            // 
            // LableSuccessMessage
            // 
            this.LableSuccessMessage.AutoSize = true;
            this.LableSuccessMessage.Location = new System.Drawing.Point(65, 261);
            this.LableSuccessMessage.Name = "LableSuccessMessage";
            this.LableSuccessMessage.Size = new System.Drawing.Size(35, 12);
            this.LableSuccessMessage.TabIndex = 8;
            this.LableSuccessMessage.Text = "成功:";
            // 
            // LableFaildMessage
            // 
            this.LableFaildMessage.AutoSize = true;
            this.LableFaildMessage.Location = new System.Drawing.Point(65, 284);
            this.LableFaildMessage.Name = "LableFaildMessage";
            this.LableFaildMessage.Size = new System.Drawing.Size(35, 12);
            this.LableFaildMessage.TabIndex = 9;
            this.LableFaildMessage.Text = "失败:";
            // 
            // LabelTotalCount
            // 
            this.LabelTotalCount.AutoSize = true;
            this.LabelTotalCount.Location = new System.Drawing.Point(103, 237);
            this.LabelTotalCount.Name = "LabelTotalCount";
            this.LabelTotalCount.Size = new System.Drawing.Size(0, 12);
            this.LabelTotalCount.TabIndex = 10;
            // 
            // LabelSuccessCount
            // 
            this.LabelSuccessCount.AutoSize = true;
            this.LabelSuccessCount.Location = new System.Drawing.Point(103, 260);
            this.LabelSuccessCount.Name = "LabelSuccessCount";
            this.LabelSuccessCount.Size = new System.Drawing.Size(0, 12);
            this.LabelSuccessCount.TabIndex = 11;
            // 
            // LabelFaildCount
            // 
            this.LabelFaildCount.AutoSize = true;
            this.LabelFaildCount.Location = new System.Drawing.Point(103, 284);
            this.LabelFaildCount.Name = "LabelFaildCount";
            this.LabelFaildCount.Size = new System.Drawing.Size(0, 12);
            this.LabelFaildCount.TabIndex = 12;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 320);
            this.Controls.Add(this.LabelFaildCount);
            this.Controls.Add(this.LabelSuccessCount);
            this.Controls.Add(this.LabelTotalCount);
            this.Controls.Add(this.LableFaildMessage);
            this.Controls.Add(this.LableSuccessMessage);
            this.Controls.Add(this.LableTotalMessage);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.DataSaveTime);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.DataEndTime);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DataStartTime);
            this.Name = "Form1";
            this.Text = "补发邀请人奖励";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker DataStartTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker DataEndTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker DataSaveTime;
        private System.Windows.Forms.Label Label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label LableTotalMessage;
        private System.Windows.Forms.Label LableSuccessMessage;
        private System.Windows.Forms.Label LableFaildMessage;
        private System.Windows.Forms.Label LabelTotalCount;
        private System.Windows.Forms.Label LabelSuccessCount;
        private System.Windows.Forms.Label LabelFaildCount;
    }
}

