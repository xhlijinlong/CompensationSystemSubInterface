namespace CompensationSystemSubInterface
{
    partial class UserControl_SalaryQuery
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

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlBody = new System.Windows.Forms.Panel();
            this.dgvSalary = new System.Windows.Forms.DataGridView();
            this.flpnlTop = new System.Windows.Forms.FlowLayoutPanel();
            this.lbDate = new System.Windows.Forms.Label();
            this.cbYear1 = new System.Windows.Forms.ComboBox();
            this.lbYear1 = new System.Windows.Forms.Label();
            this.cbMonth1 = new System.Windows.Forms.ComboBox();
            this.lbMonth1 = new System.Windows.Forms.Label();
            this.cbYear2 = new System.Windows.Forms.ComboBox();
            this.lbYear2 = new System.Windows.Forms.Label();
            this.cbMonth2 = new System.Windows.Forms.ComboBox();
            this.lbMonth2 = new System.Windows.Forms.Label();
            this.lbName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.btnSeq = new System.Windows.Forms.Button();
            this.btnDept = new System.Windows.Forms.Button();
            this.btnPost = new System.Windows.Forms.Button();
            this.btnStatus = new System.Windows.Forms.Button();
            this.btnCondition = new System.Windows.Forms.Button();
            this.btnQuery = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.pnlBody.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSalary)).BeginInit();
            this.flpnlTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBody
            // 
            this.pnlBody.Controls.Add(this.dgvSalary);
            this.pnlBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBody.Location = new System.Drawing.Point(0, 60);
            this.pnlBody.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Padding = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.pnlBody.Size = new System.Drawing.Size(1200, 540);
            this.pnlBody.TabIndex = 1;
            // 
            // dgvSalary
            // 
            this.dgvSalary.AllowUserToAddRows = false;
            this.dgvSalary.BackgroundColor = System.Drawing.Color.White;
            this.dgvSalary.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvSalary.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSalary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSalary.Location = new System.Drawing.Point(6, 5);
            this.dgvSalary.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dgvSalary.Name = "dgvSalary";
            this.dgvSalary.ReadOnly = true;
            this.dgvSalary.RowTemplate.Height = 23;
            this.dgvSalary.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSalary.Size = new System.Drawing.Size(1188, 530);
            this.dgvSalary.TabIndex = 0;
            // 
            // flpnlTop
            // 
            this.flpnlTop.Controls.Add(this.lbDate);
            this.flpnlTop.Controls.Add(this.cbYear1);
            this.flpnlTop.Controls.Add(this.lbYear1);
            this.flpnlTop.Controls.Add(this.cbMonth1);
            this.flpnlTop.Controls.Add(this.lbMonth1);
            this.flpnlTop.Controls.Add(this.cbYear2);
            this.flpnlTop.Controls.Add(this.lbYear2);
            this.flpnlTop.Controls.Add(this.cbMonth2);
            this.flpnlTop.Controls.Add(this.lbMonth2);
            this.flpnlTop.Controls.Add(this.lbName);
            this.flpnlTop.Controls.Add(this.txtName);
            this.flpnlTop.Controls.Add(this.btnSeq);
            this.flpnlTop.Controls.Add(this.btnDept);
            this.flpnlTop.Controls.Add(this.btnPost);
            this.flpnlTop.Controls.Add(this.btnStatus);
            this.flpnlTop.Controls.Add(this.btnCondition);
            this.flpnlTop.Controls.Add(this.btnQuery);
            this.flpnlTop.Controls.Add(this.btnExport);
            this.flpnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.flpnlTop.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.flpnlTop.Location = new System.Drawing.Point(0, 0);
            this.flpnlTop.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.flpnlTop.Name = "flpnlTop";
            this.flpnlTop.Padding = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.flpnlTop.Size = new System.Drawing.Size(1200, 60);
            this.flpnlTop.TabIndex = 0;
            // 
            // lbDate
            // 
            this.lbDate.AutoSize = true;
            this.lbDate.Location = new System.Drawing.Point(6, 16);
            this.lbDate.Margin = new System.Windows.Forms.Padding(0, 11, 6, 11);
            this.lbDate.Name = "lbDate";
            this.lbDate.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lbDate.Size = new System.Drawing.Size(54, 27);
            this.lbDate.TabIndex = 0;
            this.lbDate.Text = "日期:";
            // 
            // cbYear1
            // 
            this.cbYear1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbYear1.FormattingEnabled = true;
            this.cbYear1.Location = new System.Drawing.Point(66, 16);
            this.cbYear1.Margin = new System.Windows.Forms.Padding(0, 11, 6, 11);
            this.cbYear1.Name = "cbYear1";
            this.cbYear1.Size = new System.Drawing.Size(65, 29);
            this.cbYear1.TabIndex = 1;
            // 
            // lbYear1
            // 
            this.lbYear1.AutoSize = true;
            this.lbYear1.Location = new System.Drawing.Point(137, 16);
            this.lbYear1.Margin = new System.Windows.Forms.Padding(0, 11, 6, 11);
            this.lbYear1.Name = "lbYear1";
            this.lbYear1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lbYear1.Size = new System.Drawing.Size(34, 27);
            this.lbYear1.TabIndex = 2;
            this.lbYear1.Text = "年";
            // 
            // cbMonth1
            // 
            this.cbMonth1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMonth1.FormattingEnabled = true;
            this.cbMonth1.Location = new System.Drawing.Point(177, 16);
            this.cbMonth1.Margin = new System.Windows.Forms.Padding(0, 11, 6, 11);
            this.cbMonth1.Name = "cbMonth1";
            this.cbMonth1.Size = new System.Drawing.Size(45, 29);
            this.cbMonth1.TabIndex = 3;
            // 
            // lbMonth1
            // 
            this.lbMonth1.AutoSize = true;
            this.lbMonth1.Location = new System.Drawing.Point(228, 16);
            this.lbMonth1.Margin = new System.Windows.Forms.Padding(0, 11, 6, 11);
            this.lbMonth1.Name = "lbMonth1";
            this.lbMonth1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lbMonth1.Size = new System.Drawing.Size(46, 27);
            this.lbMonth1.TabIndex = 4;
            this.lbMonth1.Text = "月 -";
            // 
            // cbYear2
            // 
            this.cbYear2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbYear2.FormattingEnabled = true;
            this.cbYear2.Location = new System.Drawing.Point(280, 16);
            this.cbYear2.Margin = new System.Windows.Forms.Padding(0, 11, 6, 11);
            this.cbYear2.Name = "cbYear2";
            this.cbYear2.Size = new System.Drawing.Size(65, 29);
            this.cbYear2.TabIndex = 5;
            // 
            // lbYear2
            // 
            this.lbYear2.AutoSize = true;
            this.lbYear2.Location = new System.Drawing.Point(351, 16);
            this.lbYear2.Margin = new System.Windows.Forms.Padding(0, 11, 6, 11);
            this.lbYear2.Name = "lbYear2";
            this.lbYear2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lbYear2.Size = new System.Drawing.Size(34, 27);
            this.lbYear2.TabIndex = 6;
            this.lbYear2.Text = "年";
            // 
            // cbMonth2
            // 
            this.cbMonth2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMonth2.FormattingEnabled = true;
            this.cbMonth2.Location = new System.Drawing.Point(391, 16);
            this.cbMonth2.Margin = new System.Windows.Forms.Padding(0, 11, 6, 11);
            this.cbMonth2.Name = "cbMonth2";
            this.cbMonth2.Size = new System.Drawing.Size(45, 29);
            this.cbMonth2.TabIndex = 7;
            // 
            // lbMonth2
            // 
            this.lbMonth2.AutoSize = true;
            this.lbMonth2.Location = new System.Drawing.Point(442, 16);
            this.lbMonth2.Margin = new System.Windows.Forms.Padding(0, 11, 6, 11);
            this.lbMonth2.Name = "lbMonth2";
            this.lbMonth2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lbMonth2.Size = new System.Drawing.Size(34, 27);
            this.lbMonth2.TabIndex = 8;
            this.lbMonth2.Text = "月";
            // 
            // lbName
            // 
            this.lbName.AutoSize = true;
            this.lbName.Location = new System.Drawing.Point(482, 16);
            this.lbName.Margin = new System.Windows.Forms.Padding(0, 11, 6, 11);
            this.lbName.Name = "lbName";
            this.lbName.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lbName.Size = new System.Drawing.Size(54, 27);
            this.lbName.TabIndex = 9;
            this.lbName.Text = "姓名:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(542, 16);
            this.txtName.Margin = new System.Windows.Forms.Padding(0, 11, 6, 11);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(65, 29);
            this.txtName.TabIndex = 10;
            // 
            // btnSeq
            // 
            this.btnSeq.Location = new System.Drawing.Point(613, 16);
            this.btnSeq.Margin = new System.Windows.Forms.Padding(0, 11, 6, 11);
            this.btnSeq.Name = "btnSeq";
            this.btnSeq.Size = new System.Drawing.Size(60, 30);
            this.btnSeq.TabIndex = 14;
            this.btnSeq.Text = "序列";
            this.btnSeq.UseVisualStyleBackColor = true;
            this.btnSeq.Click += new System.EventHandler(this.btnSeq_Click);
            // 
            // btnDept
            // 
            this.btnDept.Location = new System.Drawing.Point(679, 16);
            this.btnDept.Margin = new System.Windows.Forms.Padding(0, 11, 6, 11);
            this.btnDept.Name = "btnDept";
            this.btnDept.Size = new System.Drawing.Size(60, 30);
            this.btnDept.TabIndex = 15;
            this.btnDept.Text = "部门";
            this.btnDept.UseVisualStyleBackColor = true;
            this.btnDept.Click += new System.EventHandler(this.btnDept_Click);
            // 
            // btnPost
            // 
            this.btnPost.Location = new System.Drawing.Point(745, 16);
            this.btnPost.Margin = new System.Windows.Forms.Padding(0, 11, 6, 11);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(60, 30);
            this.btnPost.TabIndex = 16;
            this.btnPost.Text = "职务";
            this.btnPost.UseVisualStyleBackColor = true;
            this.btnPost.Click += new System.EventHandler(this.btnPost_Click);
            // 
            // btnStatus
            // 
            this.btnStatus.Location = new System.Drawing.Point(811, 16);
            this.btnStatus.Margin = new System.Windows.Forms.Padding(0, 11, 6, 11);
            this.btnStatus.Name = "btnStatus";
            this.btnStatus.Size = new System.Drawing.Size(60, 30);
            this.btnStatus.TabIndex = 17;
            this.btnStatus.Text = "状态";
            this.btnStatus.UseVisualStyleBackColor = true;
            this.btnStatus.Click += new System.EventHandler(this.btnStatus_Click);
            // 
            // btnCondition
            // 
            this.btnCondition.Location = new System.Drawing.Point(877, 16);
            this.btnCondition.Margin = new System.Windows.Forms.Padding(0, 11, 6, 11);
            this.btnCondition.Name = "btnCondition";
            this.btnCondition.Size = new System.Drawing.Size(95, 30);
            this.btnCondition.TabIndex = 12;
            this.btnCondition.Text = "条件设置";
            this.btnCondition.UseVisualStyleBackColor = true;
            this.btnCondition.Click += new System.EventHandler(this.btnCondition_Click);
            // 
            // btnQuery
            // 
            this.btnQuery.BackColor = System.Drawing.Color.AliceBlue;
            this.btnQuery.Location = new System.Drawing.Point(978, 16);
            this.btnQuery.Margin = new System.Windows.Forms.Padding(0, 11, 6, 11);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(55, 30);
            this.btnQuery.TabIndex = 11;
            this.btnQuery.Text = "查询";
            this.btnQuery.UseVisualStyleBackColor = false;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(1039, 16);
            this.btnExport.Margin = new System.Windows.Forms.Padding(0, 11, 6, 11);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(95, 30);
            this.btnExport.TabIndex = 13;
            this.btnExport.Text = "导出Excel";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // UserControl_SalaryQuery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.flpnlTop);
            this.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "UserControl_SalaryQuery";
            this.Size = new System.Drawing.Size(1200, 600);
            this.Load += new System.EventHandler(this.UserControl_SalaryQuery_Load);
            this.pnlBody.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSalary)).EndInit();
            this.flpnlTop.ResumeLayout(false);
            this.flpnlTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel pnlBody;
        private System.Windows.Forms.FlowLayoutPanel flpnlTop;
        private System.Windows.Forms.Label lbDate;
        private System.Windows.Forms.ComboBox cbYear1;
        private System.Windows.Forms.Label lbYear1;
        private System.Windows.Forms.ComboBox cbMonth1;
        private System.Windows.Forms.Label lbMonth1;
        private System.Windows.Forms.ComboBox cbYear2;
        private System.Windows.Forms.Label lbYear2;
        private System.Windows.Forms.ComboBox cbMonth2;
        private System.Windows.Forms.Label lbMonth2;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.Button btnCondition;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.DataGridView dgvSalary;
        private System.Windows.Forms.Button btnSeq;
        private System.Windows.Forms.Button btnDept;
        private System.Windows.Forms.Button btnPost;
        private System.Windows.Forms.Button btnStatus;
    }
}
