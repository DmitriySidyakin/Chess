using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Entity
{
    public class Figure
    {
        public Side Side { get; init; }

        public Figures Man { get; init; }

        public SideFigures SideMan { get; }

        public Figure(Side side, Figures man)
        {
            Side = side;
            Man = man;

            SideMan = man == Figures.Empty ? SideFigures.Empty : (SideFigures)((((byte)man) << 1 | (byte)side) - 1);
        }

        public Figure(byte figureCode)
        {
            if (figureCode == 0)
            {
                var emptyCell = new EmptyCell();

                Side = emptyCell.Side;
                Man = emptyCell.Man;
                SideMan = emptyCell.SideMan;
            }
            else
            {
                Man = (Figures)(figureCode >> 1);
                Side = (Side)(figureCode & 1);
                SideMan = (SideFigures)figureCode;
            }
        }
        
        //private Figure FromByte(byte figureCode) => figureCode switch
        //{
        //    /* Empty = */
        //    0 => new EmptyCell(), // Пустая ячейка
        //    /* WhiteKing = */
        //    1 => new KingFigure(Side.White), // (Figures.King << 1 | Side.White) - 1, // Белый Король
        //    /* WhiteQueen = */
        //    2 => new QueenFigure(Side.White), // (Figures.Queen << 1 | Side.White) - 1, // Белая Королева
        //    /* WhiteBishop = */
        //    3 => new BishopFigure(Side.White), // (Figures.Bishop << 1 | Side.White) - 1, // Белый Офицер
        //    /* WhiteKnight = */
        //    4 => new KnightFigure(Side.White), // (Figures.Knight << 1 | Side.White) - 1, // Белый Конь
        //    /* WhiteRook = */
        //    5 => new RookFigure(Side.White), // (Figures.Rook << 1 | Side.White) - 1, // Белая Тура
        //    /* WhitePawn = */
        //    6 => new PawnFigure(Side.White), // (Figures.Pawn << 1 | Side.White) - 1, // Белая Пешка
        //    /* BlackKing = */
        //    7 => new KingFigure(Side.Black), // (Figures.King << 1 | Side.Black) - 1, // Чёрный Король
        //    /* BlackQueen = */
        //    8 => new QueenFigure(Side.Black), // (Figures.Queen << 1 | Side.Black) - 1, // Чёрная Королева
        //    /* BlackBishop = */
        //    9 => new BishopFigure(Side.Black), // (Figures.Bishop << 1 | Side.Black) - 1, // Чёрный Офицер
        //    /* BlackKnight = */
        //    10 => new KnightFigure(Side.Black), // (Figures.Knight << 1 | Side.Black) - 1, // Чёрный Конь
        //    /* BlackRook = */
        //    11 => new RookFigure(Side.Black), // (Figures.Rook << 1 | Side.Black) - 1, // Чёрная Тура
        //    /* BlackPawn = */
        //    12 => new PawnFigure(Side.Black), // (Figures.Pawn << 1 | Side.Black) - 1 // Чёрная Пешка
        //    _ => throw new ArgumentOutOfRangeException(nameof(figureCode)),
        //};

        public override string ToString()
        {
           string? name = Enum.GetName<SideFigures>(SideMan);

           return name ?? "[Error]";
        }
    }
}
