using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using GastrOs.Sde;
using GastrOs.Sde.Configuration;
using GastrOs.Sde.Support;
using GastrOs.Wrapper.DataObjects;
using GastrOs.Wrapper.Forms;
using GastrOs.Wrapper.Forms.DataEntry;
using GastrOs.Wrapper.Helpers;
using GastrOs.Wrapper.Reports;
using Microsoft.Practices.Unity;
using NHibernate;
using NHibernate.Cfg;

namespace GastrOs.Wrapper
{
    static class MainApp
    {
        static ISession session;
        static IReportFormatter reportFormatter;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            EhrSerialiser.KnowledgePath = WrapperConfig.Get.KnowledgePath;
            DirectoryInfo pathInfo = new DirectoryInfo(EhrSerialiser.KnowledgePath);
            if (!pathInfo.Exists)
            {
                DialogResult choice =
                    MessageBox.Show(
                        "The knowledge directory '" + pathInfo.FullName + "' doesn't exist or isn't accessible. " +
                        "Please choose a different directory.", "Knowledge path inaccessible",
                        MessageBoxButtons.OKCancel);
                if (choice == DialogResult.OK)
                {
                    FolderBrowserDialog browser = new FolderBrowserDialog();
                    choice = browser.ShowDialog();
                    browser.Dispose();
                    if (choice == DialogResult.OK)
                    {
                        EhrSerialiser.KnowledgePath = browser.SelectedPath;
                        WrapperConfig.Get.KnowledgePath = browser.SelectedPath;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }

            try
            {
                //dynamically instantiate (using dependency injection) report formatter according to
                //language set in config file
                UnityContainer container = new UnityContainer();
                GastrOsConfig.UnityConfig.Configure(container, "localisation");
                reportFormatter = container.Resolve<IReportFormatter>(WrapperConfig.Get.Language);

                Thread.CurrentThread.CurrentUICulture = new CultureInfo(WrapperConfig.Get.Language);
            }
            catch (Exception e)
            {
                Logger.Error("Localisation settings error.", e);
                MessageBox.Show("There was a problem with GastrOS trying to determine the right locale. " +
                                "Please recheck the configuration settings. Details: " + e.Message,
                                "Unable to set up localisation");
                return;
            }

            //initialise language so that GastrOS service can correctly locate the
            //operational templates in appropriate languages
            GastrOsService.Language = WrapperConfig.Get.Language;

            Configuration config = new Configuration().Configure();
            
            try
            {
                ISessionFactory sessionFactory = config.BuildSessionFactory();
                session = sessionFactory.OpenSession();
            }
            catch(Exception e)
            {
                Logger.Error("GastrOS couldn't connect to the database.", e);
                MessageBox.Show("GastrOS couldn't connect to the database. Issue: " + e.Message,
                                "Unable to start GastrOS");
                return;
            }

            if (string.Equals(WrapperConfig.Get.StartupMode, "test"))
            {
                LightweightTest();
            }
            else
            {
                Application.ApplicationExit += Application_ApplicationExit;
                Application.Run(new EntryScreen(session, reportFormatter));
            }
        }

        private static void LightweightTest()
        {
            Examination exam = session.Get<Examination>(3l);
            if (exam == null) return;
            EditExaminationForm form = new EditExaminationForm(exam, session, reportFormatter, true);
            Application.Run(form);
        }

        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            WrapperConfig.Save();
            if (session != null && session.IsOpen)
            {
                session.Close();
                session.Dispose();
            }
        }
    }
}
