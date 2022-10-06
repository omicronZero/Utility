using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    public static class NullableBooleanExtensions
    {
        public static bool NullOrTrue(this bool? value)
        {
            return value ?? true;
        }

        public static bool NullOrFalse(this bool? value)
        {
            return value == null || !value.Value;
        }
    }
}
