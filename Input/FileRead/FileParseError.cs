using Common;

namespace Scheduler.Input.FileRead;

public class FileParseError(string description) : Error(nameof(FileParseError), description);