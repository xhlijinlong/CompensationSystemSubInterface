namespace CompensationSystemSubInterface {
    partial class UserControl_PfmcQuery {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            this.pnlBody = new System.Windows.Forms.Panel();
            this.dgvSalary = new System.Windows.Forms.DataGridView();
            this.flpnlTop = new System.Windows.Forms.FlowLayoutPanel();
            this.lbName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.btnYear = new System.Windows.Forms.Button();
            this.btnRslt = new System.Windows.Forms.Button();
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
            this.flpnlTop.Controls.Add(this.lbName);
            this.flpnlTop.Controls.Add(this.txtName);
            this.flpnlTop.Controls.Add(this.btnYear);
            this.flpnlTop.Controls.Add(this.btnRslt);
            this.flpnlTop.Controls.Add(this.btnCondition);
            this.flpnlTop.Controls.Add(this.btnQuery);
            this.flpnlTop.Controls.Add(this.btnExport);
            this.flpnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.flpnlTop.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.flpnlTop.Location = new System.Drawing.Point(0, 0);
            this.flpnlTop.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.flpnlTop.Name = "flpnlTop";
            this.flpnlTop.Padding = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.flpnlTop.Size = new System.Drawing.Size(1200, 60);
            this.flpnlTop.TabIndex = 0;
            // 
            // lbName
            // 
            this.lbName.AutoSize = true;
            this.lbName.Location = new System.Drawing.Point(6, 16);
            this.lbName.Margin = new System.Windows.Forms.Padding(0, 11, 6, 11);
            this.lbName.Name = "lbName";
            this.lbName.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lbName.Size = new System.Drawing.Size(50, 20);
            this.lbName.TabIndex = 9;
            this.lbName.Text = "姓名:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(62, 16);
            this.txtName.Margin = new System.Windows.Forms.Padding(0, 11, 6, 11);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(65, 23);
            this.txtName.TabIndex = 10;
            // 
            // btnYear
            // 
            this.btnYear.Location = new System.Drawing.Point(133, 16);
            this.btnYear.Margin = new System.Windows.Forms.Padding(0, 11, 6, 11);
            this.btnYear.Name = "btnYear";
            this.btnYear.Size = new System.Drawing.Size(55, 25);
            this.btnYear.TabIndex = 14;
            this.btnYear.Text = "年度";
            this.btnYear.UseVisualStyleBackColor = true;
            this.btnYear.Click += new System.EventHandler(this.btnYear_Click);
            // 
            // btnRslt
            // 
            this.btnRslt.Location = new System.Drawing.Point(194, 16);
            this.btnRslt.Margin = new System.Windows.Forms.Padding(0, 11, 6, 11);
            this.btnRslt.Name = "btnRslt";
            this.btnRslt.Size = new System.Drawing.Size(55, 25);
            this.btnRslt.TabIndex = 15;
            this.btnRslt.Text = "结果";
            this.btnRslt.UseVisualStyleBackColor = true;
            this.btnRslt.Click += new System.EventHandler(this.btnRslt_Click);
            // 
            // btnCondition
            // 
            this.btnCondition.Location = new System.Drawing.Point(255, 16);
            this.btnCondition.Margin = new System.Windows.Forms.Padding(0, 11, 6, 11);
            this.btnCondition.Name = "btnCondition";
            this.btnCondition.Size = new System.Drawing.Size(80, 25);
            this.btnCondition.TabIndex = 12;
            this.btnCondition.Text = "员工设置";
            this.btnCondition.UseVisualStyleBackColor = true;
            this.btnCondition.Click += new System.EventHandler(this.btnCondition_Click);
            // 
            // btnQuery
            // 
            this.btnQuery.BackColor = System.Drawing.Color.AliceBlue;
            this.btnQuery.Location = new System.Drawing.Point(341, 16);
            this.btnQuery.Margin = new System.Windows.Forms.Padding(0, 11, 6, 11);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(55, 25);
            this.btnQuery.TabIndex = 11;
            this.btnQuery.Text = "查询";
            this.btnQuery.UseVisualStyleBackColor = false;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(402, 16);
            this.btnExport.Margin = new System.Windows.Forms.Padding(0, 11, 6, 11);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(95, 25);
            this.btnExport.TabIndex = 13;
            this.btnExport.Text = "导出Excel";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // UserControl_PfmcQuery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.flpnlTop);
            this.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "UserControl_PfmcQuery";
            this.Size = new System.Drawing.Size(1200, 600);
            this.Load += new System.EventHandler(this.UserControl_PfmcQuery_Load);
            this.pnlBody.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSalary)).EndInit();
            this.flpnlTop.ResumeLayout(false);
            this.flpnlTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel pnlBody;
        private System.Windows.Forms.FlowLayoutPanel flpnlTop;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.Button btnCondition;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.DataGridView dgvSalary;
        private System.Windows.Forms.Button btnYear;
        private System.Windows.Forms.Button btnRslt;
    }
}
