using System;
using System.Windows.Forms;
using GastrOs.Sde.Support;
using GastrOs.Wrapper.DataObjects;
using NHibernate;

namespace GastrOs.Wrapper.Helpers
{
    internal class PersistHelper
    {
        /// <summary>
        /// Saves examination, adding it to its parent patient if necessary.
        /// </summary>
        /// <param name="form">Current form</param>
        /// <param name="examination">Examination to save</param>
        /// <param name="session">Current hibernate session</param>
        public static void Save(Control form, Examination examination, ISession session)
        {
            form.Cursor = Cursors.WaitCursor;

            try
            {
                ITransaction trans = session.BeginTransaction();
                if (!session.Contains(examination))
                {
                    examination.Patient.Examinations.Add(examination);
                    session.Save(examination.Patient);
                }
                session.Save(examination);
                trans.Commit();
            }
            catch (Exception e)
            {
                MessageBox.Show(form, "Problem saving examination information. " + e.Message, "Saving error");
                Logger.Error("Examination saving error (patient #" + examination.Patient.ID + ")", e);
            }

            form.Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Deletes given arbitrary number of SDE entries from examination
        /// </summary>
        /// <param name="examination">examination to delete from</param>
        /// <param name="entries">the SDE entries to delete</param>
        public static void Delete(Examination examination, params SdeConcept[] entries)
        {
            //This is the "deletion" - setting serialised value to null. Thus next time
            //SDE form loads it will be empty.
            foreach (SdeConcept entry in entries)
            {
                examination.DeleteSerialisedValue(entry);
            }
        }
    }
}
