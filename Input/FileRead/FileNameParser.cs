using Common.Results;

namespace Scheduler.Input.FileRead;


public class FileNameParser(string expectedFileType)
{
    public Result<FileInfo> ExamineAndValidateFile(string file)
    {
        if (!expectedFileType.StartsWith('.'))
            expectedFileType = '.' + expectedFileType;
        
        var info = new FileInfo(file);
        if (!info.Name.EndsWith(expectedFileType)) 
            return Result.Failure<FileInfo>(new FileParseError($"The file was not a {expectedFileType}"));

        if (!info.Exists)
            return Result.Failure<FileInfo>(new FileParseError($"The file '{file}' does not exist."));

        return Result.Success(info);
    }
}