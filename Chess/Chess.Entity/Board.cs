using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Entity
{
    public class Board
    {
        public Figure[,] Positions = new Figure[BoardCellSize, BoardCellSize];

        public static readonly byte BoardCellSize = 8; 

        public Board()
        {
            // Заполняем доску пешками
            for(int i = 0; i < 8; i++) { 
                Positions[i, 6] = new PawnFigure(side: Side.White);
                Positions[i, 1] = new PawnFigure(side: Side.Black);
            }

            Positions[0, 7] = new RookFigure(side: Side.White);
            Positions[1, 7] = new KnightFigure(side: Side.White);
            Positions[2, 7] = new BishopFigure(side: Side.White);
            Positions[3, 7] = new KingFigure(side: Side.White);
            Positions[4, 7] = new QueenFigure(side: Side.White);
            Positions[5, 7] = new BishopFigure(side: Side.White);
            Positions[6, 7] = new KnightFigure(side: Side.White);
            Positions[7, 7] = new RookFigure(side: Side.White);

            Positions[0, 0] = new RookFigure(side: Side.Black);
            Positions[1, 0] = new KnightFigure(side: Side.Black);
            Positions[2, 0] = new BishopFigure(side: Side.Black);
            Positions[3, 0] = new KingFigure(side: Side.Black);
            Positions[4, 0] = new QueenFigure(side: Side.Black);
            Positions[5, 0] = new BishopFigure(side: Side.Black);
            Positions[6, 0] = new KnightFigure(side: Side.Black);
            Positions[7, 0] = new RookFigure(side: Side.Black);

            for (int i = 2; i < 6; i++)
                for (int j = 0; j < 8; j++)
                    Positions[j, i] = new EmptyCell(); 
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
