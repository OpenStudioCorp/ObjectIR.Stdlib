using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectIR.StdLib.Core.Core
{
    /// <summary>
    /// Provides Runtimes with a common type to represent values of any type. This is used for the IO system, and for any other system that needs to represent values of unknown types.
    /// </summary>
    public class AnyValue: IEquatable<AnyValue>, IComparable<AnyValue>
    {
        public object Value;
        public AnyValue(object value)
        {
            Value = value;
        }

        public int CompareTo(AnyValue? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;
            return 0; // We can't compare values of unknown types, so we just return 0. This means that all ValueObjects are considered equal for sorting purposes, which is fine because we don't use sorting on them.
        }

        public bool Equals(AnyValue? other)
        {
            if (ReferenceEquals(this, other)) return true;
            return false;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
