namespace Game.Core.Interfaces
{
    public interface IStatisticsManager
    {
        int Score { get; set; }
        
        int LinesCount { get; set; }
        
        int Level { get; set; }
    }
}