using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompensationSystemSubInterface.Models {
    /// <summary>
    /// 员工详细信息实体 (用于WPF绑定)
    /// </summary>
    public class EmpDetail {
        /// <summary>
        /// 员工ID (主键)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 员工编号
        /// </summary>
        public string EmployeeNo { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        // --- 基础下拉框 Key ---
        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        public string Nation { get; set; }

        /// <summary>
        /// 政治面貌
        /// </summary>
        public string Politic { get; set; }

        /// <summary>
        /// 婚姻状况
        /// </summary>
        public string Marital { get; set; }

        /// <summary>
        /// 学历
        /// </summary>
        public string Education { get; set; }

        /// <summary>
        /// 学位
        /// </summary>
        public string Degree { get; set; }

        /// <summary>
        /// 职称等级
        /// </summary>
        public string TitleLevel { get; set; }

        /// <summary>
        /// 人员类别
        /// </summary>
        public string PersonType { get; set; }

        // --- 组织信息 (ID用于绑定ComboBox, Name用于显示) ---
        /// <summary>
        /// 部门ID
        /// </summary>
        public int? DeptId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 序列ID
        /// </summary>
        public int? SeqId { get; set; }

        /// <summary>
        /// 序列名称
        /// </summary>
        public string SeqName { get; set; }

        /// <summary>
        /// 职务ID
        /// </summary>
        public int? JobId { get; set; }

        /// <summary>
        /// 职务名称
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// 职级ID
        /// </summary>
        public int? LevelId { get; set; }

        /// <summary>
        /// 职级名称
        /// </summary>
        public string LevelName { get; set; }

        // --- 文本/日期字段 ---
        /// <summary>
        /// 属相
        /// </summary>
        public string Zodiac { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 证件号码 (身份证号)
        /// </summary>
        public string IdCard { get; set; }

        /// <summary>
        /// 证件起始日期
        /// </summary>
        public DateTime? IdStart { get; set; }

        /// <summary>
        /// 证件截止日期
        /// </summary>
        public DateTime? IdEnd { get; set; }

        /// <summary>
        /// 参加工作时间
        /// </summary>
        public DateTime? WorkStart { get; set; }

        /// <summary>
        /// 入社时间 (入职日期)
        /// </summary>
        public DateTime? JoinDate { get; set; }

        /// <summary>
        /// 任现岗位(职务)时间
        /// </summary>
        public DateTime? PostDate { get; set; }

        /// <summary>
        /// 离职日期
        /// </summary>
        public DateTime? ResignDate { get; set; }

        /// <summary>
        /// 专业技术
        /// </summary>
        public string TechSpecialty { get; set; }

        /// <summary>
        /// 职称取得时间
        /// </summary>
        public DateTime? TitleDate { get; set; }

        /// <summary>
        /// 专业技能
        /// </summary>
        public string Skill { get; set; }

        /// <summary>
        /// 技能取得时间
        /// </summary>
        public DateTime? SkillDate { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 银行卡号
        /// </summary>
        public string BankCard { get; set; }

        /// <summary>
        /// 户籍地址
        /// </summary>
        public string HujiAddr { get; set; }

        /// <summary>
        /// 现住址
        /// </summary>
        public string CurrentAddr { get; set; }
    }
}
