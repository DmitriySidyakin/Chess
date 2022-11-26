using Chess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Educator
{
    public class EmptyStateFileGenerator
    {

        static public void Generate(string? directory = null, string fileName = "steps.dcd", long maxBoardCountForAnalysis = 1000)
        {
            if (maxBoardCountForAnalysis <= 0)
                return;

            long analisedBoardCount = 0;

            // Создаём файл
            string autoDirectory = GetDirectory(directory);

            if (!Directory.Exists(autoDirectory))
                Directory.CreateDirectory(autoDirectory);

            string fileFullName = $"{autoDirectory}/data/{fileName}";
            
            CreateFileFileIfIsNotItExists(fileFullName);

            MakeFirstRecordIfIsEmptyFile(fileFullName);

            Board? currentBoard = GetFirstUnanalyzedBoard(fileFullName);

            var foregroundColor = Console.ForegroundColor;

            while (currentBoard != null && analisedBoardCount < maxBoardCountForAnalysis)
            {
                
                Console.Write("Текущий шаг = "); Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(analisedBoardCount + 1); Console.ForegroundColor = foregroundColor;

                Board newBoard = currentBoard;

                List<Board> newBoards = GetNewSteps(newBoard);

                List<Board> unexistedBoards = FilterExistingNewBoards(newBoards, fileFullName);

                WriteBoardsWithEmptySteps(unexistedBoards, fileFullName);

                MarkBoardAnalised(newBoard, fileFullName);

                // Get next board
                analisedBoardCount++;
                currentBoard = GetFirstUnanalyzedBoard(fileFullName);
            }

            if (currentBoard == null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("All boards have analysed!"); Console.ForegroundColor = foregroundColor;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Finished! Reached max count ({maxBoardCountForAnalysis})"); Console.ForegroundColor = foregroundColor;
            }
        }

        private static void MarkBoardAnalised(Board board, string fileFullName)
        {
            if (File.Exists(fileFullName))
            {
                int foundPosition = -1;
                using (var stepFile = File.OpenRead(fileFullName))
                {
                    stepFile.Position = 0;

                    byte[] readBytes = new byte[34];
                    bool found = false;
                    
                    int currentPosition = 0;
                    while (stepFile.Read(readBytes) == 34 && !found)
                    {

                        byte[] readBytesBoard = new byte[33];
                        for (int i = 0; i < 33; i++)
                            readBytesBoard[i] = readBytes[i];

                        if (board.Equals(new Board(readBytesBoard)))
                        {
                            found = true;
                            foundPosition = currentPosition;
                        }
                        currentPosition += 34;
                    }
                    stepFile.Close();
                }


                using (var stepFile = File.OpenWrite(fileFullName))
                {
                    stepFile.Position = foundPosition;

                    byte[] bytesToWrite = MakeBoardBytes(board.ToByteArray());
                    bytesToWrite[33] = 1;
                    stepFile.Write(bytesToWrite);

                    stepFile.Close();
                }
                    
            }
        }

        private static void WriteBoardsWithEmptySteps(List<Board> unexistedBoards, string fileFullName)
        {
            if (File.Exists(fileFullName) && unexistedBoards.Count > 0)
            {
                using (var stepFile = File.OpenWrite(fileFullName))
                {
                    stepFile.Position = stepFile.Length;

                    foreach (Board board in unexistedBoards)
                    {
                        byte[] bytesToWrite = MakeBoardBytes(board.ToByteArray());
                        stepFile.Write(bytesToWrite);
                    }

                    stepFile.Close();
                }
            }
        }

        private static List<Board> FilterExistingNewBoards(List<Board> newBoards, string fileFullName)
        {
            List<Board> result = new List<Board>();

            if (File.Exists(fileFullName))
            {
                using (var stepFile = File.OpenRead(fileFullName))
                {
                    byte[] readBytes = new byte[34];
                    foreach (Board board in newBoards)
                    {
                        stepFile.Position = 0;

                        bool currentBoardFound = false;
                        while (stepFile.Read(readBytes) == 34 && currentBoardFound)
                        {
                            byte[] readBytesBoard = new byte[33];
                            for (int i = 0; i < 33; i++)
                                readBytesBoard[i] = readBytes[i];

                            if (!board.Equals(new Board(readBytesBoard)))
                            {
                                result.Add(board);
                                currentBoardFound = true;
                            }
                        }

                        if (!currentBoardFound) result.Add(board);
                    }

                    stepFile.Close();
                }
            }

            return result;
        }

        private static Board? GetFirstUnanalyzedBoard(string fileFullName)
        {
            Board? board = null;
            if (File.Exists(fileFullName))
            {
                using (var stepFile = File.OpenRead(fileFullName))
                {
                    stepFile.Position = 0;

                    byte[] readBytes = new byte[34];

                    while (stepFile.Read(readBytes) == 34)
                    {
                        if (readBytes[33] == 0)
                        {
                            stepFile.Close();
                            byte[] readBytesBoard = new byte[33];
                            for (int i = 0; i < 33; i++)
                                readBytesBoard[i] = readBytes[i];
                            board = new Board(readBytesBoard);
                            break;
                        }
                    }

                    stepFile.Close();
                }
            }

            return board;
        }

        private static List<Board> GetNewSteps(Board newBoard)
        {
            List<Board> newBoards = new List<Board>();

            var stepsForWrite = newBoard.GetAvailableSteps(newBoard.CurrentStepSide);

            foreach (var stepStartQ in stepsForWrite.Keys)
            {
                foreach (var stepEndQ in stepsForWrite[stepStartQ])
                {
                    var nsb = new Board(newBoard);

                    nsb.MakeStepWithoutChecking(stepStartQ, stepEndQ);

                    newBoards.Add(nsb);
                }
            }

            return newBoards;
        }

        private static void MakeFirstRecordIfIsEmptyFile(string fileFullName)
        {
            if (File.Exists(fileFullName))
            {
                long lenght = -1;

                using (var stepFile = File.OpenRead(fileFullName))
                {
                    lenght = stepFile.Length;
                    stepFile.Close();
                }

                if (lenght == 0)
                {
                    WriteBoardWithEmptySteps(new Board(), fileFullName);
                }
            }
        }

        private static void CreateFileFileIfIsNotItExists(string fullFileName)
        {
            if (!File.Exists(fullFileName))
            {
                using (var stepFile = File.Create(fullFileName))
                    stepFile.Close();
            }
        }

        private static string GetDirectory(string? directory)
        {
            string autoDirectory;
            if (directory == null)
                autoDirectory = string.Format("{0}/data/", AppDomain.CurrentDomain.BaseDirectory);
            else autoDirectory = directory;
            return autoDirectory;
        }

        static void WriteBoardWithEmptySteps(Board board, string fileFullName)
        {
            byte[] bytesToWrite = MakeBoardBytes(board.ToByteArray());

            if (File.Exists(fileFullName))
            {
                using (var stepFile = File.OpenWrite(fileFullName))
                {
                    stepFile.Position = stepFile.Length;

                    stepFile.Write(bytesToWrite);

                    stepFile.Close();
                }
            }
        }

        static byte[] MakeBoardBytes(byte[] boardBytes)
        {
            var bytesToWrite = new byte[boardBytes.Length + 1];

            for (int i = 0; i < boardBytes.Length; i++)
            {
                bytesToWrite[i] = (byte)boardBytes[i];
            }

            bytesToWrite[bytesToWrite.Length - 1] = 0;
            return bytesToWrite;
        }

    }
}
