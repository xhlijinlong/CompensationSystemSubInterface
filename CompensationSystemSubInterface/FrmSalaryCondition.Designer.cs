namespace CompensationSystemSubInterface {
    partial class FrmSalaryCondition {
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
            this.gpbSeq = new System.Windows.Forms.GroupBox();
            this.clbSeq = new System.Windows.Forms.CheckedListBox();
            this.cmsSelection = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiInvert = new System.Windows.Forms.ToolStripMenuItem();
            this.gpbDept = new System.Windows.Forms.GroupBox();
            this.clbDept = new System.Windows.Forms.CheckedListBox();
            this.gpbPost = new System.Windows.Forms.GroupBox();
            this.clbPost = new System.Windows.Forms.CheckedListBox();
            this.gpbLevel = new System.Windows.Forms.GroupBox();
            this.clbLevel = new System.Windows.Forms.CheckedListBox();
            this.gpbEmp = new System.Windows.Forms.GroupBox();
            this.clbEmp = new System.Windows.Forms.CheckedListBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnDefault = new System.Windows.Forms.Button();
            this.gpbItm = new System.Windows.Forms.GroupBox();
            this.clbItem = new System.Windows.Forms.CheckedListBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.gpbSeq.SuspendLayout();
            this.cmsSelection.SuspendLayout();
            this.gpbDept.SuspendLayout();
            this.gpbPost.SuspendLayout();
            this.gpbLevel.SuspendLayout();
            this.gpbEmp.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.gpbItm.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel1.Controls.Add(this.gpbSeq, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.gpbDept, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.gpbPost, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.gpbLevel, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.gpbItm, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.gpbEmp, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 27F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(684, 821);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // gpbSeq
            // 
            this.gpbSeq.Controls.Add(this.clbSeq);
            this.gpbSeq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpbSeq.Location = new System.Drawing.Point(4, 3);
            this.gpbSeq.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gpbSeq.Name = "gpbSeq";
            this.gpbSeq.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gpbSeq.Size = new System.Drawing.Size(197, 76);
            this.gpbSeq.TabIndex = 0;
            this.gpbSeq.TabStop = false;
            this.gpbSeq.Text = "选择序列";
            // 
            // clbSeq
            // 
            this.clbSeq.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.clbSeq.CheckOnClick = true;
            this.clbSeq.ContextMenuStrip = this.cmsSelection;
            this.clbSeq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbSeq.FormattingEnabled = true;
            this.clbSeq.Location = new System.Drawing.Point(4, 19);
            this.clbSeq.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.clbSeq.MultiColumn = true;
            this.clbSeq.Name = "clbSeq";
            this.clbSeq.Size = new System.Drawing.Size(189, 54);
            this.clbSeq.TabIndex = 0;
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
            this.gpbDept.Controls.Add(this.clbDept);
            this.gpbDept.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpbDept.Location = new System.Drawing.Point(209, 3);
            this.gpbDept.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gpbDept.Name = "gpbDept";
            this.gpbDept.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gpbDept.Size = new System.Drawing.Size(471, 76);
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
            this.clbDept.Size = new System.Drawing.Size(463, 54);
            this.clbDept.TabIndex = 0;
            // 
            // gpbPost
            // 
            this.gpbPost.Controls.Add(this.clbPost);
            this.gpbPost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpbPost.Location = new System.Drawing.Point(4, 85);
            this.gpbPost.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gpbPost.Name = "gpbPost";
            this.gpbPost.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gpbPost.Size = new System.Drawing.Size(197, 215);
            this.gpbPost.TabIndex = 1;
            this.gpbPost.TabStop = false;
            this.gpbPost.Text = "选择岗位";
            // 
            // clbPost
            // 
            this.clbPost.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.clbPost.CheckOnClick = true;
            this.clbPost.ContextMenuStrip = this.cmsSelection;
            this.clbPost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbPost.FormattingEnabled = true;
            this.clbPost.Location = new System.Drawing.Point(4, 19);
            this.clbPost.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.clbPost.MultiColumn = true;
            this.clbPost.Name = "clbPost";
            this.clbPost.Size = new System.Drawing.Size(189, 193);
            this.clbPost.TabIndex = 0;
            // 
            // gpbLevel
            // 
            this.gpbLevel.Controls.Add(this.clbLevel);
            this.gpbLevel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpbLevel.Location = new System.Drawing.Point(209, 85);
            this.gpbLevel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gpbLevel.Name = "gpbLevel";
            this.gpbLevel.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gpbLevel.Size = new System.Drawing.Size(471, 215);
            this.gpbLevel.TabIndex = 2;
            this.gpbLevel.TabStop = false;
            this.gpbLevel.Text = "选择层级";
            // 
            // clbLevel
            // 
            this.clbLevel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.clbLevel.CheckOnClick = true;
            this.clbLevel.ColumnWidth = 90;
            this.clbLevel.ContextMenuStrip = this.cmsSelection;
            this.clbLevel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbLevel.FormattingEnabled = true;
            this.clbLevel.Location = new System.Drawing.Point(4, 19);
            this.clbLevel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.clbLevel.MultiColumn = true;
            this.clbLevel.Name = "clbLevel";
            this.clbLevel.Size = new System.Drawing.Size(463, 193);
            this.clbLevel.TabIndex = 0;
            // 
            // gpbEmp
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.gpbEmp, 2);
            this.gpbEmp.Controls.Add(this.clbEmp);
            this.gpbEmp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpbEmp.Location = new System.Drawing.Point(4, 552);
            this.gpbEmp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gpbEmp.Name = "gpbEmp";
            this.gpbEmp.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gpbEmp.Size = new System.Drawing.Size(676, 266);
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
            this.clbEmp.Size = new System.Drawing.Size(668, 244);
            this.clbEmp.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnConfirm);
            this.flowLayoutPanel1.Controls.Add(this.btnDefault);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 821);
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
            // gpbItm
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.gpbItm, 2);
            this.gpbItm.Controls.Add(this.clbItem);
            this.gpbItm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpbItm.Location = new System.Drawing.Point(4, 306);
            this.gpbItm.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gpbItm.Name = "gpbItm";
            this.gpbItm.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gpbItm.Size = new System.Drawing.Size(676, 240);
            this.gpbItm.TabIndex = 4;
            this.gpbItm.TabStop = false;
            this.gpbItm.Text = "选择项目";
            // 
            // clbItem
            // 
            this.clbItem.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.clbItem.CheckOnClick = true;
            this.clbItem.ColumnWidth = 165;
            this.clbItem.ContextMenuStrip = this.cmsSelection;
            this.clbItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbItem.FormattingEnabled = true;
            this.clbItem.Location = new System.Drawing.Point(4, 19);
            this.clbItem.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.clbItem.MultiColumn = true;
            this.clbItem.Name = "clbItem";
            this.clbItem.Size = new System.Drawing.Size(668, 218);
            this.clbItem.TabIndex = 0;
            // 
            // FrmSalaryCondition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 861);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSalaryCondition";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "高级查询条件设置";
            this.Load += new System.EventHandler(this.FrmSalaryCondition_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.gpbSeq.ResumeLayout(false);
            this.cmsSelection.ResumeLayout(false);
            this.gpbDept.ResumeLayout(false);
            this.gpbPost.ResumeLayout(false);
            this.gpbLevel.ResumeLayout(false);
            this.gpbEmp.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.gpbItm.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox gpbSeq;
        private System.Windows.Forms.CheckedListBox clbSeq;
        private System.Windows.Forms.GroupBox gpbPost;
        private System.Windows.Forms.CheckedListBox clbPost;
        private System.Windows.Forms.GroupBox gpbLevel;
        private System.Windows.Forms.CheckedListBox clbLevel;
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
        private System.Windows.Forms.GroupBox gpbItm;
        private System.Windows.Forms.CheckedListBox clbItem;
    }
}