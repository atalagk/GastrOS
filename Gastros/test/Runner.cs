using System;
using System.Windows.Forms;

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