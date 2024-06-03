using System.Text;
using Cat.Verify;
using Common;
using Common.Results;
using Experiments.ExperimentDriver;
using ModelChecker;

namespace Experiments.ResultWriter;

public sealed class ExperimentResultWriter : IDisposable, IAsyncDisposable
{
    /// <inheritdoc />
    public void Dispose()
    {
        _csvFileWriter.Dispose();
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await _csvFileWriter.DisposeAsync();
    }

    private readonly FileInfo _resultCsvFile;
    private const char Separator = ';';
    private bool _headerWritten = false;


    private readonly StreamWriter _csvFileWriter;


    public ExperimentResultWriter(DirectoryInfo resultDirInfo, string csvFileName)
    {
        _resultCsvFile = new FileInfo(resultDirInfo.FullName + Path.DirectorySeparatorChar + csvFileName);
        _csvFileWriter = new StreamWriter(_resultCsvFile.FullName);
    }
    
    public async Task<Result<FileInfo>> WriteLine(ExperimentResultContext context, string name)
    {
        var errorFiles = new List<(Error[] errors, FileInfo file)>();
        if (context.VerificationErrors.Length > 0)
            errorFiles.Add((context.VerificationErrors, new FileInfo(_resultCsvFile.FullName + '_'+ name +  "_verification_errors")));

        var traceFiles = new List<(Schedule schedule, Execution execution, FileInfo file)>();
        if (context.Schedule is not null)
            traceFiles.Add((context.Schedule,context.Execution!, new FileInfo(_resultCsvFile.FullName + '_' + name + "_traces")));
        
        try
        { 
            await WriteErrors(errorFiles);
            await WriteTraces(traceFiles);
            await WriteResult(context);
            
            return Result.Success(_resultCsvFile);
        }
        catch (Exception e)
        {
            return Result.Failure<FileInfo>(new Error("CsvWriteError", e.Message));
        }
    }

    private static async Task WriteErrors(List<(Error[] errors, FileInfo file)> _errorFiles)
    {
        foreach (var (errs, file) in _errorFiles)
        {
            await File.WriteAllTextAsync(file.FullName, ErrorStringFormatter.Format(errs).ToString());
        }
    }
    
    private static async Task WriteTraces(List<(Schedule schedule, Execution execution, FileInfo file)> traceFiles)
    {
        var bob = new StringBuilder();
        foreach (var (trace, execution, file) in traceFiles)
        {
            bob.AppendLine("Execution trace:");
            bob.AppendLine(execution.ToString());
            bob.AppendLine("Schedule:");
            bob.AppendLine(trace.ToTimelineString());
            bob.AppendLine();
            bob.AppendLine("Schedule summary");
            bob.AppendLine(trace.ToString());
            await File.WriteAllTextAsync(file.FullName, bob.ToString());
        }
    }

    public async Task WriteHeader()
    {
        var header = ExperimentResult.ToHeader(Separator);
        if (header.EndsWith(';'))
        {
            await _csvFileWriter.WriteLineAsync(ExperimentResult.ToHeader(Separator));
        }
        else
        {
            await _csvFileWriter.WriteLineAsync(ExperimentResult.ToHeader(Separator) +";");
        }
        _headerWritten = true;
    }

    private async Task WriteResult(ExperimentResultContext context)
    {
        if (!_headerWritten) await WriteHeader();
        await _csvFileWriter.WriteLineAsync(context.Result.ToCsvRow(Separator));
    }

    public async Task Commit()
    {
        await _csvFileWriter.FlushAsync();
        _csvFileWriter.Close();
    }
}