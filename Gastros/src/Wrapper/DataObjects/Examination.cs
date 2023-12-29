using System;
using System.Collections.Generic;

namespace GastrOs.Wrapper.DataObjects
{
    public class Examination : DataObject
    {
        public virtual long ID { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual DateTime? ReportDate { get; set; }
        public virtual DateTime EndoscopyDate { get; set; }
        public virtual string Doctor { get; set; }
        public virtual Department Dept { get; set; }
        public virtual EndoscopyType EndoscopyType { get; set; }
        public virtual EndoscopyDevice Device { get; set; }
        public virtual string Premedication { get; set; }
        public virtual string Notes { get; set; }
        public virtual IDictionary<SdeConcept, string> SerialisedValues { get; set; }
        public virtual SignoutEndoscopist Signout1 { get; set; }
        public virtual SignoutEndoscopist Signout2 { get; set; }
        public virtual SignoutEndoscopist Signout3 { get; set; }
        public virtual SignoutEndoscopist Signout4 { get; set; }
        public virtual string ReportText { get; set; }
        public virtual string ExamInfoText { get; set; }
        public virtual string FindingsText { get; set; }
        public virtual string InterventionsText { get; set; }
        public virtual string DiagnosesText { get; set; }
        public virtual string LogText { get; set; }

        public virtual string GetSerialisedValue(SdeConcept concept)
        {
            if (SerialisedValues != null && SerialisedValues.ContainsKey(concept))
                return SerialisedValues[concept];
            return null;
        }

        public virtual void SetSerialisedValue(SdeConcept concept, string serialValue)
        {
            if (SerialisedValues == null)
            {
                SerialisedValues = new Dictionary<SdeConcept, string>();
            }
            SerialisedValues[concept] = serialValue;
        }

        public virtual void DeleteSerialisedValue(SdeConcept concept)
        {
            SerialisedValues.Remove(concept);
        }
    }

    public class ReportExam : Examination
    {
        public virtual string RepName { get { return Patient.FullName; } }
        public virtual string RepGender { get; set; }
        public virtual string RepAge { get { return Patient.Age.ToString(); } }


        public virtual string RepDept { get { return Dept == null ? "" : Dept.Name; } }
        public virtual string RepInsurer { get { return Patient.Insurer == null ? "" : Patient.Insurer.Name; } }
        public virtual string RepPatientID { get { return Convert.ToString(Patient.PatientNo); }}
        public virtual string RepDevice { get { return Device == null ? "" : Device.Name; } }
        public virtual string RepSignoutText1 { get; set; }
        public virtual string RepSignoutText2 { get; set; }
        public virtual string RepSignoutText3 { get; set; }
        public virtual string RepSignoutText4 { get; set; }

    }
}
