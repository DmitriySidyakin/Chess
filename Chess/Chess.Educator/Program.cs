using Chess.Educator;
using Chess.Entity;
using System.Linq;

//MakeEmptyStepFile();
//EmptyChessStateFileInMemoryGenerator.MakeEmptyStepFileInMemory();
// Число Шенона: https://ru.wikipedia.org/wiki/%D0%A7%D0%B8%D1%81%D0%BB%D0%BE_%D0%A8%D0%B5%D0%BD%D0%BD%D0%BE%D0%BD%D0%B0#:~:text=%D0%A7%D0%B8%D1%81%D0%BB%D0%BE%CC%81%20%D0%A8%D0%B5%CC%81%D0%BD%D0%BD%D0%BE%D0%BD%D0%B0%20%E2%80%94%20%D0%BE%D1%86%D0%B5%D0%BD%D0%BE%D1%87%D0%BD%D0%BE%D0%B5%20%D0%BC%D0%B8%D0%BD%D0%B8%D0%BC%D0%B0%D0%BB%D1%8C%D0%BD%D0%BE%D0%B5%20%D0%BA%D0%BE%D0%BB%D0%B8%D1%87%D0%B5%D1%81%D1%82%D0%B2%D0%BE,%D0%A1%D0%BE%D1%81%D1%82%D0%B0%D0%B2%D0%BB%D1%8F%D0%B5%D1%82%20%D0%BF%D1%80%D0%B8%D0%B1%D0%BB%D0%B8%D0%B7%D0%B8%D1%82%D0%B5%D0%BB%D1%8C%D0%BD%D0%BE%2010120.
while (true)
{
    Console.Write("Введите количество досок для анализа: ");
    var stringCount = Console.ReadLine();
    int count = int.Parse(stringCount is null ? "1000" : stringCount);

    EmptyStateFileGenerator.Generate(directory: "F:", maxBoardCountForAnalysis: count);
}


void MakeEmptyStepFile(string fileName = "steps.dcd")
{
    long existedRecordCount = 0;
    long writtenRecordsCount = 0;
    long containsCount = 0;

    long newWrittenRecords = 0;
    long newNullWrittenRecordsSequence = 0;

    string directory = string.Format("{0}/data/", AppDomain.CurrentDomain.BaseDirectory);
    string fullFileName = string.Format("{0}{1}", directory, fileName);

    if (!Directory.Exists(directory))
        Directory.CreateDirectory(directory);

    ResearchBoard board = new ResearchBoard();

    MakeFirstIteration(fullFileName, board, ref existedRecordCount, ref writtenRecordsCount);

    (var isBoardBytesExists, _) = IsBoardBytesExist(board, fullFileName);

    Queue<ResearchBoard> boardQueue = new Queue<ResearchBoard>();

    boardQueue.Enqueue(new ResearchBoard());

    long i = 0;
    while (boardQueue.Count > 0)
    {
        //var newBoard = boardQueue.Dequeue();
        //var steps = newBoard.GetAvailableSteps(newBoard.CurrentStepSide);

        //foreach (var stepStart in steps.Keys)
        //{
        //    foreach (var stepEnd in steps[stepStart])
        //    {
        //        newWrittenRecords = writtenRecordsCount;
        //        var stackBoard = new ResearchBoard(newBoard);
        //        /*
        //        stackBoard.MakeStepWithoutChecking(stepStart, stepEnd);
        //        Console.WriteLine($"first board side = {Enum.GetName<Side>(stackBoard.CurrentStepSide)}");
        //        (isBoardBytesExists, _) = IsBoardBytesExist(stackBoard, fullFileName);
        //        */
        //        if (/*!isBoardBytesExists*/true)
        //        {
        //            /*if (File.Exists(fullFileName))
        //            {
        //                var stepFile = File.OpenWrite(fullFileName);

        //                stepFile.Position = stepFile.Length;
        //            */
        //            /*
        //            var beforeStepSide = stackBoard.CurrentStepSide;
        //            Console.WriteLine($"before step side = {beforeStepSide}");
        //            Side afterStepSide;*/

        //            var stepsForWrite = stackBoard.GetAvailableSteps(stackBoard.CurrentStepSide);

        //            WriteCurrentSteps(stackBoard, stepsForWrite, ref existedRecordCount, ref writtenRecordsCount, fullFileName);

        //            foreach (var stepStartQ in stepsForWrite.Keys)
        //            {
        //                foreach (var stepEndQ in stepsForWrite[stepStartQ])
        //                {
        //                    var nsb = new ResearchBoard(stackBoard);

        //                    nsb.MakeStepWithoutChecking(stepStartQ, stepEndQ);
        //                    /*
        //                    afterStepSide = nsb.CurrentStepSide;
        //                    Console.WriteLine($"before step side = {afterStepSide}. Is right = {beforeStepSide != afterStepSide}");*/

        //                    if (!boardQueue.Contains(nsb))
        //                        boardQueue.Enqueue(nsb);
        //                    else
        //                        containsCount++;
        //                }
        //            }

        //            newWrittenRecords = writtenRecordsCount - newWrittenRecords;
        //            if (newWrittenRecords == 0)
        //            {
        //                newNullWrittenRecordsSequence++;
        //            }
        //            else
        //            {
        //                newNullWrittenRecordsSequence = 0;
        //            }

        //            Console.WriteLine($"NEW Iteration = {i++} | Queue Size = {boardQueue.Count} | Existed = {existedRecordCount} | Written = {writtenRecordsCount} | Queue collisions = {containsCount} | New written records = {newWrittenRecords} | New null written records sequence = {newNullWrittenRecordsSequence}");
        //            if (newNullWrittenRecordsSequence == 10000000)
        //            {
        //                Console.WriteLine($"New null written records sequence has max count! It's stoping ...| New null written records sequence = {newNullWrittenRecordsSequence}");
        //                return;
        //            }
        //        }
        //        /*stepFile.Close();
        //    }
        //    }
        //    else
        //    {
        //        Console.WriteLine($"[Existing board analyzed]  | Existed = {existedRecordCount} | Written = {writtenRecordsCount}");
        //    }*/


        //    }
        //}

        var newBoard = boardQueue.Dequeue();
        newWrittenRecords = writtenRecordsCount;
        var steps = newBoard.GetAvailableSteps(newBoard.CurrentStepSide);

        var stackBoard = new ResearchBoard(newBoard);

        var stepsForWrite = stackBoard.GetAvailableSteps(stackBoard.CurrentStepSide);

        WriteCurrentSteps(stackBoard, stepsForWrite, ref existedRecordCount, ref writtenRecordsCount, fullFileName);
        
        foreach (var stepStartQ in stepsForWrite.Keys)
        {
            foreach (var stepEndQ in stepsForWrite[stepStartQ])
            {
                var nsb = new ResearchBoard(stackBoard);
                //var beforeStepSide = nsb.CurrentStepSide;
                nsb.MakeStepWithoutChecking(stepStartQ, stepEndQ);
                //Console.WriteLine($"step side is right = {beforeStepSide != nsb.CurrentStepSide}");
                if (!boardQueue.Contains(nsb))
                    boardQueue.Enqueue(nsb);
                else
                    containsCount++;
            }
        }

        newWrittenRecords = writtenRecordsCount - newWrittenRecords;
        if (newWrittenRecords == 0)
        {
            newNullWrittenRecordsSequence++;
        }
        else
        {
            newNullWrittenRecordsSequence = 0;
        }

        Console.WriteLine($"NEW Iteration = {i++} | Queue Size = {boardQueue.Count} | Existed = {existedRecordCount} | Written = {writtenRecordsCount} | Queue collisions = {containsCount} | New written records = {newWrittenRecords} | New null written records sequence = {newNullWrittenRecordsSequence}");
        if (newNullWrittenRecordsSequence == 10000000)
        {
            Console.WriteLine($"New null written records sequence has max count! It's stoping ...| New null written records sequence = {newNullWrittenRecordsSequence}");
            return;
        }
    }

    Console.WriteLine($"!!!ENDED!!! Successeful!!!!!!!!");

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
            /*var currentSteps = board.GetAvailableSteps(board.CurrentStepSide);
            WriteCurrentSteps(board, currentSteps, ref existedRecordCount, ref writtenRecordsCount, fullFileName);*/


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
    else { existedRecordCount++; }
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