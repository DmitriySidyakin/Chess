// See https://aka.ms/new-console-template for more information
using Chess.Entity;

PrintChessCodes();
ConvertToBytesAndBack();

void PrintChessCodes()
{
    Console.WriteLine("Chess Figures Codes:");

    byte[] sfValues = Enum.GetValues(typeof(SideFigures)).Cast<byte>().ToArray();

    foreach (int sfValue in sfValues)
    {
        if(sfValue != 0)
        {
            Figure figure = new Figure((Side)((sfValue + 1) & 1), (Figures)((sfValue + 1) >> 1));
            Console.Write(sfValue < 10 ? sfValue.ToString() + "   " : sfValue.ToString() + "  ");
            Console.Write(Enum.GetName<SideFigures>((SideFigures)sfValue) + " ");
            Console.Write(Enum.GetName<Side>((Side)((sfValue + 1) & 1)) + " ");
            Console.Write(Enum.GetName<Figures>((Figures)((sfValue + 1) >> 1)) + " | ");
            Console.WriteLine(Enum.GetName<SideFigures>(figure.SideMan) + " | ");
        }
        else
        {
            Figure figure = new Figure((Side)((sfValue + 1) & 1), (Figures)((sfValue + 1) >> 1));
            Console.Write("0   ");
            Console.Write(Enum.GetName<SideFigures>((SideFigures)(sfValue)) + " ");
            Console.WriteLine(Enum.GetName<SideFigures>(figure.SideMan));
        }
        
    }
}

void ConvertToBytesAndBack()
{
    Console.WriteLine("Test - Convert to a byte array and back:");

    Board board = new Board();

    byte[] bytes = board.ToByteArray();

    Board restoredBoard = new Board(bytes);

    Console.WriteLine("Test passed: " + board.Equals(restoredBoard));
}