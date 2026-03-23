using ObjectIR.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectIR.Stdlib.Math
{
    public interface Numbers
    {
        /// <summary>
        /// Calculates the sum of two integer values represented by the specified operands.
        /// </summary>
        /// <param name="a">The first operand to add.</param>
        /// <param name="b">The second operand to add.</param>
        /// <returns>A new Value<int> representing the sum of the two operands.</returns>
        public static abstract Value<int> Add(Value<int> a, Value<int> b);
        /// <summary>
        /// Adds the current value to the specified value and returns the result.
        /// </summary>
        /// <param name="B">The value to add to the current value.</param>
        /// <returns>A new Value<int> representing the sum of the current value and the specified value.</returns>
        public static abstract Value<int> Add(Value<int> B);

        public static abstract Value<string>
    }
}
