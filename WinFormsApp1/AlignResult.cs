namespace WinFormsApp1;

public struct AlignResult
{
    public RavidPoint AlignCenterMm;
    
    public double AngleDegree;

    // Constructor initializes fields
    public AlignResult()
    {
        AlignCenterMm = new RavidPoint(0, 0);
        AngleDegree = 0.0;
    }

    // Copy constructor / assignment-like behavior
    public AlignResult(AlignResult src)
    {
        AlignCenterMm = src.AlignCenterMm;
        AngleDegree = src.AngleDegree;
    }

    // Clear method equivalent
    public void Clear()
    {
        AlignCenterMm.SetPoint(0, 0);
        AngleDegree = 0.0;
    }
}

// Supporting struct for CRavidPoint<double>
public struct RavidPoint
{
    public double X;
    public double Y;

    public RavidPoint(double x, double y)
    {
        X = x;
        Y = y;
    }

    public void SetPoint(double x, double y)
    {
        X = x;
        Y = y;
    }

    public override string ToString() => $"({X:F3}, {Y:F3})";
}
