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
        public int Id { get; set; }
        public string Name { get; set; }
        // 只有文本的下拉框用这个
        public string Text { get; set; }
    }
}
