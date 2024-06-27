using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Entity
{
    public class CellPoint : ICloneable
    {
        public sbyte X { get; set; }
        public sbyte Y { get; set; }

        private static CellPoint unexisted  = new CellPoint() { X = -1, Y = -1 };
        public static CellPoint Unexisted { get => unexisted; }

        public object Clone()
        {
            return new CellPoint() { X = X, Y = Y };
        }

        public override bool Equals(object obj) => this.Equals(obj as CellPoint);

        public bool Equals(CellPoint p)
        {
            if (p is null)
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, p))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != p.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return (X == p.X) && (Y == p.Y);
        }

        public override int GetHashCode() => (X, Y).GetHashCode();

        public static bool operator ==(CellPoint lhs, CellPoint rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(CellPoint lhs, CellPoint rhs) => !(lhs == rhs);
    }
}
