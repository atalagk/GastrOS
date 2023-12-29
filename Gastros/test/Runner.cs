using System;
using System.Drawing;
using System.Windows.Forms;
using GastrOs.Sde.Views.WinForms;

namespace GastrOs.Sde.Test
{
    class Runner
    {
        [STAThread]
        static void Main(string[] args)
        {
            GuiTestFormlet f = new GuiTestFormlet();
            Application.Run(f);
        }
    }
}