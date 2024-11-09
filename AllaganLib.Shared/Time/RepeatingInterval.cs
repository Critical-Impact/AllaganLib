using System;

namespace AllaganLib.Shared.Time;
//Credit to Ottermandias

public readonly struct RepeatingInterval : IEquatable<RepeatingInterval>
{
    public int OnTime { get; init; }
    
    public int OffTime { get; init; }
    
    public int ShiftTime { get; init; }
    
    public long Period
        => this.OnTime + this.OffTime;
    
    public bool AlwaysUp()
        => this.OffTime == 0;
    
    public bool NeverUp()
        => this.OnTime == 0;
    
    public bool IsUp(TimeStamp time)
    {
        if (this == Invalid || this.NeverUp())
            return false;
        if (this.AlwaysUp())
            return true;
        
        var shift = this.SyncToShift(time);
        var period = shift % this.Period;
        return period < this.OnTime;
    }
    
    private TimeStamp SyncToShift(TimeStamp ts)
        => new((ts - this.ShiftTime) / this.Period * this.Period + this.ShiftTime);
    
    public TimeInterval FirstOverlap(TimeInterval interval)
    {
        if (interval == TimeInterval.Invalid)
            return TimeInterval.Invalid;
        
        if (this.OnTime == 0)
            return this.OffTime == 0 ? TimeInterval.Invalid : TimeInterval.Never;
        if (this.OffTime == 0)
            return interval;
        
        if (interval == TimeInterval.Always)
            return new TimeInterval
            {
                Start = TimeStamp.Epoch + this.ShiftTime,
                End = TimeStamp.Epoch + this.ShiftTime + this.OnTime,
            };
        
        var start = this.SyncToShift(interval.Start);
        var end = start + this.OnTime;
        if (end < interval.Start)
        {
            start += this.Period;
            end += this.Period;
        }
        
        var newStart = interval.Start.Max(start);
        var newEnd = interval.End.Min(end);
        return newEnd <= newStart
            ? TimeInterval.Never
            : new TimeInterval(newStart, newEnd);
    }
    
    public TimeInterval NextRealUptime(TimeStamp now)
    {
        if (this.AlwaysUp())
            return TimeInterval.Always;
        if (this.OnTime == 0)
            return TimeInterval.Never;
        
        var syncedNow = this.SyncToShift(now);
        var end = syncedNow + this.OnTime;
        return end > now
            ? new TimeInterval(syncedNow, end)
            : new TimeInterval(syncedNow + this.Period, end + this.Period);
    }
    
    public static readonly RepeatingInterval Always = new()
    {
        OnTime = 1,
        OffTime = 0,
        ShiftTime = 0,
    };
    
    public static readonly RepeatingInterval Never = new()
    {
        OnTime = 0,
        OffTime = 1,
        ShiftTime = 0,
    };
    
    public static readonly RepeatingInterval Invalid = new()
    {
        OnTime = 0,
        OffTime = 0,
        ShiftTime = 0,
    };
    
    public static bool operator ==(RepeatingInterval left, RepeatingInterval right)
        => left.Equals(right);
    
    public static bool operator !=(RepeatingInterval left, RepeatingInterval right)
        => !(left == right);
    
    public bool Equals(RepeatingInterval other)
        => this.OnTime == other.OnTime
           && this.OffTime == other.OffTime
           && this.ShiftTime == other.ShiftTime;
    
    public override bool Equals(object? obj)
        => obj is RepeatingInterval other && this.Equals(other);
    
    public override int GetHashCode()
        => HashCode.Combine(this.OnTime, this.OffTime, this.ShiftTime);
    
    public static RepeatingInterval FromEorzeanMinutes(int startMinute, int endMinute)
    {
        if (startMinute == endMinute)
            return Never;
        
        startMinute %= RealTime.MinutesPerDay;
        endMinute %= RealTime.MinutesPerDay;
        if (startMinute == endMinute)
            return Always;
        
        var duration = TimeStamp.Epoch.AddEorzeaMinutes(
            endMinute < startMinute
                ? endMinute + RealTime.MinutesPerDay - startMinute
                : endMinute - startMinute);
        var offset = TimeStamp.Epoch.AddEorzeaMinutes(startMinute);
        return new RepeatingInterval()
        {
            ShiftTime = (int)(startMinute < endMinute ? offset : offset.AddEorzeaDays(1)),
            OnTime = (int)duration,
            OffTime = (int)(TimeStamp.Epoch.AddEorzeaDays(1) - duration),
        };
    }
    
    // Print eorzean hours in human readable format.
    public string PrintHours(bool simple = false)
    {
        var start = new TimeStamp(this.ShiftTime).CurrentEorzeaMinuteOfDay();
        if (start < 0)
            start += RealTime.MinutesPerDay;
        
        var end = (int)(start + new TimeStamp(this.OnTime).TotalEorzeaMinutes());
        if (end > RealTime.MinutesPerDay)
            end -= RealTime.MinutesPerDay;
        
        var hStart = start / RealTime.MinutesPerHour;
        var hEnd = end / RealTime.MinutesPerHour;
        var mStart = start - hStart * RealTime.MinutesPerHour;
        var mEnd = end - hEnd * RealTime.MinutesPerHour;
        var sStart = $"{hStart:D2}:{mStart:D2}";
        var sEnd = $"{hEnd:D2}:{mEnd:D2}";
        
        return simple ? $"{sStart}-{sEnd}" : $"{sStart} - {sEnd} ET";
    }
    
    public bool Contains(RepeatingInterval other)
        => this.ShiftTime <= other.ShiftTime && this.OnTime >= other.OnTime;
}