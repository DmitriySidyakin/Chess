using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Entity
{
    public class Board
    {
        /// <summary>
        /// Массив фигур расположенных на шахмотной доске.
        /// </summary>
        public Figure[,] Positions = new Figure[CellBoardSize, CellBoardSize];

        /// <summary>
        /// Константа, обозначающая ширину и высоту шахмотной доски в ячейках.
        /// </summary>
        public static readonly byte CellBoardSize = 8;

        /// <summary>
        /// Сторона (белые или чёрные), которая должна сделать ход.
        /// </summary>
        public Side CurrentStepSide { get; set; }

        /// <summary>
        /// Конструктор по умолчанию, создающий доску с начальным положением фигур.
        /// </summary>
        public Board()
        {
            // Заполняем доску пешками
            for (int i = 0; i < 8; i++)
            {
                Positions[i, 6] = new PawnFigure(side: Side.White);
                Positions[i, 1] = new PawnFigure(side: Side.Black);
            }

            Positions[0, 7] = new RookFigure(side: Side.White);
            Positions[1, 7] = new KnightFigure(side: Side.White);
            Positions[2, 7] = new BishopFigure(side: Side.White);
            Positions[3, 7] = new QueenFigure(side: Side.White);
            Positions[4, 7] = new KingFigure(side: Side.White);
            Positions[5, 7] = new BishopFigure(side: Side.White);
            Positions[6, 7] = new KnightFigure(side: Side.White);
            Positions[7, 7] = new RookFigure(side: Side.White);

            Positions[0, 0] = new RookFigure(side: Side.Black);
            Positions[1, 0] = new KnightFigure(side: Side.Black);
            Positions[2, 0] = new BishopFigure(side: Side.Black);
            Positions[3, 0] = new QueenFigure(side: Side.Black);
            Positions[4, 0] = new KingFigure(side: Side.Black);
            Positions[5, 0] = new BishopFigure(side: Side.Black);
            Positions[6, 0] = new KnightFigure(side: Side.Black);
            Positions[7, 0] = new RookFigure(side: Side.Black);

            for (int i = 2; i < 6; i++)
                for (int j = 0; j < 8; j++)
                    Positions[j, i] = new EmptyCell();

            CurrentStepSide = Side.White;
        }

        /// <summary>
        /// Конструктор для создания доски из массива байт.
        /// </summary>
        /// <param name="boardBytes">Байты сериализованной доски</param>
        /// <exception cref="ArgumentException">Исключение возникает, если передано не правильное количество байт</exception>
        public Board(byte[] boardBytes)
        {
            if (boardBytes.Length != 33)
                throw new ArgumentException("The bytes count does not equal 33!");

            int step = 0;
            for (int row = 1; row <= 8; row++)
                for (int col = 1; col <= 8; col++)
                {
                    if (step == 0)
                    {
                        Positions[col - 1, row - 1] = new Figure((byte)(boardBytes[((row - 1) * 8 + col - 1) / 2] >> 4));
                        step++;
                    }
                    else
                    {
                        Positions[col - 1, row - 1] = new Figure((byte)(boardBytes[((row - 1) * 8 + col - 1) / 2] & 0x0F));
                        step = 0;
                    }
                }

            CurrentStepSide = (Side)boardBytes[32];
        }

        /// <summary>
        /// Получает строковое имя ячейки шахматной доски.
        /// </summary>
        /// <param name="column">Числовой идентификатор колонки</param>
        /// <param name="row">Числовой идентификатор строки</param>
        /// <returns>Имя ячейки</returns>
        public static string GetStringCellName(byte column, byte row) => GetColumnName(column) + GetRowName(row);

        /// <summary>
        /// Получить текстовое имя колонки шахматной доски.
        /// </summary>
        /// <param name="column">Числовой идентификатор колонки</param>
        /// <returns>Имя колонки ячейки шахматной доски</returns>
        public static string GetColumnName(byte column) => column switch
        {
            0 => "A",
            1 => "B",
            2 => "C",
            3 => "D",
            4 => "E",
            5 => "F",
            6 => "G",
            7 => "H",
            _ => string.Empty,
        };

        /// <summary>
        /// Получить текстовое имя строки шахматной доски.
        /// </summary>
        /// <param name="row">Числовой идентификатор строки</param>
        /// <returns>Имя строки ячейки шахматной доски</returns>
        public static string GetRowName(byte row) => (8 - row).ToString();

        /// <summary>
        /// Сериализация доски в массив байт.
        /// </summary>
        /// <returns>Массив байт сериализованной доски.</returns>
        public byte[] ToByteArray()
        {
            byte[] bytes = new byte[33];

            int position = 0;
            int step = 0;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if (step == 0)
                    {
                        bytes[position] = 0;
                        bytes[position] ^= (byte)Positions[j, i].SideMan;
                        step++;
                    }
                    else
                    {
                        bytes[position] <<= 4;
                        bytes[position] ^= (byte)Positions[j, i].SideMan;
                        step = 0;
                        position++;
                    }
                }

            bytes[32] = (byte)CurrentStepSide;

            return bytes;
        }

        /// <summary>
        /// Оператор стравнения шахматных досок, с учётом текущей стороны, которая делает ход.
        /// </summary>
        /// <param name="obj">Доска для сравнения.</param>
        /// <returns>true - если равны, иначе false.</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            if (obj is not Board board)
                return false;

            bool result = true;

            for (int row = 0; row < 8 && result; row++)
                for (int column = 0; column < 8 && result; column++)
                {
                    result = Positions[column, row].SideMan == board.Positions[column, row].SideMan;
                }

            result = result && (CurrentStepSide == board.CurrentStepSide);

            return result;
        }

        /// <summary>
        /// Хешкод шахмотной достки.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            byte[] bytes = new byte[4];

            byte position = 0;

            for (int row = 7; row >= 0; row--)
                for (int column = 0; column < 8; column++)
                {
                    bytes[position] += (byte)Positions[column, row].SideMan;
                    bytes[position] ^= (byte)Positions[column, row].SideMan;
                    position++;
                    if (position >= 4)
                        position = 0;
                }

            int result = 0;
            for (byte byteNum = 3; byteNum >= 0; byteNum--)
            {
                result ^= bytes[byteNum];
                result <<= 8;
            }

            return result;
        }

        /// <summary>
        /// Шахматная доска в строку.
        /// </summary>
        /// <returns>Строка представления шахмотной доски, отформатированная символами переноса строки.</returns>
        public override string ToString()
        {
            string result = "Step For: " + Enum.GetName<Side>(CurrentStepSide) + Environment.NewLine;

            for (int row = 0; row < CellBoardSize; row++)
            {
                for (int column = 0; column < CellBoardSize; column++)
                {
                    result += Positions[column, row].ToString();

                    if (column < 7)
                    {
                        result += ", ";
                    }
                }

                result += Environment.NewLine;
            }

            return result;
        }

        /// <summary>
        /// Получить все доступные ходы.
        /// </summary>
        /// <param name="side">Сторона, которах ходит</param>
        /// <returns>Все доступные ходы для стороны</returns>
        public Dictionary<CellPoint, List<CellPoint>> GetAvailiableSteps(Side side)
        {
            Dictionary<CellPoint, List<CellPoint>> result = new Dictionary<CellPoint, List<CellPoint>>();

            for (int row = 0; row < CellBoardSize; row++)
                for (int column = 0; column < CellBoardSize; column++)
                {
                    if (Positions[column, row].Man != Figures.Empty && Positions[column, row].Side == side)
                    {
                        var current = new CellPoint() { X = (sbyte)column, Y = (sbyte)row };
                        result.Add(current, GetAvailiableStepsFor(current));
                    }
                }

            return result;
        }

        /// <summary>
        /// Получить все доступные ходы. Не учитывая то, что король не может ходить под шах.
        /// </summary>
        /// <param name="side">Сторона, которах ходит</param>
        /// <returns>Все доступные ходы для стороны. Не учитывая то, что король не может ходить под шах.</returns>
        private Dictionary<CellPoint, List<CellPoint>> GetAvailiableStepsPre(Side side)
        {
            Dictionary<CellPoint, List<CellPoint>> result = new Dictionary<CellPoint, List<CellPoint>>();

            for(int row = 0; row < CellBoardSize; row++)
                for(int column = 0; column < CellBoardSize; column++)
                {
                    if (Positions[column, row].Man != Figures.Empty && Positions[column, row].Side == side)
                    {
                        var current = new CellPoint() { X = (sbyte)column, Y = (sbyte)row };
                        result.Add(current, GetAvailiableStepsForPre(current));
                    }
                }

            return result;
        }

        /// <summary>
        /// Вернуть противоположную сторону игры, к примеру, для белых это чёрные.
        /// </summary>
        /// <param name="side">Текущая сторона</param>
        /// <returns>Противоположная сторона</returns>
        private static Side GetOppositeSide(Side side)
        {
            return side == Side.White ? Side.Black : Side.White;
        }

        /// <summary>
        /// Получаем доступные шахматные ходы, без рокировки.
        /// </summary>
        /// <param name="start">Начальная позиция фигуры для хода</param>
        /// <returns>Список доступных ходов</returns>
        public List<CellPoint> GetAvailiableStepsFor(CellPoint start)
        {
            var figure = Positions[start.X, start.Y];
            var result = GetAvailiableStepsForPre(start);

            if(figure.Man == Figures.King)
            {
                Side side = figure.Side;
                Side oppositeSide = GetOppositeSide(side);
                var kingResults = new List<CellPoint>();

                // Отфильтровать результаты, где король ходит под удар.
                Dictionary<CellPoint, List<CellPoint>> allOppositeSteps = GetAvailiableStepsPre(oppositeSide);

                foreach (var step in allOppositeSteps)
                {
                    List<CellPoint> currentOppositeSteps = step.Value;

                    foreach (var possibleKingStep in kingResults)
                    {
                        if (currentOppositeSteps.Where(c => c.X == possibleKingStep.X && c.Y == possibleKingStep.Y).Count() == 0)
                        {
                            kingResults.Add(possibleKingStep);
                        }
                    }
                }

                return kingResults;
            }

            return result;
            
        }

        /// <summary>
        /// Получаем доступные шахматные ходы, без рокировки, с возможностью короля ходить под удар.
        /// </summary>
        /// <param name="start">Начальная позиция фигуры для хода</param>
        /// <returns>Список доступных ходов, с возможностью короля ходить под урар!</returns>
        private List<CellPoint> GetAvailiableStepsForPre(CellPoint start)
        {
            var figure = Positions[start.X, start.Y];

            Side side = figure.Side;
            Side oppositeSide = GetOppositeSide(side);

            var result = new List<CellPoint>();

            switch (figure.Man)
            {
                case Figures.Pawn:

                    var pawnResults = new List<CellPoint>();

                    // Проверка на возможность съесть
                    var pawnEatResults = new List<CellPoint>();

                    // Возможности есть
                    if (side == Side.White)
                    {
                        pawnEatResults.Add(new CellPoint() { X = (sbyte)(start.X - 1), Y = (sbyte)(start.Y - 1) });
                        pawnEatResults.Add(new CellPoint() { X = (sbyte)(start.X + 1), Y = (sbyte)(start.Y - 1) });
                    }
                    else
                    {
                        pawnEatResults.Add(new CellPoint() { X = (sbyte)(start.X - 1), Y = (sbyte)(start.Y + 1) });
                        pawnEatResults.Add(new CellPoint() { X = (sbyte)(start.X + 1), Y = (sbyte)(start.Y + 1) });
                    }

                    foreach (var step in pawnEatResults)
                    {
                        if (Positions[step.X, step.Y].Man != Figures.Empty && Positions[step.X, step.Y].Side == oppositeSide)
                        {
                            pawnResults.Add(new CellPoint() { X = step.X, Y = step.Y });
                        }
                    }

                    // Получение возможного хода

                    int maxDeltaY;
                    if((start.X == 6 && side == Side.White) || (start.X == 1 && side == Side.Black))
                    {
                        maxDeltaY = 2;
                    }
                    else
                    {
                        maxDeltaY = 1;
                    }

                    for (int i = 1; i < maxDeltaY + 1; i++)
                    {
                        CellPoint cp;

                        if (side == Side.White)
                        {
                            cp = new CellPoint() { X = start.X, Y = (sbyte)(start.Y - i) };
                        }
                        else
                        {
                            cp = new CellPoint() { X = start.X, Y = (sbyte)(start.Y + i) };
                        }

                        if ((cp.X > Board.CellBoardSize - 1) || (cp.Y > Board.CellBoardSize - 1) || cp.X < 0 || cp.Y < 0)
                            break;

                        if (Positions[cp.X, cp.Y].Man == Figures.Empty)
                        {
                            pawnResults.Add(cp);
                        }
                        else break;
                    }

                    result.AddRange(pawnResults);

                    break;
                case Figures.Knight:

                    var knightResults = new List<CellPoint>();

                    List<CellPoint> availiableKnightDeltas = new List<CellPoint>()
                    {
                        new CellPoint() { X = -1, Y =  2 },
                        new CellPoint() { X = -1, Y = -2 },
                        new CellPoint() { X = -2, Y =  1 },
                        new CellPoint() { X = -2, Y = -1 },
                        new CellPoint() { X =  1, Y =  2 },
                        new CellPoint() { X =  1, Y = -2 },
                        new CellPoint() { X =  2, Y =  1 },
                        new CellPoint() { X =  2, Y = -1 }
                    };

                    foreach (var delta in availiableKnightDeltas)
                    {
                        var end = new CellPoint() { X = (sbyte)(start.X + delta.X), Y = (sbyte)(start.Y + delta.Y) };

                        if ((end.X > Board.CellBoardSize - 1) || (end.Y > Board.CellBoardSize - 1) || end.X < 0 || end.Y < 0)
                            continue;

                        if (Positions[end.X, end.Y].Man == Figures.Empty || Positions[end.X, end.Y].Side == oppositeSide)
                        {
                            knightResults.Add(end);
                            continue;
                        }
                    }

                    result.AddRange(knightResults);

                    break;
                case Figures.Bishop:

                    var bishopResults = new List<CellPoint>();

                    // Рассмотрим 4 диагонали

                    for (int i = 1; i < Board.CellBoardSize; i++)
                    {
                        CellPoint cp;

                        cp = new CellPoint() { X = (sbyte)(start.X + i), Y = (sbyte)(start.Y + i) };

                        if ((cp.X > Board.CellBoardSize - 1) || (cp.Y > Board.CellBoardSize - 1) || cp.X < 0 || cp.Y < 0)
                            break;

                        if (Positions[cp.X, cp.Y].Man == Figures.Empty)
                        {
                            bishopResults.Add(cp);
                        }
                        else if (Positions[cp.X, cp.Y].Side == oppositeSide)
                        {
                            bishopResults.Add(new CellPoint() { X = cp.X, Y = cp.Y });
                            break;
                        }
                        else break;
                    }

                    for (int i = 1; i < Board.CellBoardSize; i++)
                    {
                        CellPoint cp;

                        cp = new CellPoint() { X = (sbyte)(start.X - i), Y = (sbyte)(start.Y + i) };

                        if ((cp.X > Board.CellBoardSize - 1) || (cp.Y > Board.CellBoardSize - 1) || cp.X < 0 || cp.Y < 0)
                            break;

                        if (Positions[cp.X, cp.Y].Man == Figures.Empty)
                        {
                            bishopResults.Add(cp);
                        }
                        else if (Positions[cp.X, cp.Y].Side == oppositeSide)
                        {
                            bishopResults.Add(new CellPoint() { X = cp.X, Y = cp.Y });
                            break;
                        }
                        else break;
                    }

                    for (int i = 1; i < Board.CellBoardSize; i++)
                    {
                        CellPoint cp;

                        cp = new CellPoint() { X = (sbyte)(start.X + i), Y = (sbyte)(start.Y - i) };

                        if ((cp.X > Board.CellBoardSize - 1) || (cp.Y > Board.CellBoardSize - 1) || cp.X < 0 || cp.Y < 0)
                            break;

                        if (Positions[cp.X, cp.Y].Man == Figures.Empty)
                        {
                            bishopResults.Add(cp);
                        }
                        else if (Positions[cp.X, cp.Y].Side == oppositeSide)
                        {
                            bishopResults.Add(new CellPoint() { X = cp.X, Y = cp.Y });
                            break;
                        }
                        else break;
                    }

                    for (int i = 1; i < Board.CellBoardSize; i++)
                    {
                        CellPoint cp;

                        cp = new CellPoint() { X = (sbyte)(start.X - i), Y = (sbyte)(start.Y - i) };

                        if ((cp.X > Board.CellBoardSize - 1) || (cp.Y > Board.CellBoardSize - 1) || cp.X < 0 || cp.Y < 0)
                            break;

                        if (Positions[cp.X, cp.Y].Man == Figures.Empty)
                        {
                            bishopResults.Add(cp);
                        }
                        else if (Positions[cp.X, cp.Y].Side == oppositeSide)
                        {
                            bishopResults.Add(new CellPoint() { X = cp.X, Y = cp.Y });
                        }
                        else break;
                    }

                    result.AddRange(bishopResults);

                    break;
                case Figures.King:

                    var kingResults = new List<CellPoint>();

                    List<CellPoint> availiableKingDeltas = new List<CellPoint>()
                    {
                        new CellPoint() { X = 1, Y =  1 },
                        new CellPoint() { X = -1, Y =  1 },
                        new CellPoint() { X = 1, Y =  -1 },
                        new CellPoint() { X = -1, Y =  -1 },
                        new CellPoint() { X = 0, Y =  -1 },
                        new CellPoint() { X = 0, Y =  1 },
                        new CellPoint() { X = -1, Y =  0 },
                        new CellPoint() { X = 1, Y =  0 },

                    };

                    foreach (var delta in availiableKingDeltas)
                    {
                        var end = new CellPoint() { X = (sbyte)(start.X + delta.X), Y = (sbyte)(start.Y + delta.Y) };

                        if ((end.X > Board.CellBoardSize - 1) || (end.Y > Board.CellBoardSize - 1) || end.X < 0 || end.Y < 0)
                            continue;

                        if (Positions[end.X, end.Y].Man == Figures.Empty || Positions[end.X, end.Y].Side == oppositeSide)
                        {
                            kingResults.Add(end);
                            continue;
                        }
                    }

                    result.AddRange(kingResults);

                    // Рокировка находится в отдельном методе, т.к. подразумевает движение двух фигур.
                    // В данном методе король может ходить под удар.
                    break;
                case Figures.Queen:

                    var queenResults = new List<CellPoint>();

                    // Рассмотрим 4 диагонали

                    for (int i = 1; i < Board.CellBoardSize; i++)
                    {
                        CellPoint cp;

                        cp = new CellPoint() { X = (sbyte)(start.X + i), Y = (sbyte)(start.Y + i) };

                        if ((cp.X > Board.CellBoardSize - 1) || (cp.Y > Board.CellBoardSize - 1) || cp.X < 0 || cp.Y < 0)
                            break;

                        if (Positions[cp.X, cp.Y].Man == Figures.Empty)
                        {
                            queenResults.Add(cp);
                        }
                        else if (Positions[cp.X, cp.Y].Side == oppositeSide)
                        {
                            queenResults.Add(new CellPoint() { X = cp.X, Y = cp.Y });
                            break;
                        }
                        else break;
                    }

                    for (int i = 1; i < Board.CellBoardSize; i++)
                    {
                        CellPoint cp;

                        cp = new CellPoint() { X = (sbyte)(start.X - i), Y = (sbyte)(start.Y + i) };

                        if ((cp.X > Board.CellBoardSize - 1) || (cp.Y > Board.CellBoardSize - 1) || cp.X < 0 || cp.Y < 0)
                            break;

                        if (Positions[cp.X, cp.Y].Man == Figures.Empty)
                        {
                            queenResults.Add(cp);
                        }
                        else if (Positions[cp.X, cp.Y].Side == oppositeSide)
                        {
                            queenResults.Add(new CellPoint() { X = cp.X, Y = cp.Y });
                            break;
                        }
                        else break;
                    }

                    for (int i = 1; i < Board.CellBoardSize; i++)
                    {
                        CellPoint cp;

                        cp = new CellPoint() { X = (sbyte)(start.X + i), Y = (sbyte)(start.Y - i) };

                        if ((cp.X > Board.CellBoardSize - 1) || (cp.Y > Board.CellBoardSize - 1) || cp.X < 0 || cp.Y < 0)
                            break;

                        if (Positions[cp.X, cp.Y].Man == Figures.Empty)
                        {
                            queenResults.Add(cp);
                        }
                        else if (Positions[cp.X, cp.Y].Side == oppositeSide)
                        {
                            queenResults.Add(new CellPoint() { X = cp.X, Y = cp.Y });
                            break;
                        }
                        else break;
                    }

                    for (int i = 1; i < Board.CellBoardSize; i++)
                    {
                        CellPoint cp;

                        cp = new CellPoint() { X = (sbyte)(start.X - i), Y = (sbyte)(start.Y - i) };

                        if ((cp.X > Board.CellBoardSize - 1) || (cp.Y > Board.CellBoardSize - 1) || cp.X < 0 || cp.Y < 0)
                            break;

                        if (Positions[cp.X, cp.Y].Man == Figures.Empty)
                        {
                            queenResults.Add(cp);
                        }
                        else if (Positions[cp.X, cp.Y].Side == oppositeSide)
                        {
                            queenResults.Add(new CellPoint() { X = cp.X, Y = cp.Y });
                            break;
                        }
                        else break;
                    }

                    // Рассмотрим 4 оси

                    for (int i = 1; i < Board.CellBoardSize; i++)
                    {
                        CellPoint cp;

                        cp = new CellPoint() { X = (sbyte)(start.X), Y = (sbyte)(start.Y + i) };

                        if ((cp.X > Board.CellBoardSize - 1) || (cp.Y > Board.CellBoardSize - 1) || cp.X < 0 || cp.Y < 0)
                            break;

                        if (Positions[cp.X, cp.Y].Man == Figures.Empty)
                        {
                            queenResults.Add(cp);
                        }
                        else if (Positions[cp.X, cp.Y].Side == oppositeSide)
                        {
                            queenResults.Add(new CellPoint() { X = cp.X, Y = cp.Y });
                            break;
                        }
                        else break;
                    }

                    for (int i = 1; i < Board.CellBoardSize; i++)
                    {
                        CellPoint cp;

                        cp = new CellPoint() { X = (sbyte)(start.X), Y = (sbyte)(start.Y - i) };

                        if ((cp.X > Board.CellBoardSize - 1) || (cp.Y > Board.CellBoardSize - 1) || cp.X < 0 || cp.Y < 0)
                            break;

                        if (Positions[cp.X, cp.Y].Man == Figures.Empty)
                        {
                            queenResults.Add(cp);
                        }
                        else if (Positions[cp.X, cp.Y].Side == oppositeSide)
                        {

                            queenResults.Add(new CellPoint() { X = cp.X, Y = cp.Y });
                            break;
                        }
                        else break;
                    }

                    for (int i = 1; i < Board.CellBoardSize; i++)
                    {
                        CellPoint cp;

                        cp = new CellPoint() { X = (sbyte)(start.X + i), Y = (sbyte)(start.Y) };

                        if ((cp.X > Board.CellBoardSize - 1) || (cp.Y > Board.CellBoardSize - 1) || cp.X < 0 || cp.Y < 0)
                            break;

                        if (Positions[cp.X, cp.Y].Man == Figures.Empty)
                        {
                            queenResults.Add(cp);
                        }
                        else if (Positions[cp.X, cp.Y].Side == oppositeSide)
                        {
                            queenResults.Add(new CellPoint() { X = cp.X, Y = cp.Y });
                            break;
                        }
                        else break;
                    }

                    for (int i = 1; i < Board.CellBoardSize; i++)
                    {
                        CellPoint cp;

                        cp = new CellPoint() { X = (sbyte)(start.X - i), Y = (sbyte)(start.Y) };

                        if ((cp.X > Board.CellBoardSize - 1) || (cp.Y > Board.CellBoardSize - 1) || cp.X < 0 || cp.Y < 0)
                            break;

                        if (Positions[cp.X, cp.Y].Man == Figures.Empty)
                        {
                            queenResults.Add(cp);
                        }
                        else if (Positions[cp.X, cp.Y].Side == oppositeSide)
                        {
                            queenResults.Add(new CellPoint() { X = cp.X, Y = cp.Y });
                            break;
                        }
                        else break;
                    }

                    result.AddRange(queenResults);

                    break;
                case Figures.Rook:

                    var rookResults = new List<CellPoint>();

                    // Рассмотрим 4 оси

                    for (int i = 1; i < Board.CellBoardSize; i++)
                    {
                        CellPoint cp;

                        cp = new CellPoint() { X = (sbyte)(start.X), Y = (sbyte)(start.Y + i) };

                        if ((cp.X > Board.CellBoardSize - 1) || (cp.Y > Board.CellBoardSize - 1) || cp.X < 0 || cp.Y < 0)
                            break;

                        if (Positions[cp.X, cp.Y].Man == Figures.Empty)
                        {
                            rookResults.Add(cp);
                        }
                        else if (Positions[cp.X, cp.Y].Side == oppositeSide)
                        {
                            rookResults.Add(new CellPoint() { X = cp.X, Y = cp.Y });
                            break;
                        }
                        else break;
                    }

                    for (int i = 1; i < Board.CellBoardSize; i++)
                    {
                        CellPoint cp;

                        cp = new CellPoint() { X = (sbyte)(start.X), Y = (sbyte)(start.Y - i) };

                        if ((cp.X > Board.CellBoardSize - 1) || (cp.Y > Board.CellBoardSize - 1) || cp.X < 0 || cp.Y < 0)
                            break;

                        if (Positions[cp.X, cp.Y].Man == Figures.Empty)
                        {
                            rookResults.Add(cp);
                        }
                        else if (Positions[cp.X, cp.Y].Side == oppositeSide)
                        {
                            rookResults.Add(new CellPoint() { X = cp.X, Y = cp.Y });
                            break;
                        }
                        else break;
                    }

                    for (int i = 1; i < Board.CellBoardSize; i++)
                    {
                        CellPoint cp;

                        cp = new CellPoint() { X = (sbyte)(start.X + i), Y = (sbyte)(start.Y) };

                        if ((cp.X > Board.CellBoardSize - 1) || (cp.Y > Board.CellBoardSize - 1) || cp.X < 0 || cp.Y < 0)
                            break;

                        if (Positions[cp.X, cp.Y].Man == Figures.Empty)
                        {
                            rookResults.Add(cp);
                        }
                        else if (Positions[cp.X, cp.Y].Side == oppositeSide)
                        {
                            rookResults.Add(new CellPoint() { X = cp.X, Y = cp.Y });
                            break;
                        }
                        else break;
                    }

                    for (int i = 1; i < Board.CellBoardSize; i++)
                    {
                        CellPoint cp;

                        cp = new CellPoint() { X = (sbyte)(start.X - i), Y = (sbyte)(start.Y) };

                        if ((cp.X > Board.CellBoardSize - 1) || (cp.Y > Board.CellBoardSize - 1) || cp.X < 0 || cp.Y < 0)
                            break;

                        if (Positions[cp.X, cp.Y].Man == Figures.Empty)
                        {
                            rookResults.Add(cp);
                        }
                        else if (Positions[cp.X, cp.Y].Side == oppositeSide)
                        {
                            rookResults.Add(new CellPoint() { X = cp.X, Y = cp.Y });
                            break;
                        }
                        else break;
                    }

                    result.AddRange(rookResults);

                    break;
                default: break;
            }

            return result;
        }

        /// <summary>
        /// Доступна ли рокировка. Внимание: проверяет доступна ли рокировка, т.к. она не входит в метод GetAvailiableSteps.
        /// </summary>
        public bool IsCastlingAvailable(CellPoint start)
        {
            var figure = Positions[start.X, start.Y];
            Side side = figure.Side;

            // Короткая рокировка
            if(side == Side.White)
            {
                if(Positions[start.X, start.Y] == Positions[4, 0] &&
                   Positions[4, 0].Man == Figures.King &&
                   Positions[7, 0].Man == Figures.Rook &&
                   Positions[5, 0].Man == Figures.Empty &&
                   Positions[6, 0].Man == Figures.Empty) return true;
            } else
            {
                if (Positions[start.X, start.Y] == Positions[4, 7] &&
                   Positions[4, 7].Man == Figures.King &&
                   Positions[7, 7].Man == Figures.Rook &&
                   Positions[5, 7].Man == Figures.Empty &&
                   Positions[6, 7].Man == Figures.Empty) return true;
            }

            // Длинная рокировка
            if (side == Side.White)
            {
                if (Positions[start.X, start.Y] == Positions[4, 0] &&
                   Positions[4, 0].Man == Figures.King &&
                   Positions[0, 0].Man == Figures.Rook &&
                   Positions[3, 0].Man == Figures.Empty &&
                   Positions[2, 0].Man == Figures.Empty &&
                   Positions[1, 0].Man == Figures.Empty) return true;
            }
            else
            {
                if (Positions[start.X, start.Y] == Positions[4, 7] &&
                   Positions[4, 7].Man == Figures.King &&
                   Positions[0, 7].Man == Figures.Rook &&
                   Positions[3, 7].Man == Figures.Empty &&
                   Positions[2, 7].Man == Figures.Empty &&
                   Positions[1, 7].Man == Figures.Empty) return true;
            }

            return false;
        }

        /// <summary>
        /// Возвращает доски после рокировки.
        /// </summary>
        /// <param name="start">Позиция короля, по ней определяется сторона (белые, чёрные) для рокировки</param>
        /// <returns>Доски, после применения возможных рокировок. Возможны 0, 1на, 2е рокировки</returns>
        public List<Board> GetBoardsAfterCastling(CellPoint start)
        {
            var figure = Positions[start.X, start.Y];

            if (figure.Man != Figures.King)
                return new List<Board>();

            List<Board> resultBoards = new List<Board>();
            Side side = figure.Side;

            // Короткая рокировка
            if (side == Side.White)
            {
                if (Positions[start.X, start.Y] == Positions[4, 0] &&
                   Positions[4, 0].Man == Figures.King &&
                   Positions[7, 0].Man == Figures.Rook &&
                   Positions[5, 0].Man == Figures.Empty &&
                   Positions[6, 0].Man == Figures.Empty)
                {
                    Board newBoard = new Board(boardBytes: this.ToByteArray());

                    newBoard.Positions[4, 0] = new EmptyCell();
                    newBoard.Positions[7, 0] = new EmptyCell();
                    newBoard.Positions[5, 0] = new RookFigure(side: Side.White);
                    newBoard.Positions[6, 0] = new KingFigure(side: Side.White);

                    resultBoards.Add(newBoard);
                }
            }
            else
            {
                if (Positions[start.X, start.Y] == Positions[4, 7] &&
                   Positions[4, 7].Man == Figures.King &&
                   Positions[7, 7].Man == Figures.Rook &&
                   Positions[5, 7].Man == Figures.Empty &&
                   Positions[6, 7].Man == Figures.Empty)
                {
                    Board newBoard = new Board(boardBytes: this.ToByteArray());

                    newBoard.Positions[4, 7] = new EmptyCell();
                    newBoard.Positions[7, 7] = new EmptyCell();
                    newBoard.Positions[5, 7] = new RookFigure(side: Side.Black);
                    newBoard.Positions[6, 7] = new KingFigure(side: Side.Black);

                    resultBoards.Add(newBoard);
                }
            }

            // Длинная рокировка
            if (side == Side.White)
            {
                if (Positions[start.X, start.Y] == Positions[4, 0] &&
                   Positions[4, 0].Man == Figures.King &&
                   Positions[0, 0].Man == Figures.Rook &&
                   Positions[3, 0].Man == Figures.Empty &&
                   Positions[2, 0].Man == Figures.Empty &&
                   Positions[1, 0].Man == Figures.Empty)
                {
                    Board newBoard = new Board(boardBytes: this.ToByteArray());

                    newBoard.Positions[1, 0] = new EmptyCell();
                    newBoard.Positions[4, 0] = new EmptyCell();
                    newBoard.Positions[0, 0] = new EmptyCell();
                    newBoard.Positions[3, 0] = new RookFigure(side: Side.White);
                    newBoard.Positions[2, 0] = new KingFigure(side: Side.White);

                    resultBoards.Add(newBoard);
                }
            }
            else
            {
                if (Positions[start.X, start.Y] == Positions[4, 7] &&
                   Positions[4, 7].Man == Figures.King &&
                   Positions[0, 7].Man == Figures.Rook &&
                   Positions[3, 7].Man == Figures.Empty &&
                   Positions[2, 7].Man == Figures.Empty &&
                   Positions[1, 7].Man == Figures.Empty)
                {
                    Board newBoard = new Board(boardBytes: this.ToByteArray());

                    newBoard.Positions[1, 7] = new EmptyCell();
                    newBoard.Positions[4, 7] = new EmptyCell();
                    newBoard.Positions[0, 7] = new EmptyCell();
                    newBoard.Positions[3, 7] = new RookFigure(side: Side.Black);
                    newBoard.Positions[2, 7] = new KingFigure(side: Side.Black);

                    resultBoards.Add(newBoard);
                }
            }

            return resultBoards;
        }

        /// <summary>
        /// Возвращает положения туры (Rook) до рокировки.
        /// </summary>
        /// <param name="start">Позиция короля, по ней определяется сторона (белые, чёрные) для рокировки</param>
        /// <returns>Положения туры (Rook) до рокировки. Возможны 0, 1на, 2е рокировки</returns>
        public List<CellPoint> GetRookPositionsAfterCastling(CellPoint start)
        {
            var figure = Positions[start.X, start.Y];

            List<CellPoint> results = new List<CellPoint>();
            Side side = figure.Side;

            // Короткая рокировка
            if (side == Side.White)
            {
                if (Positions[start.X, start.Y] == Positions[4, 0] &&
                   Positions[4, 0].Man == Figures.King &&
                   Positions[7, 0].Man == Figures.Rook &&
                   Positions[5, 0].Man == Figures.Empty &&
                   Positions[6, 0].Man == Figures.Empty)
                {
                    results.Add(new CellPoint() { X = 7, Y = 0});
                }
            }
            else
            {
                if (Positions[start.X, start.Y] == Positions[4, 7] &&
                   Positions[4, 7].Man == Figures.King &&
                   Positions[7, 7].Man == Figures.Rook &&
                   Positions[5, 7].Man == Figures.Empty &&
                   Positions[6, 7].Man == Figures.Empty)
                {
                    results.Add(new CellPoint() { X = 7, Y = 7 });
                }
            }

            // Длинная рокировка
            if (side == Side.White)
            {
                if (Positions[start.X, start.Y] == Positions[4, 0] &&
                   Positions[4, 0].Man == Figures.King &&
                   Positions[0, 0].Man == Figures.Rook &&
                   Positions[3, 0].Man == Figures.Empty &&
                   Positions[2, 0].Man == Figures.Empty &&
                   Positions[1, 0].Man == Figures.Empty)
                {
                    results.Add(new CellPoint() { X = 0, Y = 0 });
                }
            }
            else
            {
                if (Positions[start.X, start.Y] == Positions[4, 7] &&
                   Positions[4, 7].Man == Figures.King &&
                   Positions[0, 7].Man == Figures.Rook &&
                   Positions[3, 7].Man == Figures.Empty &&
                   Positions[2, 7].Man == Figures.Empty &&
                   Positions[1, 7].Man == Figures.Empty)
                {
                    results.Add(new CellPoint() { X = 0, Y = 7 });
                }
            }

            return results;
        }

        /// <summary>
        /// Замена пешек, дошедших до стороны противника, на королеву.
        /// </summary>
        public void MakePawnFiguresTransformationIfIsItAvailable()
        {
            ReplaceWhitePawns();
            ReplaceBlackPawns();
        }

        /// <summary>
        /// Заменить все чёрные пешки, которые заменяемы на королеву. Не забывайте заменять после достижения пешкой конца.
        /// </summary>
        private void ReplaceBlackPawns()
        {
            for(int x = 0; x < Board.CellBoardSize; x++)
            {
                if (Positions[x, 7].Side == Side.Black && Positions[x, 7].Man == Figures.Pawn)
                    Positions[x, 7] = new(Side.Black, Figures.Queen);
            }
        }

        /// <summary>
        /// Заменить все белые пешки, которые заменяемы на королеву. Не забывайте заменять после достижения пешкой конца.
        /// </summary>
        private void ReplaceWhitePawns()
        {
            for (int x = 0; x < Board.CellBoardSize; x++)
            {
                if (Positions[x, 0].Side == Side.White && Positions[x, 0].Man == Figures.Pawn)
                    Positions[x, 0] = new(Side.White, Figures.Queen);
            }
        }

        /// <summary>
        /// Получить позицию короля.
        /// </summary>
        /// <param name="side">Сторона короля</param>
        /// <returns>Позиция короля определённой стороны.</returns>
        public CellPoint GetKingPosition(Side side)
        {
            for (int row = 0; row < CellBoardSize; row++)
                for (int column = 0; column < CellBoardSize; column++)
                    if (Positions[column, row].Man == Figures.King && Positions[column, row].Side == side)
                        return new CellPoint() { X = (sbyte)column, Y = (sbyte)row };

            return CellPoint.Unexisted;
        }

        /// <summary>
        /// Получить все позиции съедания для фигуры.
        /// </summary>
        /// <param name="start">Позиция фигуры</param>
        /// <returns>Позиции ходов съедания</returns>
        public List<CellPoint> GetCapturePositions(CellPoint start)
        {
            var figure = Positions[start.X, start.Y];
            Side side = figure.Side;
            Side oppositeSide = GetOppositeSide(side);

            var allPosibleSteps = GetAvailiableStepsFor(start);

            List<CellPoint> result = new List<CellPoint>();
            foreach(var step in allPosibleSteps)
            {
                if (Positions[step.X, step.Y].Side == oppositeSide)
                    result.Add(step);
            }

            return result;
        }

        /// <summary>
        /// Возвращает true, если король под ударом.
        /// </summary>
        /// <param name="side">Сторона, которой принадлежит король.</param>
        /// <returns>true, если указанной стороне поставлен шах.</returns>
        public bool IsCheck(Side side)
        {
            var oppositeSide = GetOppositeSide(side);
            var kingPosition = GetKingPosition(side);

            // Где король од ударом у противоположной стороны?
            Dictionary<CellPoint, List<CellPoint>> allOppositeSteps = GetAvailiableStepsPre(oppositeSide);

            foreach (var step in allOppositeSteps)
            {
                List<CellPoint> currentOppositeSteps = step.Value;

                if (currentOppositeSteps.Where(c => c.X == kingPosition.X && c.Y == kingPosition.Y).Count() > 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Возвращает если указанной стороне поставлен шах и мат.
        /// </summary>
        /// <param name="side">Сторона, которой принадлежит атакуемый король.</param>
        /// <returns>true, если указанной стороне поставлен шах и мат.</returns>
        public bool IsCheckmate(Side side)
        {
            if(IsCheck(side))
            {
                var kingPosition = GetKingPosition(side);

                if (GetAvailiableStepsFor(kingPosition).Count == 0)
                    return true;
            }

            return false;
        }
    }
}
