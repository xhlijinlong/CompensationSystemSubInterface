using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompensationSystemSubInterface.Models {
    /// <summary>
    /// 简单的键值对，用于绑定ComboBox
    /// </summary>
    public class ComboItem {
        /// <summary>
        /// 唯一标识ID (用于数据绑定的值)
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 显示名称 (用于ComboBox显示)
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 文本内容 (用于只需要文本的下拉框)
        /// </summary>
        public string Text { get; set; }
    }
}
