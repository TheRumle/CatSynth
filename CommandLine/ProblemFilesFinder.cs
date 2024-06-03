using Common;
using Common.Results;

namespace CommandLineApp;

public static class ProblemFilesFinder
{
    
    private static readonly string InstanceDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Problems", "Instances");
    private static readonly string SystemDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Problems", "Systems");
    
    private static readonly DirectoryInfo SystemDirectory = new(SystemDirPath);
    private static readonly DirectoryInfo InstanceDirectory = new(InstanceDirPath);
    
    public static FileInfo FindSystemFile(string systemName)
    {
        var system = SystemDirectory.EnumerateFiles().FirstOrDefault(file => StripFileType(file).Equals(systemName));
        if (system is null)
            throw new Exception($"No system named {systemName}");
        return system;
    }
    
    public static FileInfo FindInstanceFile(string instanceFile)
    {
        var system = InstanceDirectory.EnumerateFiles().FirstOrDefault(file => StripFileType(file).Equals(instanceFile));
        if (system is null)
            throw new Exception($"No problem instance named {instanceFile}");
        return system;
    }

    public static Result<(FileInfo systemFile, FileInfo instance)> FindProblemTuple(string systemName, string instanceName)
    {
        if (!SystemDirectory.Exists)
            throw new Exception("No directory for systems! It should be " + SystemDirPath);
        
        if (!InstanceDirectory.Exists)
            throw new Exception("No directory for systems! It should be " + InstanceDirPath);
        
        var system = SystemDirectory.EnumerateFiles().FirstOrDefault(file => StripFileType(file).Equals(systemName));
        var instance = InstanceDirectory.EnumerateFiles().FirstOrDefault(file => StripFileType(file).Equals(instanceName));
        List<Error> errs = [];
        if (system is null)
            errs.Add(new Error("InputError",$"No system {systemName}"));
        
        if (instance is null)
            errs.Add(new Error("InputError",$"No problem instance {instanceName}"));
        
        if (errs.Count != 0)
        {
            return Result.Failure<(FileInfo systemFile, FileInfo instance)>(errs);
        }

        return Result.Success((system, instance))!;
    }
    
    private static string StripFileType(FileInfo fileInfo)
    {
        return Path.GetFileNameWithoutExtension(fileInfo.FullName);
    }
}