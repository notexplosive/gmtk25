﻿namespace ExplogineCore.Data;

public struct Depth
{
    public const int MaxAsInt = 10569646;
    public static Depth Middle = new(MaxAsInt / 2);
    public static Depth Back = new(MaxAsInt);

    public Depth(int val)
    {
        AsInt = val;
    }

    public static implicit operator int(Depth d)
    {
        return d.AsInt;
    }

    public static implicit operator Depth(int i)
    {
        return new Depth(i);
    }

    public static implicit operator float(Depth d)
    {
        return d.AsFloat;
    }

    public int AsInt { get; }

    public float AsFloat
    {
        get
        {
            var isValid = AsInt <= MaxAsInt && AsInt >= 0;
            if (!isValid)
            {
                throw new Exception("Invalid Depth");
            }

            return (float) AsInt / MaxAsInt;
        }
    }

    public static Depth Front => new(0);

    public static Depth operator +(Depth a, Depth b)
    {
        return new Depth(a.AsInt + b.AsInt);
    }

    public static Depth operator -(Depth a, Depth b)
    {
        return new Depth(a.AsInt - b.AsInt);
    }

    public static Depth operator +(Depth a, int b)
    {
        return new Depth(a.AsInt + b);
    }

    public static Depth operator -(Depth a, int b)
    {
        return new Depth(a.AsInt - b);
    }

    public static bool operator ==(Depth a, Depth b)
    {
        return a.AsInt == b.AsInt;
    }

    public static bool operator !=(Depth a, Depth b)
    {
        return !(a == b);
    }

    public override bool Equals(object? obj)
    {
        if (obj is Depth other)
        {
            return other.AsInt == AsInt;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return AsInt;
    }

    public override string ToString()
    {
        return AsInt.ToString();
    }
}
