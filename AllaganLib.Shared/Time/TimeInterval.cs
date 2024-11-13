using System;
using System.Globalization;

namespace AllaganLib.Shared.Time;
//Credit to Ottermandias

public readonly struct TimeInterval : IEquatable<TimeInterval>, IComparable<TimeInterval>
{
    public TimeStamp Start { get; init; }
    
    public TimeStamp End { get; init; }
    
    public TimeInterval(TimeStamp start, TimeStamp end)
    {
        this.Start = start;
        this.End = end;
    }
    
    public long Duration
        => this == Always ? long.MaxValue : this == Invalid ? 0 : this.End - this.Start;
    
    public long SecondDuration
        => this == Always ? long.MaxValue :
            this == Invalid ? 0 : (this.End - this.Start) / RealTime.MillisecondsPerSecond;
    
    public string DurationString(bool shortString = false)
        => DurationString(this.Start, this.End, shortString);
    
    public TimeInterval Overlap(TimeInterval rhs)
    {
        if (rhs == Invalid || this == Invalid)
            return Invalid;
        
        var newStart = this.Start.Max(rhs.Start);
        var newEnd = this.End.Min(rhs.End);
        return newEnd <= newStart ? Never : new TimeInterval(newStart, newEnd);
    }
    
    public TimeInterval FirstOverlap(RepeatingInterval rhs)
        => rhs.FirstOverlap(this);
    
    public TimeInterval Merge(TimeInterval rhs)
    {
        if (rhs.Start > this.End || this.Start > rhs.End || this == Invalid || rhs == Invalid)
            return Invalid;
        
        var newStart = this.Start.Min(rhs.Start);
        var newEnd = this.End.Max(rhs.End);
        return new TimeInterval(newStart, newEnd);
    }
    
    public TimeInterval Extend(long duration)
    {
        if (duration == 0)
            return this;
        if (this == Always)
            return Always;
        if (this == Invalid)
            return Invalid;
        if (this == Never)
            return Never;
        
        return duration > 0
            ? new TimeInterval(this.Start, this.End + duration)
            : new TimeInterval(this.Start + duration, this.End);
    }
    
    public bool this[TimeStamp timeStamp]
        => this.InRange(timeStamp);
    
    public bool InRange(TimeStamp timeStamp)
        => timeStamp >= this.Start && timeStamp < this.End;
    
    public static readonly TimeInterval Always = new(TimeStamp.MinValue, TimeStamp.MaxValue);
    
    public static readonly TimeInterval Never = new(TimeStamp.Epoch, TimeStamp.Epoch);
    
    public static readonly TimeInterval Invalid = new(TimeStamp.MaxValue, TimeStamp.MinValue);
    
    public bool Equals(TimeInterval other)
        => this.Start == other.Start
           && this.End == other.End;
    
    public override bool Equals(object? obj)
        => obj is TimeInterval other && this.Equals(other);
    
    public override int GetHashCode()
        => HashCode.Combine(this.Start, this.End);
    
    public static bool operator ==(TimeInterval left, TimeInterval right)
        => left.Equals(right);
    
    public static bool operator !=(TimeInterval left, TimeInterval right)
        => !(left == right);
    
    public int CompareTo(TimeInterval rhs)
    {
        if (this == Invalid)
            return Never.CompareTo(rhs);
        
        if (rhs == Invalid)
            rhs = Never;
        
        var diff = this.End - rhs.End;
        if (Math.Abs(diff) > 0)
            return diff.CompareTo(0);
        
        var diff2 = this.Start - rhs.Start;
        return diff2.CompareTo(0);
    }
    
    public static string DurationString(TimeStamp a, TimeStamp b, bool shortString)
    {
        (a, b) = a < b ? (a, b) : (b, a);
        var tmp = new TimeStamp(b - a).RoundToSecond();
        return tmp.Time switch
        {
            > RealTime.MillisecondsPerDay => shortString
                ? $">{tmp.TotalDays}d"
                : $"{((float)tmp.Time / RealTime.MillisecondsPerDay).ToString("F2", CultureInfo.InvariantCulture)} Days",
            > RealTime.MillisecondsPerHour => shortString
                ? $">{tmp.TotalHours}h"
                : $"{tmp.TotalHours:D2}:{tmp.CurrentMinute:D2} Hours",
            _ => shortString
                ? $"{tmp.TotalMinutes}:{tmp.CurrentSecond:D2}m"
                : $"{tmp.TotalMinutes:D2}:{tmp.CurrentSecond:D2} Minutes",
        };
    }
    
    // Returns true if currently active and false otherwise.
    public bool ToTimeString(TimeStamp now, bool shortString, out string timeString)
    {
        if (this == Always)
        {
            timeString = "Always";
            return true;
        }
        
        if (this == Never)
        {
            timeString = "Never";
            return false;
        }
        
        if (this == Invalid)
        {
            timeString = "Unknown";
            return false;
        }
        
        if (this.Start < now)
        {
            if (this.End < now)
            {
                timeString = "Never";
                return false;
            }
            
            timeString = DurationString(this.End, now, shortString);
            return true;
        }
        
        timeString = DurationString(this.Start, now, shortString);
        return false;
    }
}