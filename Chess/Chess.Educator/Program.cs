using Chess.Educator;
using Chess.Entity;
using System.Linq;

MakeEmptyStepFile();




void MakeEmptyStepFile(string fileName = "steps.dcd")
{
    long existedRecordCount = 0;
    long writtenRecordsCount = 0;

    string directory = string.Format("{0}/data/", AppDomain.CurrentDomain.BaseDirectory);
    string fullFileName = string.Format("{0}{1}", directory, fileName);

    if (!Directory.Exists(directory))
        Directory.CreateDirectory(directory);

    ResearchBoard board = new ResearchBoard();

    MakeFirstIteration(fullFileName, board, ref existedRecordCount, ref writtenRecordsCount);

    (var isBoardBytesExists, _) = IsBoardBytesExist(board, fullFileName);

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

                if (/*!isBoardBytesExists*/true)
                {
                    /*if (File.Exists(fullFileName))
                    {
                        var stepFile = File.OpenWrite(fullFileName);

                        stepFile.Position = stepFile.Length;
                    */
                        var stepsForWrite = stackBoard.GetAvailableSteps(stackBoard.CurrentStepSide);

                        WriteCurrentSteps(stackBoard, stepsForWrite, ref existedRecordCount, ref writtenRecordsCount, fullFileName);

                        foreach (var stepStartQ in stepsForWrite.Keys)
                        {
                            foreach (var stepEndQ in stepsForWrite[stepStartQ])
                            {
                                var nsb = new ResearchBoard(stackBoard);

                                nsb.MakeStepWithoutChecking(stepStartQ, stepEndQ);
                                
                                boardQueue.Enqueue(nsb);
                            }
                        }

                        Console.WriteLine($"NEW Iteration = {i++} | Queue Size = {boardQueue.Count} | Existed = {existedRecordCount} | Written = {writtenRecordsCount}");

                        /*stepFile.Close();
                    }*/
                }
                else
                {
                    Console.WriteLine($"[Existing board analyzed]  | Existed = {existedRecordCount} | Written = {writtenRecordsCount}");
                }

                
            }
        }

    }

    Console.WriteLine($"!!!ENDED!!!");

    static void WriteCurrentSteps(ResearchBoard board, Dictionary<CellPoint, List<CellPoint>> currentSteps, ref long existedRecordCount, ref long writtenRecordsCount, string fullFileName)
    {
        foreach (var (start, newBoard, end) in from start in currentSteps.Keys
                                               let newBoard = new ResearchBoard(board)
                                               from end in currentSteps[start]
                                               select (start, newBoard, end))
        {
            
            newBoard.MakeStepWithoutChecking(start, end);
            WriteBoardWithEmptySteps(newBoard, ref existedRecordCount, ref writtenRecordsCount, fullFileName);
        }
    }

    static void MakeFirstIteration(string fullFileName, ResearchBoard board, ref long existedRecordCount, ref long writtenRecordsCount)
    {
        if (!File.Exists(fullFileName))
        {
            var stepFile = File.Create(fullFileName);

            stepFile.Position = 0;

            stepFile.Close();

            /* Записываем начальные комбинации досок с 2мя дополнительными пустыми байтами для хода белых и чёрных. */
            WriteBoardWithEmptySteps(board, ref existedRecordCount, ref writtenRecordsCount, fullFileName);
            var currentSteps = board.GetAvailableSteps(board.CurrentStepSide);
            WriteCurrentSteps(board, currentSteps, ref existedRecordCount, ref writtenRecordsCount, fullFileName);

            
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

static void WriteBoardWithEmptySteps(ResearchBoard board, ref long existedRecordCount, ref long writtenRecordsCount, string fullFileName)
{
    byte[] bytesToWrite = MakeBoardBytes(board.ToShortByteArray());
    (bool isBoardBytesExists, _) = IsBoardBytesExist(board, fullFileName);
    if (!isBoardBytesExists)
    {
        if (File.Exists(fullFileName))
        {
            var stepFile = File.OpenWrite(fullFileName);

            stepFile.Position = stepFile.Length;

            stepFile.Write(bytesToWrite);
            writtenRecordsCount++;

            stepFile.Close();
        }
    } 
    else { existedRecordCount++;  }
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