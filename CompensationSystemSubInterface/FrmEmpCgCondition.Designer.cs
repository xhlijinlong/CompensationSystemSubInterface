namespace CompensationSystemSubInterface {
    partial class FrmEmpCgCondition {
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
            this.cmsSelection = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiInvert = new System.Windows.Forms.ToolStripMenuItem();
            this.gpbDept = new System.Windows.Forms.GroupBox();
            this.clbDept = new System.Windows.Forms.CheckedListBox();
            this.gpbEmp = new System.Windows.Forms.GroupBox();
            this.clbEmp = new System.Windows.Forms.CheckedListBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnDefault = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.cmsSelection.SuspendLayout();
            this.gpbDept.SuspendLayout();
            this.gpbEmp.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.gpbDept, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.gpbEmp, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(684, 421);
            this.tableLayoutPanel1.TabIndex = 0;
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
            // gpbDept
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.gpbDept, 2);
            this.gpbDept.Controls.Add(this.clbDept);
            this.gpbDept.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpbDept.Location = new System.Drawing.Point(4, 3);
            this.gpbDept.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gpbDept.Name = "gpbDept";
            this.gpbDept.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gpbDept.Size = new System.Drawing.Size(676, 120);
            this.gpbDept.TabIndex = 1;
            this.gpbDept.TabStop = false;
            this.gpbDept.Text = "选择部门";
            // 
            // clbDept
            // 
            this.clbDept.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.clbDept.CheckOnClick = true;
            this.clbDept.ContextMenuStrip = this.cmsSelection;
            this.clbDept.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbDept.FormattingEnabled = true;
            this.clbDept.Location = new System.Drawing.Point(4, 19);
            this.clbDept.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.clbDept.MultiColumn = true;
            this.clbDept.Name = "clbDept";
            this.clbDept.Size = new System.Drawing.Size(668, 98);
            this.clbDept.TabIndex = 0;
            // 
            // gpbEmp
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.gpbEmp, 2);
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
            // FrmEmpInfoCondition
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
            this.Name = "FrmEmpInfoCondition";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "高级查询条件设置";
            this.Load += new System.EventHandler(this.FrmEmpInfoCondition_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.cmsSelection.ResumeLayout(false);
            this.gpbDept.ResumeLayout(false);
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
        private System.Windows.Forms.GroupBox gpbDept;
        private System.Windows.Forms.CheckedListBox clbDept;
        private System.Windows.Forms.GroupBox gpbEmp;
        private System.Windows.Forms.CheckedListBox clbEmp;
        private System.Windows.Forms.ContextMenuStrip cmsSelection;
        private System.Windows.Forms.ToolStripMenuItem tsmiSelectAll;
        private System.Windows.Forms.ToolStripMenuItem tsmiInvert;
    }
}