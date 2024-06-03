using Cocona;

namespace CommandLineApp;

public sealed record RunDir(
    [Argument(Description =
        "The directory containing a .json file describing system and a subdirectory /seqs/ containing sequences the topology of the system.")]
    string Dir,
    [Argument(Description = "Which heuristics to use. Use --list to list possible heuristics.")]
    string[] Heuristics,
    [Argument(Description = $"The timeout in minutes. Default is 60 minutes.")]
    int? TimeOut
)
{
    public const string SeqsDir = "seqs";
}

  

public sealed record Run(
    [Option(Description = "The .json file describing the topology of the system.")]
    string SystemFile,
    [Option(Description = "The .json file describing the input sequence to use in experiments.")]
    string SequenceFile,
    [Option(Description = "Which heuristics to use. Use --list to list possible heuristics.")]
    string[] Heuristics,
    [Option(Description = "The destination directory of the result files.")]
    string ResultDir,
    [Option(Description = $"The timeout in minutes. Default is 60 minutes.")]
    int? TimeOut
) : ICommandParameterSet
{
    private const int TimeoutMinutes = 120;
    public static TimeSpan DefaultTimeout = TimeSpan.FromMinutes(TimeoutMinutes);
}
