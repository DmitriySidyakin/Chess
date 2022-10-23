using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Entity
{
    public enum SideFigures : byte
    {
        Empty = 0, // Пустая ячейка
        WhiteKing = (Figures.King << 1 | Side.White) - 1, // Белый Король
        WhiteQueen = (Figures.Queen << 1 | Side.White) - 1, // Белая Королева
        WhiteBishop = (Figures.Bishop << 1 | Side.White) - 1, // Белый Офицер
        WhiteKnight = (Figures.Knight << 1 | Side.White) - 1, // Белый Конь
        WhiteRook = (Figures.Rook << 1 | Side.White) - 1, // Белая Тура
        WhitePawn = (Figures.Pawn << 1 | Side.White) - 1, // Белая Пешка
        BlackKing = (Figures.King << 1 | Side.Black) - 1, // Чёрный Король
        BlackQueen = (Figures.Queen << 1 | Side.Black) - 1, // Чёрная Королева
        BlackBishop = (Figures.Bishop << 1 | Side.Black) - 1, // Чёрный Офицер
        BlackKnight = (Figures.Knight << 1 | Side.Black) - 1, // Чёрный Конь
        BlackRook = (Figures.Rook << 1 | Side.Black) - 1, // Чёрная Тура
        BlackPawn = (Figures.Pawn << 1 | Side.Black) - 1 // Чёрная Пешка
    }
}
