using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Time;

public class ContinuousTimeModel : ITimeModel
{
    private readonly List<TimeSlot> slots;


    public ContinuousTimeModel(
        List<TimeSlot> slots)
    {
        if(slots == null ||
           slots.Count == 0)
            throw new ArgumentException(
                "时间槽不能为空");


        this.slots =
            slots;


        for(var i = 0;
            i < slots.Count;
            i++)
        {
            slots[i].Index =
                i;
        }
    }



    public IReadOnlyList<TimeSlot> Slots =>
        slots;



    public int SlotCount =>
        slots.Count;



    public TimeSlot GetSlot(
        int slotIndex)
    {
        Validate(slotIndex);

        return slots[slotIndex];
    }



    public DateTime GetSlotStart(
        int slotIndex)
    {
        return GetSlot(slotIndex)
            .StartTime;
    }



    public DateTime GetSlotEnd(
        int slotIndex)
    {
        return GetSlot(slotIndex)
            .EndTime;
    }



    public int GetSlotIndex(
        DateTime time)
    {
        for(var i = 0;
            i < slots.Count;
            i++)
        {
            if(slots[i].Contains(time))
                return i;
        }


        return -1;
    }



    public bool ContainsSlot(
        int slotIndex)
    {
        return slotIndex >= 0 &&
               slotIndex < slots.Count;
    }



    public bool IsWorkingSlot(
        int slotIndex)
    {
        return ContainsSlot(slotIndex);
    }



    private void Validate(
        int index)
    {
        if(!ContainsSlot(index))
            throw new ArgumentOutOfRangeException(
                nameof(index));
    }
}