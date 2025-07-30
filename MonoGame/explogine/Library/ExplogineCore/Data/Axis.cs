﻿namespace ExplogineCore.Data;

public class Axis
{
    public static readonly Axis X = new("X");
    public static readonly Axis Y = new("Y");
    private readonly string _name;

    private Axis()
    {
        // privatize constructor
        throw new Exception("Should not use default constructor for Axis; use static instances");
    }

    private Axis(string name)
    {
        _name = name;
    }

    public static IEnumerable<Axis> Each
    {
        get
        {
            yield return X;
            yield return Y;
        }
    }

    public override string ToString()
    {
        return _name;
    }

    public Axis Opposite()
    {
        if (this == X)
        {
            return Y;
        }

        if (this == Y)
        {
            return X;
        }

        throw new Exception("Unknown Axis");
    }

    public static Axis FromOrientation(Orientation orientation)
    {
        switch (orientation)
        {
            case Data.Orientation.Horizontal:
                return X;
            case Data.Orientation.Vertical:
                return Y;
            default:
                throw new Exception($"Unknown Orientation {orientation}");
        }
    }

    public Orientation Orientation()
    {
        if (this == X)
        {
            return Data.Orientation.Horizontal;
        }

        if (this == Y)
        {
            return Data.Orientation.Vertical;
        }

        throw new Exception("Unknown Axis");
    }

    public T ReturnIfXElseY<T>(Func<T> doIfX, Func<T> doIfY)
    {
        if (this == X)
        {
            return doIfX();
        }

        if (this == Y)
        {
            return doIfY();
        }

        throw new Exception("Unknown axis");
    }
}
