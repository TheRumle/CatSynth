using System.Text;
using Cat.Verify.Definitions;
using Cat.Verify.Verification.TransitionVerification;

namespace Cat.Verify;



public sealed record Execution(IEnumerable<Transition> Steps)
{
    public (int time, ICatOperation alpha)[] Actions = Steps.Select(e => (e.Time, e.Operation)).ToArray();

    public readonly Configuration[] ReachedConfiguration =
        Steps.Select(e => e.CPrime).Union(Steps.Select(e => e.C)).ToHashSet().ToArray();
    
    /// <inheritdoc />
    public override string ToString()
    {
        var bob = new  StringBuilder();
        bob.Append("C_0");
        for (int i = 0; i < Actions.Length; i++)
        {
            bob.Append("--");
            bob.Append($"({Actions[i].time}, {Actions[i].alpha.GetType().Name})");
            bob.Append($"-->C_{i+1}");
        }

        return bob.ToString();
    }
};
