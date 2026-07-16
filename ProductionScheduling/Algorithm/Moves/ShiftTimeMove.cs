using ProductionScheduling.Algorithm.Optimization;

namespace ProductionScheduling.Algorithm.Moves;

/// <summary>
/// 同设备时间移动
///
/// 不改变设备
/// 只调整开始时间
/// </summary>
public class ShiftTimeMove : IMove
{
    public string Name =>
        "ShiftTime";



    public bool Apply(
        MoveContext context)
    {
        var operation =
            context.CurrentOperation;


        if(operation == null)
        {
            context.ExecutionRecord =
                new MoveExecutionRecord
                {
                    MoveName = Name,
                    Success = false
                };

            return false;
        }



        if(!context.Timeline.Machines
            .TryGetValue(
                operation.MachineCode,
                out var timeline))
        {
            context.ExecutionRecord =
                new MoveExecutionRecord
                {
                    MoveName = Name,
                    Success = false
                };

            return false;
        }



        var oldStart =
            operation.StartSlot;


        var duration =
            operation.DurationSlots;



        /*
         * 释放旧位置
         */
        timeline.Release(
            oldStart,
            duration);



        /*
         * 查找新的时间
         *
         * 当前策略:
         * 向后移动
         */
        var newStart =
            timeline.FindEarliest(
                duration,
                oldStart + 1);



        if(newStart < 0)
        {
            /*
             * 恢复
             */
            timeline.Occupy(
                oldStart,
                duration);



            context.ExecutionRecord =
                new MoveExecutionRecord
                {
                    MoveName = Name,
                    Success = false
                };


            return false;
        }



        /*
         * 占用新位置
         */
        timeline.Occupy(
            newStart,
            duration);



        operation.StartSlot =
            newStart;



        context.ExecutionRecord =
            new MoveExecutionRecord
            {
                MoveName =
                    Name,

                Success =
                    true,

                JobTicketCode =
                    operation.JobTicketCode,


                OldMachineCode =
                    operation.MachineCode,


                OldStartSlot =
                    oldStart,


                NewStartSlot =
                    newStart,


                OldDurationSlots =
                    duration,


                NewDurationSlots =
                    duration
            };



        return true;
    }



    /// <summary>
    /// 撤销移动
    /// </summary>
    public void Undo(
        MoveContext context)
    {
        var record =
            context.ExecutionRecord;


        var operation =
            context.CurrentOperation;



        if(record == null ||
           !record.Success ||
           operation == null)
        {
            return;
        }



        if(!context.Timeline.Machines
            .TryGetValue(
                record.OldMachineCode!,
                out var timeline))
        {
            return;
        }



        /*
         * 删除新位置
         */
        timeline.Release(
            record.NewStartSlot,
            record.NewDurationSlots);



        /*
         * 恢复旧位置
         */
        timeline.Occupy(
            record.OldStartSlot,
            record.OldDurationSlots);



        operation.StartSlot =
            record.OldStartSlot;



        operation.DurationSlots =
            record.OldDurationSlots;



        context.ExecutionRecord =
            null;
    }
}