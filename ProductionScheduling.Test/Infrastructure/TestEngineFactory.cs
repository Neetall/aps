using ProductionScheduling.Algorithm.Calculation;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Application;
using ProductionScheduling.Application.Result;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Test.Infrastructure;

public static class TestEngineFactory
{
    public static SchedulingEngine Create(
        SchedulingContext context)
    {
        /*
         * Resource Index
         */
        var resourceIndex =
            new SchedulingResourceIndex();


        resourceIndex.Build(
            context.Machines);



        /*
         * Scheduler
         */
        var scheduler =
            new GreedyScheduler(
                new ScheduleDurationCalculator(),
                resourceIndex);



        /*
         * Evaluator
         */
        var evaluator =
            new ScheduleEvaluator();



        /*
         * Result Converter
         */
        var resultConverter =
            new SchedulingResultConverter();



        /*
         * Optimization Pipeline
         */
        var pipelineRunner =
            TestAlgorithmFactory
                .CreatePipelineRunner(
                    context);



        return new SchedulingEngine(
            new TimelineInitializer(),
            scheduler,
            evaluator,
            resultConverter,
            pipelineRunner);
    }
}