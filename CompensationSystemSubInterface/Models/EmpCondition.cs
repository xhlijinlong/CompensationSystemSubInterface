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
        /// 序列ID列表
        /// </summary>
        public List<int> SequenceIds { get; set; } = new List<int>();
        /// <summary>
        /// 职务ID列表
        /// </summary>
        public List<int> PositionIds { get; set; } = new List<int>();
        /// <summary>
        /// 性别列表（字符串：男、女）
        /// </summary>
        public List<string> Genders { get; set; } = new List<string>();
        /// <summary>
        /// 民族列表（字符串）
        /// </summary>
        public List<string> Ethnics { get; set; } = new List<string>();
        /// <summary>
        /// 属相列表（字符串：12生肖）
        /// </summary>
        public List<string> Zodiacs { get; set; } = new List<string>();
        /// <summary>
        /// 政治面貌列表（字符串）
        /// </summary>
        public List<string> Politics { get; set; } = new List<string>();
        /// <summary>
        /// 学历列表（字符串）
        /// </summary>
        public List<string> Educations { get; set; } = new List<string>();
        /// <summary>
        /// 学位列表（字符串）
        /// </summary>
        public List<string> Degrees { get; set; } = new List<string>();
        /// <summary>
        /// 职称等级列表（字符串）
        /// </summary>
        public List<string> TitleLevels { get; set; } = new List<string>();

        /// <summary>
        /// 获取一个值，指示是否设置了任何筛选条件
        /// </summary>
        /// <value>如果至少有一个筛选条件列表不为空，则为 true；否则为 false</value>
        public bool HasFilter {
            get {
                return (DepartmentIds.Count > 0) ||
                    (EmployeeIds.Count > 0) ||
                    (SequenceIds.Count > 0) ||
                    (PositionIds.Count > 0) ||
                    (Genders.Count > 0) ||
                    (Ethnics.Count > 0) ||
                    (Zodiacs.Count > 0) ||
                    (Politics.Count > 0) ||
                    (Educations.Count > 0) ||
                    (Degrees.Count > 0) ||
                    (TitleLevels.Count > 0);
            }
        }

        /// <summary>
        /// 克隆当前筛选条件对象
        /// </summary>
        /// <returns>包含相同筛选条件的新实例</returns>
        public EmpCondition Clone() {
            return new EmpCondition {
                DepartmentIds = new List<int>(this.DepartmentIds),
                EmployeeIds = new List<int>(this.EmployeeIds),
                SequenceIds = new List<int>(this.SequenceIds),
                PositionIds = new List<int>(this.PositionIds),
                Genders = new List<string>(this.Genders),
                Ethnics = new List<string>(this.Ethnics),
                Zodiacs = new List<string>(this.Zodiacs),
                Politics = new List<string>(this.Politics),
                Educations = new List<string>(this.Educations),
                Degrees = new List<string>(this.Degrees),
                TitleLevels = new List<string>(this.TitleLevels)
            };
        }
    }
}

