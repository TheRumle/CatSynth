using Cat.Verify.Definitions;
using Cat.Verify.Verification.Errors;
using Common;
using Common.Results;
using Common.String;

namespace Cat.Verify.Verification.ExecutionVerification;

internal class ExecutionTimeVerifier(CatContext context, ProtocolAnalyser protocolAnalyser) : IExecutionTimeVerifier
{
    /// <inheritdoc />
    public Result IsTimely(Execution execution)
    {
        Error[] errors = [..VerifyDeadline(execution), ..VerifyMachineTimes(execution), ..VerifyActiveProcessing(execution)];
        return errors.Any() ? Result.Failure(errors.ToHashSet().ToArray()) : Result.Success();
    }

    private IEnumerable<VerificationError> VerifyActiveProcessing(Execution execution)
    {
        for (int i = 0; i < execution.Actions.Length; i++)
        {
            var (tI, op) = execution.Actions[i];
            //If the operation is a arrival or placement and it is for an active machine
            if (op is not Placement or Arrival) 
                continue;
            //If it is not an operation on an active machine, continue
            if (op is not IMachineOperation alphaI || !context.ActiveMachines.Contains(alphaI.Machine))
                continue;
            
            var cI = execution.ReachedConfiguration[i];
            var pt = cI.ContentOf(alphaI.Machine);

            for (int j = i+1; j < execution.Actions.Length; j++)
            {
                var (tJ, alphaJ) = execution.Actions[j];
                //If it is not a pickup of this machine
                if (alphaJ is not Pickup pickup || !pickup.Machine.Equals(alphaI.Machine))
                    continue;
                
                var elementsPickedUp = pickup.ProductIds.Intersect(pt); 
                foreach (var id in elementsPickedUp)
                {
                    var (_, min, max) = protocolAnalyser.Next(id, execution.ReachedConfiguration[i]);
                    var timeInMachine = tJ - tI;
                    if (timeInMachine > max || timeInMachine < min)
                        yield return new VerificationError(tJ,
                            $"{alphaI.ToPaperFormattedString()}  ...  {alphaJ.ToPaperFormattedString()} occured too late/early. Product '{id}' was moved out of machine '{alphaI.Machine}' at wrong time. It was in {alphaI.Machine} for {tJ - tI} time units but must be [{min},{max}]", 6);
                }
            }
        }
    }

    private IEnumerable<VerificationError> VerifyMachineTimes(Execution execution)
    {
        for (int i = 0; i < execution.Actions.Length; i++)
        {
            var (tI, alphaI) = execution.Actions[i];
            if (alphaI is not Start start) continue;
            var cI = execution.ReachedConfiguration[i];
            var content = cI.ContentOf(start.Machine);

            if (!content.Any()) continue;

            var (maxLow, lowMax) = Respect(content, cI);
            //The interval is not possible - smallest max processing time is greater than greatest min processing time
            if (maxLow > lowMax)
                yield return new VerificationError(tI,$"The legal processing interval of {content.CommaSeparate()} is [{maxLow},{lowMax}], which is not a valid interval.", 5);
            
            
            for (int j = i+1; j < execution.Actions.Length; j++)
            {
                var (tJ, alphaJ) = execution.Actions[j];
                if (alphaJ is not Stop stop || stop.Machine != start.Machine) continue;
                if (tJ - tI < maxLow ||  tJ - tI > lowMax)
                {
                    var violatedProducts = content.Where(e => protocolAnalyser.Next(e, cI).maxTime > tJ - tI);
                    yield return new VerificationError(tJ,
                        $"Executing {stop.ToPaperFormattedString()} at time {tJ} breaks processing interval of products '{violatedProducts.CommaSeparate()}'", 5);
                }
                break;
            }
        }
    }

    private (int maxLow, int lowMax) Respect(IEnumerable<string> pt, Configuration cI)
    {
        var enumerable = pt as string[] ?? pt.ToArray();
        var maxMin = enumerable.Select(e => protocolAnalyser.Next(e, cI).minTime).Max();
        var minMax = enumerable.Select(e => protocolAnalyser.Next(e, cI).maxTime).Min();
        return (maxMin, minMax);
    }

    private IEnumerable<VerificationError> VerifyDeadline(Execution execution)
    {
        foreach (var id in context.ProductIds)
        {
            var (critStart, critEnd, deadline) = context.CriticalSection[id];
            for (int i = 0; i < execution.ReachedConfiguration.Length; i++)
            {
                var cI = execution.ReachedConfiguration[i];
                //Product has not entered critical section
                if (cI[id] == Configuration.Bot || cI[id].stepsCompleted < critStart) continue;
                //Product is past critical section
                if (cI[id].stepsCompleted > critStart) break;
                
                for (int j = i + 1; j < execution.ReachedConfiguration.Length; j++)
                {
                    //Product was critical but exited critical section
                    if (execution.ReachedConfiguration[j][id].stepsCompleted > critEnd) break;
                    //Product exceeded deadline while in critical section
                    if (execution.Actions[j].time - execution.Actions[i].time > deadline)
                    {
                        yield return new VerificationError(execution.Actions[j].time,
                            $"Product '{id}' exceeds deadline at time {execution.Actions[j].time}",4);                
                    }
                }
            }
        }
    }
}