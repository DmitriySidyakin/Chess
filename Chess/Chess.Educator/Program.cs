using Chess.Educator;
using Chess.Entity;
using System.Linq;

MakeEmptyStepFile();

void MakeEmptyStepFile(string fileName = "steps.dcd")
{
    string directory = string.Format("{0}/data/", AppDomain.CurrentDomain.BaseDirectory);
    string fullFileName = string.Format("{0}{1}", directory, fileName);

    if (!Directory.Exists(directory))
        Directory.CreateDirectory(directory);

    ResearchBoard board = new ResearchBoard();

    MakeFirstIteration(fullFileName, board);

    (var isBoardBytesExists, _) = IsBoardBytesExist(board, fullFileName);
    bool hasSteps = true;

    Queue<ResearchBoard> boardQueue = new Queue<ResearchBoard>();

    boardQueue.Enqueue(board);

    long i = 0;
    while (boardQueue.Count > 0)
    { 
        var newBoard = boardQueue.Dequeue();
        var steps = newBoard.GetAvailableSteps(newBoard.CurrentStepSide);

        foreach (var stepStart in steps.Keys)
        {
            foreach (var stepEnd in steps[stepStart])
            {
                var stackBoard = new ResearchBoard(newBoard);
                stackBoard.MakeStepWithoutChecking(stepStart, stepEnd);

                (isBoardBytesExists, _) = IsBoardBytesExist(stackBoard, fullFileName);

                if (!isBoardBytesExists)
                {
                    if (File.Exists(fullFileName))
                    {
                        var stepFile = File.OpenWrite(fullFileName);

                        stepFile.Position = stepFile.Length;

                        var stepsForWrite = stackBoard.GetAvailableSteps(stackBoard.CurrentStepSide);

                        WriteCurrentSteps(stepFile, stackBoard, stepsForWrite);

                        foreach (var stepStartQ in stepsForWrite.Keys)
                        {
                            foreach (var stepEndQ in stepsForWrite[stepStartQ])
                            {
                                var nsb = new ResearchBoard(stackBoard);

                                nsb.MakeStepWithoutChecking(stepStartQ, stepEndQ);
                                
                                boardQueue.Enqueue(nsb);
                            }
                        }

                        

                        Console.WriteLine($"NEW Iteration = {i++} | Queue Size = {boardQueue.Count}");

                        stepFile.Close();
                    }
                }/*
                else
                {
                    var stepsForWrite = stackBoard.GetAvailableSteps(stackBoard.CurrentStepSide);

                    foreach (var stepStartQ in stepsForWrite.Keys)
                    {
                        foreach (var stepEndQ in stepsForWrite[stepStartQ])
                        {
                            var nsb = new ResearchBoard(stackBoard);
                            nsb.MakeStepWithoutChecking(stepStartQ, stepEndQ);
                            if (!boardQueue.Contains(nsb))
                            {
                                boardQueue.Enqueue(nsb);
                                Console.WriteLine("Added Existed boardes");
                            }
                                
                            
                        }
                    }
                }*/

                
            }
        }

        Console.WriteLine($"!!!ENDED!!!");
    }

    static void WriteCurrentSteps(FileStream stepFile, ResearchBoard board, Dictionary<CellPoint, List<CellPoint>> currentSteps, int i = 0)
    {
        foreach (var (start, newBoard, end) in from start in currentSteps.Keys
                                               let newBoard = new ResearchBoard(board)
                                               from end in currentSteps[start]
                                               select (start, newBoard, end))
        {
            newBoard.MakeStepWithoutChecking(start, end);
            WriteBoardWithEmptySteps(stepFile, newBoard);
        }
    }

    static void MakeFirstIteration(string fullFileName, ResearchBoard board)
    {
        if (!File.Exists(fullFileName))
        {
            var stepFile = File.Create(fullFileName);

            stepFile.Position = 0;

            /* Записываем начальные комбинации досок с 2мя дополнительными пустыми байтами для хода белых и чёрных. */
            WriteBoardWithEmptySteps(stepFile, board);

            var currentSteps = board.GetAvailableSteps(board.CurrentStepSide);
            WriteCurrentSteps(stepFile, board, currentSteps);

            stepFile.Close();
        }
    }
}

static (bool, int) IsBoardBytesExist(ResearchBoard board, string fullFileName)
{
    byte[] searchBytes = MakeBoardBytes(board.ToShortByteArray());

    if (File.Exists(fullFileName))
    {
        var stepFile = File.OpenRead(fullFileName);

        var readBytes = new byte[searchBytes.Length];

        int i = 0;
        bool notFound = true;
        while (stepFile.Read(readBytes, 0, searchBytes.Length) == searchBytes.Length && notFound)
        {
            i += searchBytes.Length;

            bool equals = true;
            for (int j = 0; j < searchBytes.Length && equals; j++)
            {
                equals = equals && searchBytes[j] == readBytes[j];
            }

            notFound = notFound && !equals;
        }

        stepFile.Close();

        return (!notFound, i / searchBytes.Length - 1);
    }

    return (false, -1);
}

static void WriteBoardWithEmptySteps(FileStream stepFile, Board board)
{
    byte[] bytesToWrite = MakeBoardBytes(board.ToShortByteArray());
    stepFile.Write(bytesToWrite);
}

static byte[] MakeBoardBytes(byte[] boardBytes)
{
    var bytesToWrite = new byte[boardBytes.Length + 2];

    for (int i = 0; i < boardBytes.Length; i++)
    {
        bytesToWrite[i] = (byte)boardBytes[i];
    }

    bytesToWrite[bytesToWrite.Length - 1] = 0;
    bytesToWrite[bytesToWrite.Length - 1] = 0;
    return bytesToWrite;
}