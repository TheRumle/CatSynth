using Common;
using Common.Results;
using Scheduler.Input.FileRead;

namespace CommandLineApp;

public class ResultDirectoryBuilder
{
    public static Result<(DirectoryInfo outDirectory, FileInfo systemFile, DirectoryInfo seqsDir)> AssertAndConstructDirectory(string directory)
    {
        var loadDir = new DirectoryInfo(directory);
        if (!loadDir.Exists)
        {
            Console.WriteLine($"No directory {loadDir.FullName} found.");
            return Result.Failure<(DirectoryInfo outDirectory, FileInfo systemFile, DirectoryInfo seqsDir)>();
        }
    

        DirectoryInfo? seqsDir = loadDir.EnumerateDirectories().FirstOrDefault(dir => dir.Name.ToLower().Equals(RunDir.SeqsDir));
        if (seqsDir is null || !seqsDir.Exists)
        {
            Console.WriteLine($"No sub-directory { RunDir.SeqsDir} found.");
            return Result.Failure<(DirectoryInfo outDirectory, FileInfo systemFile, DirectoryInfo seqsDir)>(new Error("InputError",$"No sub-directory { RunDir.SeqsDir} found."));
        }

        var systemFile = loadDir.EnumerateFiles().FirstOrDefault(e => e.Name.ToLower().Equals("system.json"));
        if (systemFile == null)
        {
            Console.WriteLine($"No system file 'system.json' found.");
            return Result.Failure<(DirectoryInfo outDirectory, FileInfo systemFile, DirectoryInfo seqsDir)>(
                new Error("InputError",$"No system file 'system.json' found."));
        }
        FileNameParser fileNameParser = new(".json");
        var catSystemFile = fileNameParser.ExamineAndValidateFile(systemFile.FullName);
        var resultDir = loadDir.CreateSubdirectory("results");
        if (catSystemFile.IsFailure)
        {
            Console.WriteLine(ErrorStringFormatter.Format(catSystemFile.Errors));
            return Result.Failure<(DirectoryInfo, FileInfo, DirectoryInfo)>(catSystemFile.Errors);
        }

        var date = DateTime.Now.ToString("yyyyMMddHHmmss");
        var dir = resultDir.CreateSubdirectory(date);
        return Result.Success((outDir: dir, catSystemFile.Value, seqsDir));
    }
    
}