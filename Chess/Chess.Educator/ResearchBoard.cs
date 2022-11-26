using Chess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Educator
{
    public class ResearchBoard : Board
    {
        public ResearchBoard() : base() { }
        public ResearchBoard(ResearchBoard board) : base(board) { }

        public ResearchBoard(Board board) : base(board) { }

        public override bool Equals(Object? obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                
                ResearchBoard b = (ResearchBoard)obj;
                var a1 = this.ToShortByteArray();
                var a2 = b.ToShortByteArray();

                if (a1 == null || a2 == null)
                    return false;

                if (a1.Length != a2.Length)
                    return false;

                bool result = true;

                for(int i = 0; i < a1.Length && result; i++)
                {
                    result &= a1[i] == a2[i];
                }

                return result;
            }
        }
    }
}
