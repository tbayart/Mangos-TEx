using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace Framework.MarkupExtensions
{
    public class EnumValues : MarkupExtension
    {
        private Array _values;

        public EnumValues(Type enumType)
        {
            if (enumType.IsEnum == false)
                throw new ArgumentException("EnumList : enumType is not an enum");

            _values = Enum.GetValues(enumType);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _values;
        }
    }
}
