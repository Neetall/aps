namespace ProductionScheduling.Algorithm.Configuration;

public class TabuSearchOptions
{
    public int Iterations {get;set;} = 100;

    public int TabuTenure {get;set;} = 10;
    
    public bool AllowWorseMoves {get;set;} = true;
}