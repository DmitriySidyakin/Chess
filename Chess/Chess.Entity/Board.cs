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
        
        public Side CurrentStepSide { get; set; }

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

            CurrentStepSide = Side.White;
        }

        public Board(byte[] boardBytes)
        {
            if (boardBytes.Length != 33)
                throw new ArgumentException("The bytes count does not equal 33!");

            int step = 0;
            for (int row = 1; row <= 8; row++)
                for (int col = 1; col <= 8; col++)
                {
                    if(step == 0)
                    {
                        Positions[col - 1, row - 1] = new Figure((byte)(boardBytes[((row - 1) * 8 + col - 1)/2] >> 4));
                        step++;
                    }
                    else
                    {
                        Positions[col - 1, row - 1] = new Figure((byte)(boardBytes[((row - 1) * 8 + col - 1)/2] & 0x0F));
                        step = 0;
                    }
                }

            CurrentStepSide = (Side)boardBytes[32];
        }

        public byte[] ToByteArray()
        {
            byte[] bytes = new byte[33];

            int position = 0;
            int step = 0;
            for(int i = 0; i < 8; i++)
                for(int j = 0; j < 8; j++)
                {
                    if(step == 0)
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

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            if (obj is not Board board)
                return false;

            bool result = true;

            for(int row = 0; row < 8 && result; row++)
                for(int column = 0; column < 8 && result; column++)
                {
                    result = Positions[column, row].SideMan == board.Positions[column, row].SideMan;
                }

            result = result && (CurrentStepSide == board.CurrentStepSide);

            return result;
        }

        public override int GetHashCode()
        {
            byte[] bytes = new byte[4];

            byte position = 0;

            for (int row = 7; row >= 0; row--)
                for (int column = 0; column < 8; column++)
                {
                    bytes[position] ^= (byte) Positions[column, row].SideMan;
                    position++;
                    if(position >= 4)
                        position = 0;
                }

            int result = 0;
            for(byte byteNum = 3; byteNum >= 0; byteNum--)
            {
                result ^= bytes[byteNum];
                result <<= 8;
            }

            return result;
        }

        public override string ToString()
        {
            string result = "Step For: " + Enum.GetName<Side>(CurrentStepSide) + Environment.NewLine;

            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    result += Positions[column, row].ToString();
                    
                    if(column < 7)
                    {
                        result += ", ";
                    }
                }

                result += Environment.NewLine;
            }

            return result;
        } 
    }
}
