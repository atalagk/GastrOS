
namespace GastrOs.Wrapper.DataObjects
{
    public class Clinical : DataObject
    {
        private Patient patient;

        public virtual long ID { get; set; }
        public virtual Patient Patient {
            get
            {
                return patient;
            }
            set
            {
                patient = value;
                if (patient != null && patient.Clinical == null)
                {
                    patient.Clinical = this;
                }
            }
        }
        public virtual bool CRF { get; set; }
        public virtual bool HBV { get; set; }
        public virtual bool HCV { get; set; }
        public virtual bool HDV { get; set; }
        public virtual bool HIV { get; set; }
        public virtual string Details { get; set; }
    }
}
