namespace CompensationSystemSubInterface {
    partial class FrmPfmcCondition {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.gpbYear = new System.Windows.Forms.GroupBox();
            this.clbYear = new System.Windows.Forms.CheckedListBox();
            this.cmsSelection = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiInvert = new System.Windows.Forms.ToolStripMenuItem();
            this.gpbRslt = new System.Windows.Forms.GroupBox();
            this.clbRslt = new System.Windows.Forms.CheckedListBox();
            this.gpbEmp = new System.Windows.Forms.GroupBox();
            this.clbEmp = new System.Windows.Forms.CheckedListBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnDefault = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.gpbYear.SuspendLayout();
            this.cmsSelection.SuspendLayout();
            this.gpbRslt.SuspendLayout();
            this.gpbEmp.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.gpbYear, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.gpbRslt, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.gpbEmp, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(684, 421);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // gpbYear
            // 
            this.gpbYear.Controls.Add(this.clbYear);
            this.gpbYear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpbYear.Location = new System.Drawing.Point(4, 3);
            this.gpbYear.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gpbYear.Name = "gpbYear";
            this.gpbYear.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gpbYear.Size = new System.Drawing.Size(676, 57);
            this.gpbYear.TabIndex = 2;
            this.gpbYear.TabStop = false;
            this.gpbYear.Text = "选择年度";
            // 
            // clbYear
            // 
            this.clbYear.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.clbYear.CheckOnClick = true;
            this.clbYear.ContextMenuStrip = this.cmsSelection;
            this.clbYear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbYear.FormattingEnabled = true;
            this.clbYear.Location = new System.Drawing.Point(4, 19);
            this.clbYear.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.clbYear.MultiColumn = true;
            this.clbYear.Name = "clbYear";
            this.clbYear.Size = new System.Drawing.Size(668, 35);
            this.clbYear.TabIndex = 0;
            // 
            // cmsSelection
            // 
            this.cmsSelection.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSelectAll,
            this.tsmiInvert});
            this.cmsSelection.Name = "cmsSelection";
            this.cmsSelection.Size = new System.Drawing.Size(101, 48);
            // 
            // tsmiSelectAll
            // 
            this.tsmiSelectAll.Name = "tsmiSelectAll";
            this.tsmiSelectAll.Size = new System.Drawing.Size(100, 22);
            this.tsmiSelectAll.Text = "全选";
            this.tsmiSelectAll.Click += new System.EventHandler(this.tsmiSelectAll_Click);
            // 
            // tsmiInvert
            // 
            this.tsmiInvert.Name = "tsmiInvert";
            this.tsmiInvert.Size = new System.Drawing.Size(100, 22);
            this.tsmiInvert.Text = "反选";
            this.tsmiInvert.Click += new System.EventHandler(this.tsmiInvert_Click);
            // 
            // gpbRslt
            // 
            this.gpbRslt.Controls.Add(this.clbRslt);
            this.gpbRslt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpbRslt.Location = new System.Drawing.Point(4, 66);
            this.gpbRslt.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gpbRslt.Name = "gpbRslt";
            this.gpbRslt.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gpbRslt.Size = new System.Drawing.Size(676, 57);
            this.gpbRslt.TabIndex = 1;
            this.gpbRslt.TabStop = false;
            this.gpbRslt.Text = "选择结果";
            // 
            // clbRslt
            // 
            this.clbRslt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.clbRslt.CheckOnClick = true;
            this.clbRslt.ContextMenuStrip = this.cmsSelection;
            this.clbRslt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbRslt.FormattingEnabled = true;
            this.clbRslt.Location = new System.Drawing.Point(4, 19);
            this.clbRslt.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.clbRslt.MultiColumn = true;
            this.clbRslt.Name = "clbRslt";
            this.clbRslt.Size = new System.Drawing.Size(668, 35);
            this.clbRslt.TabIndex = 0;
            // 
            // gpbEmp
            // 
            this.gpbEmp.Controls.Add(this.clbEmp);
            this.gpbEmp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpbEmp.Location = new System.Drawing.Point(4, 129);
            this.gpbEmp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gpbEmp.Name = "gpbEmp";
            this.gpbEmp.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gpbEmp.Size = new System.Drawing.Size(676, 289);
            this.gpbEmp.TabIndex = 3;
            this.gpbEmp.TabStop = false;
            this.gpbEmp.Text = "选择员工";
            // 
            // clbEmp
            // 
            this.clbEmp.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.clbEmp.CheckOnClick = true;
            this.clbEmp.ContextMenuStrip = this.cmsSelection;
            this.clbEmp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbEmp.FormattingEnabled = true;
            this.clbEmp.Location = new System.Drawing.Point(4, 19);
            this.clbEmp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.clbEmp.MultiColumn = true;
            this.clbEmp.Name = "clbEmp";
            this.clbEmp.Size = new System.Drawing.Size(668, 267);
            this.clbEmp.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnConfirm);
            this.flowLayoutPanel1.Controls.Add(this.btnDefault);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 421);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(684, 40);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(604, 5);
            this.btnConfirm.Margin = new System.Windows.Forms.Padding(0, 5, 5, 5);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(75, 25);
            this.btnConfirm.TabIndex = 0;
            this.btnConfirm.Text = "确定";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnDefault
            // 
            this.btnDefault.Location = new System.Drawing.Point(514, 5);
            this.btnDefault.Margin = new System.Windows.Forms.Padding(0, 5, 5, 5);
            this.btnDefault.Name = "btnDefault";
            this.btnDefault.Size = new System.Drawing.Size(85, 25);
            this.btnDefault.TabIndex = 1;
            this.btnDefault.Text = "恢复默认";
            this.btnDefault.UseVisualStyleBackColor = true;
            this.btnDefault.Click += new System.EventHandler(this.btnDefault_Click);
            // 
            // FrmPfmcCondition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 461);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmPfmcCondition";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "高级查询条件设置";
            this.Load += new System.EventHandler(this.FrmPfmcCondition_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.gpbYear.ResumeLayout(false);
            this.cmsSelection.ResumeLayout(false);
            this.gpbRslt.ResumeLayout(false);
            this.gpbEmp.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnDefault;
        private System.Windows.Forms.GroupBox gpbRslt;
        private System.Windows.Forms.CheckedListBox clbRslt;
        private System.Windows.Forms.GroupBox gpbEmp;
        private System.Windows.Forms.CheckedListBox clbEmp;
        private System.Windows.Forms.ContextMenuStrip cmsSelection;
        private System.Windows.Forms.ToolStripMenuItem tsmiSelectAll;
        private System.Windows.Forms.ToolStripMenuItem tsmiInvert;
        private System.Windows.Forms.GroupBox gpbYear;
        private System.Windows.Forms.CheckedListBox clbYear;
    }
}