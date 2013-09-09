using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MangosData.Context
{
    public partial class locales_item
    {
        public string[] Names
        {
            get
            {
                return new[] { string.Empty, name_loc1, name_loc2, name_loc3, name_loc4, name_loc5, name_loc6, name_loc7, name_loc8 };
            }
        }

        public string[] Descriptions
        {
            get
            {
                return new[] { string.Empty, description_loc1, description_loc2, description_loc3, description_loc4, description_loc5, description_loc6, description_loc7, description_loc8 };
            }
        }
    }
}
