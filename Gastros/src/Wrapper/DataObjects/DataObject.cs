
namespace GastrOs.Wrapper.DataObjects
{
    public abstract class DataObject
    {
        public virtual DataObject This
        {
            get
            {
                return this;
            }
        }

        public virtual bool Validate(out string invalidPropName, out string details)
        {
            invalidPropName = "";
            details = "";
            return true;
        }
    }
}
