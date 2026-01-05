using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompensationSystemSubInterface.Models {
    /// <summary>
    /// 通用员工筛选条件类
    /// 用于员工查询、员工变动查询、员工维护等界面的筛选条件
    /// </summary>
    public class EmpCondition {
        /// <summary>
        /// 部门ID列表
        /// </summary>
        public List<int> DepartmentIds { get; set; } = new List<int>();
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
                    (EmployeeIds.Count > 0);
            }
        }

        /// <summary>
        /// 克隆当前筛选条件对象
        /// </summary>
        /// <returns>包含相同筛选条件的新实例</returns>
        public EmpCondition Clone() {
            return new EmpCondition {
                DepartmentIds = new List<int>(this.DepartmentIds),
                EmployeeIds = new List<int>(this.EmployeeIds)
            };
        }
    }
}
