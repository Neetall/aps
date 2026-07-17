namespace ProductionScheduling.Algorithm.Optimization.Tabu;

public class TabuMemory
{
    private readonly List<TabuEntry> entries = [];

    private readonly int tenure;



    public TabuMemory(
        int tenure)
    {
        this.tenure =
            tenure;
    }



    public void Add(
        string key,
        int iteration)
    {
        entries.Add(
            new TabuEntry
            {
                Key =
                    key,

                ExpireIteration =
                    iteration + tenure
            });
    }



    public bool IsTabu(
        string key,
        int iteration)
    {
        RemoveExpired(
            iteration);


        return entries.Any(x =>
            x.Key ==
            key);
    }



    public int Count =>
        entries.Count;



    public IReadOnlyList<TabuEntry> Entries =>
        entries;



    private void RemoveExpired(
        int iteration)
    {
        entries.RemoveAll(
            x =>
                x.ExpireIteration <= iteration);
    }
}