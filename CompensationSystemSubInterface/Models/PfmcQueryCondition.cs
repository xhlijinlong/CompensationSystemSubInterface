using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompensationSystemSubInterface.Models {
    /// <summary>
    /// 绩效查询筛选条件类
    /// </summary>
    public class PfmcQueryCondition {
        /// <summary>
        /// 年份筛选 (存 int, 例如 2023, 2024)
        /// </summary>
        public List<int> Years { get; set; } = new List<int>();

        /// <summary>
        /// 结果筛选 (存 string, 例如 "优秀", "合格")
        /// </summary>
        public List<string> Results { get; set; } = new List<string>();

        /// <summary>
        /// 员工ID列表
        /// </summary>
        public List<int> EmployeeIds { get; set; } = new List<int>();

        /// <summary>
        /// 序列ID列表
        /// </summary>
        public List<int> SequenceIds { get; set; } = new List<int>();

        /// <summary>
        /// 部门ID列表
        /// </summary>
        public List<int> DepartmentIds { get; set; } = new List<int>();

        /// <summary>
        /// 职务ID列表
        /// </summary>
        public List<int> PositionIds { get; set; } = new List<int>();

        /// <summary>
        /// 获取一个值，指示是否设置了任何筛选条件
        /// </summary>
        public bool HasFilter {
            get {
                return (EmployeeIds.Count > 0) || (Years.Count > 0) || (Results.Count > 0) 
                    || (SequenceIds.Count > 0) || (DepartmentIds.Count > 0) || (PositionIds.Count > 0);
            }
        }

        /// <summary>
        /// 克隆当前筛选条件对象
        /// </summary>
        public PfmcQueryCondition Clone() {
            return new PfmcQueryCondition {
                EmployeeIds = new List<int>(this.EmployeeIds),
                Years = new List<int>(this.Years),
                Results = new List<string>(this.Results),
                SequenceIds = new List<int>(this.SequenceIds),
                DepartmentIds = new List<int>(this.DepartmentIds),
                PositionIds = new List<int>(this.PositionIds)
            };
        }
    }
}
