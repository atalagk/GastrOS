using System;
using GastrOs.Sde.Support;
using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.AM.OpenehrProfile.DataTypes.Quantity;
using OpenEhr.AM.OpenehrProfile.DataTypes.Text;
using OpenEhr.AssumedTypes;
using OpenEhr.DesignByContract;
using OpenEhr.Futures.OperationalTemplate;
using OpenEhr.RM.DataTypes.Basic;
using OpenEhr.RM.DataTypes.Quantity;
using OpenEhr.RM.DataTypes.Quantity.DateTime;
using OpenEhr.RM.DataTypes.Text;

namespace GastrOs.Sde.Engine
{
    public class DvTextProvider : IDataValueProvider
    {
        public DataValue ToDataValue(object rawValue)
        {
            string value = rawValue as string;
            return value == null ? null : new DvText(value);
        }

        public object ToRawValue(DataValue dataValue)
        {
            DvText text = dataValue as DvText;
            return text == null ? null : text.Value;
        }
    }

    public class DvCodedTextProvider : IDataValueProvider
    {
        private CCodePhrase codePhrase;
        private CArchetypeRoot root;

        public DvCodedTextProvider(CComplexObject constraint)
        {
            Check.Assert(constraint.RmTypeMatches<DvCodedText>());
            codePhrase = constraint.ExtractCodePhrase();
            root = constraint.GetArchetypeRoot();
        }

        public DataValue ToDataValue(object rawValue)
        {
            string value = rawValue as string;
            return value == null
                       ? null
                       : new DvCodedText(value, AomHelper.ExtractOntologyText(value, root),
                                         codePhrase.TerminologyId.Value);
        }

        public object ToRawValue(DataValue dataValue)
        {
            DvCodedText codedText = dataValue as DvCodedText;
            return codedText == null ? RmFactory.DummyCodedValue : codedText.Value;
        }
    }

    public class DvBooleanProvider : IDataValueProvider
    {
        public DataValue ToDataValue(object rawValue)
        {
            bool value = Convert.ToBoolean(rawValue);
            return new DvBoolean(value);
        }

        public object ToRawValue(DataValue dataValue)
        {
            DvBoolean dvBoolean = dataValue as DvBoolean;
            //TODO could have ramifications on validity of data (i.e. false vs. unspecified)
            return dvBoolean == null ? false : dvBoolean.Value;
        }
    }

    public class DvQuantityProvider : IDataValueProvider
    {
        private CDvQuantity constraint;

        public DvQuantityProvider(CDvQuantity constraint)
        {
            Check.Require(constraint.List != null && constraint.List.Count > 0);
            this.constraint = constraint;
        }

        public DataValue ToDataValue(object rawValue)
        {
            //NOTE Special (temporary) provision: if view's value is unset, set model value to 0.
            double quant = Convert.ToDouble(rawValue);
            //TODO should support units AND values.
            return new DvQuantity(quant, constraint.List[0].Units);
        }

        public object ToRawValue(DataValue dataValue)
        {
            DvQuantity dvQuant = dataValue as DvQuantity;
            return dvQuant != null ? dvQuant.Magnitude : 0;
        }
    }

    public class DvCountProvider : IDataValueProvider
    {
        public DataValue ToDataValue(object rawValue)
        {
            int count = Convert.ToInt32(rawValue);
            return new DvCount(count);
        }

        public object ToRawValue(DataValue dataValue)
        {
            DvCount count = dataValue as DvCount;
            return count != null ? count.Magnitude : 0;
        }
    }

    public class DvDateTimeProvider : IDataValueProvider
    {
        public DataValue ToDataValue(object rawValue)
        {
            DateTime? date = rawValue as DateTime?;
            return date.HasValue ? new DvDateTime(date.Value) : null;
        }

        public object ToRawValue(DataValue dataValue)
        {
            DvDateTime date = dataValue as DvDateTime;
            return date != null ? Iso8601DateTime.ToDateTime(new Iso8601DateTime(date.Value)) : (DateTime?)null;
        }
    }
}