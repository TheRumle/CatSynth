using Cat.Verify;
using Cat.Verify.Verification;
using CatConversion.SynthesisVerification;
using Common.Results;
using FluentAssertions;
using ModelChecker;
using ModelChecker.Domain;
using ModelChecker.Domain.Actions;
using ModelChecker.Problem;
using ModelChecker.Search;
using Xunit.Abstractions;

namespace Scheduler.ModelChecker.IntegrationTest;

public class ScheduleFindingTest(ITestOutputHelper output) : IntegrationTest<Result<Schedule>>(output), IDisposable
{
    
    private static IEnumerable<ISearchHeuristic> _heuristics = new HeuristicCollection().All();
    public static IEnumerable<object[]> HeuristicCollection()
    {
        foreach (var heuristic in _heuristics)
        {
            yield return [heuristic];
        }
    }
    
    
 
    [Theory]
    [MemberData(nameof(HeuristicCollection))]
    public async Task CanFindOptimalTimeForSimpleSystem(ISearchHeuristic heuristic)
    {
        var productType = "P1";
        var start = Machine.Active(1, "first machine");
        var m1 = new Machine(1, "second machine", [1], MachineState.Off);
        var exit = new Exit("E1");
        var arm = new Arm("Arm", [start, m1], 1, 1, [exit]);
        
        ProtocolList list = new ProtocolList(new Dictionary<string, Protocol>
        {
            { productType, new Protocol([(start, 1, 1), (m1, 1, 1)]) }
        });
        
        InputSequence inputSequence = new([productType], 1, 1, start, list);
        var (checker,conf, problem) = CreateEmptyStartModelChecker(start, m1, arm, exit, CreateNoDeadline(productType), inputSequence, list, heuristic);
        Result = await checker.ExecuteAsync(TimeSpan.FromDays(1));
        Result.IsSuccess.Should().BeTrue();
        await AssertSolution(conf,Result.Value,problem);
        Result.Value.ReachedStates.Any(e => e.ReachedConfiguration.AllLocations.Count() != conf.AllLocations.Count()).Should().Be(false);
        Result.Value.TotalMakespan.Should().Be(5);
    }
    
    [Fact]
    public async Task CanFindSolution_DepthFirst()
    {
        var productType = "P1";
        var start = Machine.Active(1, "first machine");
        var m1 = new Machine(1, "second machine", [1], MachineState.Off);
        var exit = new Exit("E1");
        var arm = new Arm("Arm", [start, m1], 1, 1, [exit]);
        
        ProtocolList list = new ProtocolList(new Dictionary<string, Protocol>
        {
            { productType, new Protocol([(start, 1, 1), (m1, 1, 1)]) }
        });
        
        InputSequence inputSequence = new([productType], 1, 1, start, list);
        var (checker,conf, problem) = CreateDepthFirst(start, m1, arm, exit, CreateNoDeadline(productType), inputSequence, list);
        Result = await checker.ExecuteAsync(TimeSpan.FromDays(1));
        Result.IsSuccess.Should().BeTrue();
        await AssertSolution(conf,Result.Value,problem);
        Result.Value.ReachedStates.Any(e => e.ReachedConfiguration.AllLocations.Count() != conf.AllLocations.Count()).Should().Be(false);
        Result.Value.TotalMakespan.Should().Be(5);
    }
    
    [Theory]
    [MemberData(nameof(HeuristicCollection))]
    public async Task CanFindOptimalTimeForUpscaledSimpleSystem(ISearchHeuristic heuristic)
    {
        var productType = "P1";
        var start = Machine.Active(2, "first machine");
        var m1 = new Machine(1, "second machine", MachineState.Off);
        var exit = new Exit("E1");
        var arm = new Arm("Arm", [start, m1], 1, 1, [exit]);
        
        ProtocolList list = new ProtocolList(new Dictionary<string, Protocol>
        {
            { productType, new Protocol([(start, 1, 5), (m1, 1, 1)]) }
        });
        
        InputSequence inputSequence = new([productType, productType], 1, 1, start, list);
        
        var (checker,conf, problem) = CreateEmptyStartModelChecker(start, m1, arm, exit, CreateNoDeadline(productType), inputSequence, list, heuristic);
        Result = await checker.ExecuteAsync(TimeSpan.FromDays(1));
        Output.WriteLine($"\n\n\nReached states: {checker.NumberOfConfigurationsExplored}");
        Result.IsSuccess.Should().BeTrue();
        Result.Value.ReachedStates.Any(e => e.ReachedConfiguration.AllLocations.Count() != conf.AllLocations.Count()).Should().Be(false);
        await AssertSolution(conf,Result.Value,problem);



        var indicesOfStart = Result.Value.ReachedStates.IndicesWhere(e => e.ReachedBy is MachineStart);
        indicesOfStart.Should().HaveCount(2);
        Result.Value.TotalMakespan.Should().Be(8);
    }
    
    [Theory]
    [MemberData(nameof(HeuristicCollection))]
    public async Task CannotFindSolutionForSimple_WithTwoParts(ISearchHeuristic heuristic)
    {
        var productType = "P1";
        var start = Machine.Active(1, "first machine");
        var m1 = new Machine(1, "second machine", [1], MachineState.Off);
        var exit = new Exit("E1");
        var arm = new Arm("Arm", [start, m1], 1, 1, [exit]);
        
        ProtocolList list = new ProtocolList(new Dictionary<string, Protocol>
        {
            { productType, new Protocol([(start, 1, 1), (m1, 1, 1)]) }
        });
        
        InputSequence inputSequence = new([productType, productType], 1, 1, start, list);


        var (checker,_, _) = CreateEmptyStartModelChecker(start, m1, arm, exit, CreateNoDeadline(productType), inputSequence, list, heuristic);
        Result = await checker.ExecuteAsync(TimeSpan.FromDays(1));
        Result.IsSuccess.Should().BeFalse();
    }

    private (ICatSynthesiser checker, Configuration startConfig, SchedulingProblem problem) CreateEmptyStartModelChecker(Machine start, Machine M1, Arm arm, Exit e, DeadlineCollection deadlineCollection,
        InputSequence inputSequence, ProtocolList list, ISearchHeuristic heuristic)
    {
        var modelcheckerFactory = new SynthesiserFactory();
        var startConfig = EmptyConfiguration([start,M1,arm, e]);
        var problem = new SchedulingProblem("Simple", startConfig, deadlineCollection, inputSequence, list);
        var checker = modelcheckerFactory.CatStar(heuristic, problem);
        return (checker, startConfig, problem);
    }

    
    private (ICatSynthesiser checker, Configuration startConfig, SchedulingProblem problem) CreateDepthFirst(Machine start, Machine M1, Arm arm, Exit e, DeadlineCollection deadlineCollection,
        InputSequence inputSequence, ProtocolList list)
    {
        var modelcheckerFactory = new SynthesiserFactory();
        var startConfig = EmptyConfiguration([start,M1,arm, e]);
        var problem = new SchedulingProblem("Simple", startConfig, deadlineCollection, inputSequence, list);
        var checker = modelcheckerFactory.DepthFirst(problem);
        return (checker, startConfig, problem);
    }

    private static DeadlineCollection CreateNoDeadline(string productType)
    {
        return new DeadlineCollection([(productType, int.MaxValue, int.MaxValue, int.MaxValue)]);
    }

    public Configuration EmptyConfiguration(IEnumerable<Location> locations)
    {
        return new Configuration(locations.ToDictionary(e => e, e => new Product[] { }),0);
    }

    private  async Task AssertSolution(Configuration initialConfiguration, Schedule schedule, SchedulingProblem problem)
    {
        schedule.ReachedStates.First().ReachedConfiguration.Should().Be(initialConfiguration);
        schedule.ReachedStates.First().ReachedConfiguration.Should().Be(initialConfiguration);
        schedule.ToTimeline().First().from.Should().Be(initialConfiguration);
        var (context,execution) = ScheduleCatConverter.Convert(problem, schedule);

        for (int i = 1; i < execution.Actions.Length; i++)
        {
            Output.WriteLine($"({execution.Actions[i].time.ToString()}, {execution.Actions[i].alpha.GetType().Name}) executed under '{execution.ReachedConfiguration[i-1]}'");

        }

        this.Output.WriteLine("\n\n");
        foreach (var c in schedule.ReachedStates.Select(e=>e.ReachedConfiguration))
        {
            this.Output.WriteLine(c.ToString());
        }

        
        
        
        ExecutionVerifierFactory factory = new ExecutionVerifierFactory();
        IExecutionFeasibleVerifier verifier = factory.ConstructVerifier(context);
        var result = await verifier.IsFeasible(execution);
        
        Output.WriteLine("\n");
        Output.WriteLine(execution.ToString());
        Output.WriteLine("\n");
        result.IsSuccess.Should().BeTrue(string.Join('\n', result.Errors.Select(e=>e.ToString())));
        
    }
}