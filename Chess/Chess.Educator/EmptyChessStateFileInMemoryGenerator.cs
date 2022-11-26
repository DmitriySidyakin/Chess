using Chess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Educator
{
    public class EmptyChessStateFileInMemoryGenerator
    {
        static string fullFileName = "steps.dcd";

        public static void MakeEmptyStepFileInMemory(string? directory = null, string fileName = "steps.dcd")
        {
            // Создаём файл
            if(directory == null)
                directory = string.Format("{0}/data/", AppDomain.CurrentDomain.BaseDirectory);

            fullFileName = string.Format("{0}{1}", directory, fileName);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            if (!File.Exists(fullFileName))
            {
                var stepFile = File.Create(fullFileName);

                stepFile.Position = 0;

                stepFile.Close();
            }

            // Создаём первый проанализированный элемент очереди
            var startBoard = new Board();
            WriteBoardWithEmptySteps(new Board());

            Queue<Board> boardQueue = new Queue<Board>();

            List<Board> analyzedBoards = new List<Board>();
            List<ResearchBoard> researchBoards = new List<ResearchBoard>();

            boardQueue.Enqueue(startBoard);

            long varAnalyzedBoardCount = 0;
            while (boardQueue.Count > 0)
            {
                var newBoard = boardQueue.Dequeue();

                if(analyzedBoards.Contains(newBoard)) continue;

                var stackBoard = new Board(newBoard);

                var stepsForWrite = newBoard.GetAvailableSteps(stackBoard.CurrentStepSide);

                if (!researchBoards.Contains(new ResearchBoard(stackBoard))) 
                    WriteCurrentSteps(stackBoard, stepsForWrite);

                foreach (var stepStartQ in stepsForWrite.Keys)
                {
                    foreach (var stepEndQ in stepsForWrite[stepStartQ])
                    {
                        var nsb = new Board(stackBoard);
                        
                        nsb.MakeStepWithoutChecking(stepStartQ, stepEndQ);
                        
                        if (!analyzedBoards.Contains(nsb) && !boardQueue.Contains(nsb))
                            boardQueue.Enqueue(nsb);
                    }
                }

                if (researchBoards.Contains(new ResearchBoard(stackBoard)))
                    researchBoards.Add(new ResearchBoard(stackBoard));

                analyzedBoards.Add(newBoard); varAnalyzedBoardCount++;

                Console.WriteLine($"Analysed board count = {varAnalyzedBoardCount} | Researched board state count = {varAnalyzedBoardCount} | Queue size = {boardQueue.Count}");
            }

            Console.WriteLine($"END! Successeful!");
        }

        static void WriteCurrentSteps(Board board, Dictionary<CellPoint, List<CellPoint>> currentSteps)
        {
            foreach (var (start, newBoard, end) in from start in currentSteps.Keys
                                                   let newBoard = new Board(board)
                                                   from end in currentSteps[start]
                                                   select (start, newBoard, end))
            {
                newBoard.MakeStepWithoutChecking(start, end);
                WriteBoardWithEmptySteps(newBoard);
            }
        }

        static void WriteBoardWithEmptySteps(Board board)
        {
            byte[] bytesToWrite = MakeBoardBytes(board.ToShortByteArray());

            if (File.Exists(fullFileName))
            {
                var stepFile = File.OpenWrite(fullFileName);

                stepFile.Position = stepFile.Length;

                stepFile.Write(bytesToWrite);

                stepFile.Close();
            }
        }

        static byte[] MakeBoardBytes(byte[] boardBytes)
        {
            var bytesToWrite = new byte[boardBytes.Length + 2];

            for (int i = 0; i < boardBytes.Length; i++)
            {
                bytesToWrite[i] = (byte)boardBytes[i];
            }

            bytesToWrite[bytesToWrite.Length - 2] = 0;
            bytesToWrite[bytesToWrite.Length - 1] = 0;
            return bytesToWrite;
        }

        public static void MakeEmptyStepFileWithFileDump(string? directory = null, string fileName = "steps.dcd")
        {
            // In a memory chache
            // Analysed board count = 1000 | Researched board state count = 1000 | Queue size = 10000
        }
    }


}
