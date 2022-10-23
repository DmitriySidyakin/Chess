using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Entity
{
    public class Board
    {
        public Figure[,] Positions = new Figure[8, 8];

        public Board()
        {
            // Заполняем доску пешками
            for(int i = 0; i < 8; i++) { 
                Positions[1, i] = new PawnFigure(side: Side.White);
                Positions[6, i] = new PawnFigure(side: Side.Black); ;
            }

            Positions[0, 0] = new RookFigure(side: Side.White);
            Positions[0, 1] = new KnightFigure(side: Side.White);
            Positions[0, 2] = new BishopFigure(side: Side.White);
            Positions[0, 3] = new QueenFigure(side: Side.White);
            Positions[0, 4] = new KingFigure(side: Side.White);
            Positions[0, 5] = new BishopFigure(side: Side.White);
            Positions[0, 6] = new KnightFigure(side: Side.White);
            Positions[0, 7] = new RookFigure(side: Side.White);

            Positions[7, 0] = new RookFigure(side: Side.Black);
            Positions[7, 1] = new KnightFigure(side: Side.Black);
            Positions[7, 2] = new BishopFigure(side: Side.Black);
            Positions[7, 3] = new QueenFigure(side: Side.Black);
            Positions[7, 4] = new KingFigure(side: Side.Black);
            Positions[7, 5] = new BishopFigure(side: Side.Black);
            Positions[7, 6] = new KnightFigure(side: Side.Black);
            Positions[7, 7] = new RookFigure(side: Side.Black);

            for (int i = 3; i < 6; i++)
                for (int j = 0; j < 8; j++)
                    Positions[i, j] = new EmptyCell(); 
        }

        public byte[] ToByteArray()
        {
            byte[] bytes = new byte[64];

            int position = 0;
            for(int i = 0; i < 8; i++)
                for(int j = 0; j < 8; j++)
                {
                    bytes[position] = (byte)Positions[i, j].SideMan;
                    position++;
                }

            return bytes;
        }
    }
}
