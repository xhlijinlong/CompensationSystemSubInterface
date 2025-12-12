using CompensationSystemSubInterface.Models;
using CompensationSystemSubInterface.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompensationSystemSubInterface {
    /// <summary>
    /// 员工变动信息查询高级筛选条件窗体
    /// </summary>
    public partial class FrmEmpCgCondition : Form {
        /// <summary>
        /// 获取当前的筛选条件
        /// </summary>
        public EmpCgQueryCondition CurrentCondition { get; private set; }

        /// <summary>
        /// 定义事件: 点击应用时将最新的条件传给主界面
        /// </summary>
        public event Action<EmpCgQueryCondition> ApplySelect;

        /// <summary>
        /// 初始化薪资筛选条件窗体
        /// </summary>
        /// <param name="existing">现有的筛选条件，如果为 null 则创建新的筛选条件对象</param>
        public FrmEmpCgCondition(EmpCgQueryCondition existing) {
            InitializeComponent();
            CurrentCondition = existing != null ? existing.Clone() : new EmpCgQueryCondition();
        }

        /// <summary>
        /// 窗体加载事件处理
        /// </summary>
        private void FrmEmpInfoCondition_Load(object sender, EventArgs e) {
            LoadBaseData();
            RestoreSelection();

            // 绑定右键菜单 (防止Designer未绑定)
            BindContextMenu(clbDept);
            BindContextMenu(clbEmp);
        }

        /// <summary>
        /// 为控件绑定右键上下文菜单
        /// </summary>
        /// <param name="ctl">要绑定菜单的控件</param>
        private void BindContextMenu(Control ctl) {
            if (ctl.ContextMenuStrip == null) ctl.ContextMenuStrip = cmsSelection;
        }

        /// <summary>
        /// 加载基础数据到各个 CheckedListBox 控件
        /// </summary>
        private void LoadBaseData() {
            // 加载基础数据 (注意 SQL 筛选条件)
            BindList(clbDept, "SELECT id, bmname FROM ZX_config_bm WHERE IsEnabled=1 AND DeleteType=0 ORDER BY DisplayOrder", "bmname", "id");
            BindList(clbEmp, "SELECT id, xingming FROM ZX_config_yg WHERE zaizhi=1 ORDER BY xuhao", "xingming", "id");
        }

        /// <summary>
        /// 绑定数据到 CheckedListBox 控件
        /// </summary>
        /// <param name="clb">要绑定数据的 CheckedListBox 控件</param>
        /// <param name="sql">查询 SQL 语句</param>
        /// <param name="display">显示成员字段名</param>
        /// <param name="value">值成员字段名</param>
        private void BindList(CheckedListBox clb, string sql, string display, string value) {
            DataTable dt = SqlHelper.ExecuteDataTable(sql);
            ((ListBox)clb).DataSource = dt;
            ((ListBox)clb).DisplayMember = display;
            ((ListBox)clb).ValueMember = value;
        }

        /// <summary>
        /// 根据当前筛选条件恢复各控件的选中状态
        /// </summary>
        private void RestoreSelection() {
            SetChecks(clbDept, CurrentCondition.DepartmentIds);
            SetChecks(clbEmp, CurrentCondition.EmployeeIds);
        }

        /// <summary>
        /// 根据 ID 列表设置 CheckedListBox 中的选中状态
        /// </summary>
        /// <param name="clb">CheckedListBox 控件</param>
        /// <param name="ids">要选中的 ID 列表</param>
        private void SetChecks(CheckedListBox clb, List<int> ids) {
            /*string valueField = clb.ValueMember;
            if (string.IsNullOrEmpty(valueField)) valueField = "id";
            for (int i = 0; i < clb.Items.Count; i++) {
                DataRowView drv = clb.Items[i] as DataRowView;
                if (drv != null && ids.Contains(Convert.ToInt32(drv[valueField]))) {
                    clb.SetItemChecked(i, true);
                }
            }*/
            for (int i = 0; i < clb.Items.Count; i++) {
                DataRowView drv = clb.Items[i] as DataRowView;
                if (drv != null && ids.Contains(Convert.ToInt32(drv["id"])))
                    clb.SetItemChecked(i, true);
            }
        }

        /// <summary>
        /// 确认按钮点击事件处理，保存筛选条件
        /// </summary>
        private void btnConfirm_Click(object sender, EventArgs e) {
            CurrentCondition.DepartmentIds = GetChecks(clbDept);
            CurrentCondition.EmployeeIds = GetChecks(clbEmp);

            ApplySelect?.Invoke(CurrentCondition);
            // 条件已经应用,主界面置顶
        }

        /// <summary>
        /// 获取 CheckedListBox 中已选中项的 ID 列表
        /// </summary>
        /// <param name="clb">CheckedListBox 控件</param>
        /// <returns>已选中项的 ID 列表</returns>
        private List<int> GetChecks(CheckedListBox clb) {
            /*List<int> list = new List<int>();
            string valueField = ((ListBox)clb).ValueMember;
            if (string.IsNullOrEmpty(valueField)) valueField = "id";
            foreach (DataRowView item in clb.CheckedItems) {
                list.Add(Convert.ToInt32(item[valueField]));
            }
            return list;*/
            List<int> list = new List<int>();
            foreach (DataRowView item in clb.CheckedItems) {
                list.Add(Convert.ToInt32(item["id"]));
            }
            return list;
        }

        /// <summary>
        /// 默认按钮点击事件处理，清空所有选中状态
        /// </summary>
        private void btnDefault_Click(object sender, EventArgs e) {
            // 清空所有 CheckBox
            foreach (Control gpb in tableLayoutPanel1.Controls) {
                if (gpb is GroupBox) {
                    foreach (Control c in gpb.Controls)
                        if (c is CheckedListBox clb)
                            for (int i = 0; i < clb.Items.Count; i++) clb.SetItemChecked(i, false);
                }
            }
        }

        /// <summary>
        /// 右键菜单"全选"项点击事件处理
        /// </summary>
        private void tsmiSelectAll_Click(object sender, EventArgs e) {
            CheckedListBox clb = GetSourceControl(sender);
            if (clb != null) {
                for (int i = 0; i < clb.Items.Count; i++) clb.SetItemChecked(i, true);
            }
        }

        /// <summary>
        /// 右键菜单"反选"项点击事件处理
        /// </summary>
        private void tsmiInvert_Click(object sender, EventArgs e) {
            CheckedListBox clb = GetSourceControl(sender);
            if (clb != null) {
                for (int i = 0; i < clb.Items.Count; i++) clb.SetItemChecked(i, !clb.GetItemChecked(i));
            }
        }

        /// <summary>
        /// 获取右键菜单的源控件
        /// </summary>
        /// <param name="sender">菜单项发送者</param>
        /// <returns>触发右键菜单的 CheckedListBox 控件，如果无法获取则返回 null</returns>
        private CheckedListBox GetSourceControl(object sender) {
            ToolStripMenuItem mi = sender as ToolStripMenuItem;
            if (mi != null && mi.Owner is ContextMenuStrip cms) return cms.SourceControl as CheckedListBox;
            return null;
        }
    }
}