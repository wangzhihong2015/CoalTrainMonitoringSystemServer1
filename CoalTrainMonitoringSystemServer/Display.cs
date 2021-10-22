using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoalTrainMonitoringSystemServer
{
    public partial class Display : UserControl
    {
        public Display()
        {
            InitializeComponent();

            splitContainer1.SplitterDistance = splitContainer1.Height / 7;
            splitContainerImage1.SplitterDistance = splitContainerImage1.Width * 8 / 15;
        }
    }
}
