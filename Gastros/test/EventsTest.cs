using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using GastrOs.Sde.Views.WinForms.Generic;
using NUnit.Framework;

namespace GastrOs.Sde.Test
{
    public class Observable : IDisposable
    {
        public event EventHandler Fire;

        ~Observable()
        {
            Console.WriteLine("Collected");
        }

        public void FireEvent()
        {
            if (Fire != null)
                Fire(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            //Fire = null;
        }
    }

    public class Observer : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    [TestFixture]
    public class EventsTest
    {
        [Test]
        public void Test1()
        {
            Observable o = new Observable();
            o.Fire += Response1;
            o.Fire += Response2;

            o.FireEvent();

            o.Dispose();

            o = null;

            GC.Collect();

            Thread.CurrentThread.Join(1000);

            Observable o2 = new Observable();
            o2.Fire += Response1;
            o2.Fire += Response2;

            o2.FireEvent();
        }

        [Test]
        public void Test2()
        {
            long mem = GC.GetTotalMemory(true);
            Debug.WriteLine(mem);
            List<Form> l = new List<Form>();
            for (int i = 0; i < 100; i++)
            {
                LockEditForm form = new LockEditForm(new Button(), true, ButtonsConfig.EditSave);
                form.Dispose();
                GC.Collect();
                //l.Add(form);
            }
            long newMem = GC.GetTotalMemory(true);
            Debug.WriteLine(newMem);
            Debug.WriteLine("("+(newMem - mem)+")");
        }

        private void Response1(object sender, EventArgs a)
        {
            Console.WriteLine("respond1");
        }

        private void Response2(object sender, EventArgs a)
        {
            Console.WriteLine("respond2");
        }
    }
}
