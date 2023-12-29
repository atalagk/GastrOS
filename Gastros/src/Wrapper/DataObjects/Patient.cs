using System;
using GastrOs.Sde.Support;
using Iesi.Collections.Generic;

namespace GastrOs.Wrapper.DataObjects
{
    //public enum Gender { Male, Female, Indeterminate }
    public struct Gender
    {
        public static readonly Gender Male = new Gender { Text = "Male", Code = "m" };
        public static readonly Gender Female = new Gender { Text = "Female", Code = "f" };
        public static readonly Gender Indeterminate = new Gender { Text = "Indeterminate", Code = "i" };

        public static Gender[] GenderSet
        {
            get
            {
                return new[] { Male, Female, Indeterminate };
            }
        }

        public string Text { get; set; }
        public string Code { get; set; }
    }

    public class Insurer : DataObject
    {
        public virtual long ID { get; set; }
        public virtual string Name { get; set; }
    }

    public class Patient : DataObject
    {
        private Clinical clinical;

        public Patient()
        {
            Clinical = new Clinical();
            Examinations = new HashedSet<Examination>();
            //Birthdate = new DateTime(1950, 1, 1);
        }

        public virtual long ID {
            get; set;
        }
        public virtual string PatientNo {
            get; set;
        }
        public virtual string LastName {
            get; set;
        }
        public virtual string FirstName {
            get; set;
        }
        public virtual string MidName {
            get; set;
        }
        public virtual DateTime Birthdate {
            get; set;
        }

        public virtual string Gender {
            get; set;
        }
        public virtual string Ethnicity {
            get; set;
        }
        public virtual Insurer Insurer {
            get; set;
        }
        public virtual Clinical Clinical
        {
            get
            {
                return clinical;
            }
            set
            {
                clinical = value;
                if (clinical != null && clinical.Patient == null)
                {
                    clinical.Patient = this;
                }
            }
        }
        public virtual ISet<Examination> Examinations {
            get; set;
        }
        

        #region(derived properties)

        public virtual string FullName
        {
            get
            {
                return StringUtils.ToPrettyString(new[] { FirstName, MidName, LastName }, " ", false, true);
            }
        }

        public virtual int Age
        {
            get
            {
                return (DateTime.Now - Birthdate).Days / 365;
            }
            set
            {
                Birthdate = Birthdate.AddYears(Age - value);
            }
        }

        #endregion

        public override bool Validate(out string invalidPropName, out string details)
        {
            if (string.IsNullOrEmpty(PatientNo))
            {
                invalidPropName = "Patient Number";
                details = "Please specify the patient number";
                return false;
            }
            int result;
            if (!int.TryParse(PatientNo, out result))
            {
                invalidPropName = "Patient Number";
                details = "Patient number can only be numeric";
                return false;
            }
            if (FullName.Trim().Length == 0)
            {
                invalidPropName = "Patient's full name";
                details = "Please enter patient's name";
                return false;
            }
            invalidPropName = "";
            details = "";
            return true;
        }

        public override string ToString()
        {
            return string.Format("{0} --- {1} --- {2}", PatientNo, FullName, Birthdate.ToShortDateString());
        }
    }
}
