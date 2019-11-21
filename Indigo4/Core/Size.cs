using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo.Core
{
    public struct Size : IEquatable<Size>
    {
        public static readonly Size Zero = new Size { Width = 0, Height = 0 };

        public float Width { get; set; }
        public float Height { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Size size && Equals(size);
        }

        public bool Equals(Size other)
        {
            return Width == other.Width &&
                   Height == other.Height;
        }

        public override int GetHashCode()
        {
            var hashCode = 859600377;
            hashCode = hashCode * -1521134295 + Width.GetHashCode();
            hashCode = hashCode * -1521134295 + Height.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Size left, Size right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Size left, Size right)
        {
            return !(left == right);
        }
    }
}
