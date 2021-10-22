using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoalTrainMonitoringSystemServer
{
    public partial class FormMain1 : Form
    {
        const int BORDER_WIDTH = 15;

        public FormMain1()
        {
            InitializeComponent();
            HideTab();

             CheckForIllegalCrossThreadCalls = false;//允许跨线程调用
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);
            //初始化所有窗口
            InitializeAllWindows();
        }

        private void HideTab()
        {
            tabControlMain.SizeMode = TabSizeMode.Fixed;
            tabControlMain.ItemSize = new Size(0, 1);
        }

        void InitializeAllWindows()
        {
            int heightDisplay = Screen.PrimaryScreen.Bounds.Height - toolStrip1.Height;

            Display display = new Display();
            display.Parent = tabControlMain.TabPages[0];
            display.Left = Screen.PrimaryScreen.Bounds.Width / 24 + BORDER_WIDTH;
            display.Top = BORDER_WIDTH;
            display.Width = Screen.PrimaryScreen.Bounds.Width * 22 / 24 - BORDER_WIDTH * 2;
            display.Height = Screen.PrimaryScreen.Bounds.Height * 13 / 16 -BORDER_WIDTH * 2;
            display.Show();
        }

        private void FormMain1_Load(object sender, EventArgs e)
        {
            string backgroundImagePath = Application.StartupPath + "\\3363s.jpg";
            //Console.WriteLine(backgroundImagePath);
            Image TabBackground = Image.FromFile("3363s.jpg");//设置tab页面背景图片
            
            if (TabBackground != null)
            {
                //Console.WriteLine(tabControlMain.TabPages.Count);

                tabControlMain.TabPages[0].BackgroundImage = TabBackground;
                tabControlMain.TabPages[1].BackgroundImage = TabBackground;


                this.BackgroundImage = TabBackground;

            }
            else
            {
                Globals.Log("为空");
            }
        }

        private void tabControlMain_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripLabel2_Click(object sender, EventArgs e)
        {

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void splitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }
    }
}
