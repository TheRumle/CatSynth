using System.Text;
using Cat.Verify.Definitions;

namespace Cat.Verify.Verification.ScheduleVerification;

public sealed record Schedule(IEnumerable<(int time, ICatOperation action)> steps);
