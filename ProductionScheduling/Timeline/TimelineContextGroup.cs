namespace ProductionScheduling.Timeline;

public class TimelineContextGroup
{
    private readonly Dictionary<string, FactoryTimeline> factories = [];


    public IReadOnlyDictionary<string, FactoryTimeline> Factories =>
        factories;


    public void AddFactory(
        FactoryTimeline factory)
    {
        factories[factory.FactoryCode] =
            factory;
    }


    public bool TryGetFactory(
        string factoryCode,
        out FactoryTimeline factory)
    {
        return factories.TryGetValue(
            factoryCode,
            out factory!);
    }


    public FactoryTimeline Get(
        string factoryCode)
    {
        if(!factories.TryGetValue(
               factoryCode,
               out var factory))
        {
            throw new Exception(
                $"工厂时间轴不存在:{factoryCode}");
        }

        return factory;
    }
}