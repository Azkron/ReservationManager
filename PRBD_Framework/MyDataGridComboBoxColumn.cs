using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PRBD_Framework
{
    public class MyDataGridComboBoxColumn : DataGridComboBoxColumn
    {
        public MyDataGridComboBoxColumn() : base()
        {
        }

        public string MyItemSource
        {
            get; set;
        }


        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            return base.GenerateElement(cell, dataItem);
        }
    }
}
