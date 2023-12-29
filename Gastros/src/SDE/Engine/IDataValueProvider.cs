using OpenEhr.RM.DataTypes.Basic;

namespace GastrOs.Sde.Engine
{
    public interface IDataValueProvider
    {
        DataValue ToDataValue(object rawValue);
        object ToRawValue(DataValue dataValue);
    }
}
