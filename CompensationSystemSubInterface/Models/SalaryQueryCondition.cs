using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompensationSystemSubInterface.Models {
    /// <summary>
    /// 薪资查询筛选条件类
    /// </summary>
    public class SalaryQueryCondition {
        /// <summary>
        /// 部门ID列表
        /// </summary>
        public List<int> DepartmentIds { get; set; } = new List<int>();
        /// <summary>
        /// 序列ID列表
        /// </summary>
        public List<int> SequenceIds { get; set; } = new List<int>();
        /// <summary>
        /// 职务ID列表
        /// </summary>
        public List<int> PositionIds { get; set; } = new List<int>();
        /// <summary>
        /// 层级ID列表
        /// </summary>
        public List<int> LevelIds { get; set; } = new List<int>();
        /// <summary>
        /// 项目ID列表
        /// </summary>
        public List<int> SalaryItemIds { get; set; } = new List<int>();
        /// <summary>
        /// 员工ID列表
        /// </summary>
        public List<int> EmployeeIds { get; set; } = new List<int>();

        /// <summary>
        /// 获取一个值，指示是否设置了任何筛选条件
        /// </summary>
        /// <value>如果至少有一个筛选条件列表不为空，则为 true；否则为 false</value>
        public bool HasFilter {
            get {
                return (DepartmentIds.Count > 0) ||
                    (SequenceIds.Count > 0) ||
                    (PositionIds.Count > 0) ||
                    (LevelIds.Count > 0) ||
                    (EmployeeIds.Count > 0) ||
                    (SalaryItemIds.Count > 0);
            }
        }

        /// <summary>
        /// 克隆当前筛选条件对象
        /// </summary>
        /// <returns>包含相同筛选条件的新实例</returns>
        public SalaryQueryCondition Clone() {
            return new SalaryQueryCondition {
                DepartmentIds = new List<int>(this.DepartmentIds),
                SequenceIds = new List<int>(this.SequenceIds),
                PositionIds = new List<int>(this.PositionIds),
                LevelIds = new List<int>(this.LevelIds),
                EmployeeIds = new List<int>(this.EmployeeIds),
                SalaryItemIds = new List<int>(this.SalaryItemIds)
            };
        }
    }
}
