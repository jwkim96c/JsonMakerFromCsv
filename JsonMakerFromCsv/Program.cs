using System.Text.Json;
using System.Text.RegularExpressions;

if (args.Length < 2)
{
    Console.WriteLine("Usage: JsonMakerFromCsv [InputCsvFileName] [OutputJsonFileName]");
    Environment.Exit(1);
}

var inputFilename = args[0];
var outputFilename = args[1];
var walletColumn = args.Length >= 3 ? Convert.ToInt32(args[2]) : 2;
var amountColumn = args.Length >= 4 ? Convert.ToInt32(args[3]) : 3;

var errorAddress = new List<String?>();
Console.WriteLine("###### Program Start.");
try
{
    using var reader = new StreamReader($"{inputFilename}");
    using var writer = new StreamWriter($"{outputFilename}");

    writer.WriteLine("[");

    var isTitleRead = false;
    var isFirstWrite = true;
    var lineNumber = 0;
    while (!reader.EndOfStream)
    {
        var line = reader.ReadLine();
        Console.WriteLine($"READLINE: {line} ({++lineNumber})");
        if (isTitleRead == false)
        {
            isTitleRead = true;
            continue;
        }

        var values = line?.Split(',');
        if (values is null || values.Length < 2)
        {
            Console.WriteLine("READ ERROR !!!!!");
            Environment.Exit(2);
        }

        if (Regex.IsMatch(values[walletColumn], "terra1[a-z0-9]{38}") == false)
            errorAddress.Add(line);

        var arr = new[]
        {
            values[walletColumn],
            values[amountColumn],
        };
        var jsonEncoded = JsonSerializer.Serialize(arr);
        if (isFirstWrite)
            isFirstWrite = false;
        else
            writer.WriteLine(",");
        writer.Write("    " + jsonEncoded);
    }

    writer.WriteLine();
    writer.Write("]");
}
catch (Exception e)
{
    Console.WriteLine(e.GetType() + " " + e.Message);
    if (e is not FileNotFoundException)
        Console.WriteLine(e.StackTrace);
}

if (errorAddress.Any())
{
    Console.WriteLine();
    Console.WriteLine("@@@ CHECK ADDRESS @@@");
    foreach (var item in errorAddress)
    {
        Console.WriteLine(item);
    }

    Console.WriteLine("@@@ CHECK END @@@");
    Console.WriteLine();
}

Console.WriteLine("###### Program End.");