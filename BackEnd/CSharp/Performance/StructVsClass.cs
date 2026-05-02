// StructVsClass.cs
// readonly struct + `in` parameter avoids copies of large structs.

namespace BackEnd.CSharp.Performance;

public readonly struct Vector3(float X, float Y, float Z)
{
    public float X { get; } = X;
    public float Y { get; } = Y;
    public float Z { get; } = Z;

    public float Dot(in Vector3 other) =>
        X * other.X + Y * other.Y + Z * other.Z;

    public static Vector3 Add(in Vector3 a, in Vector3 b) =>
        new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
}

public static class VectorDemo
{
    public static float Run()
    {
        var a = new Vector3(1, 2, 3);
        var b = new Vector3(4, 5, 6);
        return a.Dot(b); // 32
    }
}
