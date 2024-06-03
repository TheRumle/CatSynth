using Common.Results;

namespace Cat.Verify.Verification.ScheduleVerification;

internal interface IScheduleVerifier
{
    public Result VerifySchedule(Schedule schedule);
}