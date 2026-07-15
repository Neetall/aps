namespace ProductionScheduling.Timeline;

/// <summary>
///     时间计算工具
/// </summary>
public static class TimelineCalculator
{
    /// <summary>
    ///     分钟转换Slot数量
    /// </summary>
    public static int CalculateSlots(
        double minutes,
        int granularityMinutes)
    {
        return (int)Math.Ceiling(
            minutes / granularityMinutes);
    }


    /// <summary>
    ///     根据产量计算生产Slot
    /// </summary>
    public static int CalculateProductionSlots(
        double length,
        double hourlyCapacity,
        int granularityMinutes)
    {
        var minutes =
            length / hourlyCapacity * 60;


        return CalculateSlots(
            minutes,
            granularityMinutes);
    }
}