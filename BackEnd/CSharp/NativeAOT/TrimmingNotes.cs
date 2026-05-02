// TrimmingNotes.cs
// Annotations to keep code AOT/trim-safe.

using System.Diagnostics.CodeAnalysis;

namespace BackEnd.CSharp.NativeAOT;

public static class TrimSafe
{
    // Tell the trimmer: the type T might be reflected over for its public
    // properties at runtime, so don't strip them.
    public static T? Bind<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(
        IReadOnlyDictionary<string, string> source) where T : new()
    {
        var inst = new T();
        var props = typeof(T).GetProperties();
        foreach (var p in props)
        {
            if (source.TryGetValue(p.Name, out var raw) && p.CanWrite)
                p.SetValue(inst, Convert.ChangeType(raw, p.PropertyType));
        }
        return inst;
    }
}

public static class TrimUnsafe
{
    // If you cannot make a method trim-safe, mark it. The compiler will warn
    // any caller that's compiled with PublishAot/PublishTrimmed.
    [RequiresUnreferencedCode("Uses reflection over arbitrary types.")]
    [RequiresDynamicCode("Calls MakeGenericType at runtime.")]
    public static object? CreateGeneric(Type open, params Type[] args) =>
        Activator.CreateInstance(open.MakeGenericType(args));
}
