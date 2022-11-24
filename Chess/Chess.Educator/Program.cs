MakeEmptyStepFile();

void MakeEmptyStepFile(string fileName = "steps.dat")
{
    string directory = string.Format("{0}/data/", /*AppDomain.CurrentDomain.BaseDirectory*/ "F:");
    string path = string.Format("{0}{1}", directory, fileName);

    if(!Directory.Exists(directory))
        Directory.CreateDirectory(directory);

    if (!File.Exists(path))
    {
        var stepFile = File.Create(fileName);

        stepFile.Position = 0;
        
        for(byte i = 0; i <= byte.MaxValue; i++)
            stepFile.WriteByte((byte)i);

        stepFile.Close();
    }

}
