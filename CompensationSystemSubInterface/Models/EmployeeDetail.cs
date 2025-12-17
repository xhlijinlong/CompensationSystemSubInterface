using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompensationSystemSubInterface.Models {
    /// <summary>
    /// 员工详细信息实体 (用于WPF绑定)
    /// </summary>
    public class EmployeeDetail {
        public int Id { get; set; }
        public string EmployeeNo { get; set; } // 员工编号
        public string Name { get; set; } // 姓名

        // --- 基础下拉框 Key ---
        public string Gender { get; set; }
        public string Nation { get; set; }
        public string Politic { get; set; }
        public string Marital { get; set; }
        public string Education { get; set; }
        public string Degree { get; set; }
        public string TitleLevel { get; set; } // 职称等级
        public string PersonType { get; set; } // 人员类别

        // --- 组织信息 (ID用于绑定ComboBox, Name用于显示) ---
        public int? DeptId { get; set; }
        public string DeptName { get; set; }
        public int? SeqId { get; set; }
        public string SeqName { get; set; }
        public int? JobId { get; set; }
        public string JobName { get; set; }
        public int? LevelId { get; set; }
        public string LevelName { get; set; }

        // --- 文本/日期字段 ---
        public string Zodiac { get; set; } // 属相
        public int Age { get; set; }
        public DateTime? Birthday { get; set; }
        public string IdCard { get; set; } // 证件号码
        public DateTime? IdStart { get; set; }
        public DateTime? IdEnd { get; set; }

        public DateTime? WorkStart { get; set; } // 参加工作时间
        public DateTime? JoinDate { get; set; }  // 入社时间
        public DateTime? PostDate { get; set; }  // 任现岗位时间
        public DateTime? ResignDate { get; set; } // 离职日期

        public string TechSpecialty { get; set; } // 专业技术
        public DateTime? TitleDate { get; set; } // 职称取得时间
        public string Skill { get; set; } // 专业技能
        public DateTime? SkillDate { get; set; } // 技能时间

        public string Phone { get; set; }
        public string BankCard { get; set; }
        public string HujiAddr { get; set; }
        public string CurrentAddr { get; set; }
    }
}
