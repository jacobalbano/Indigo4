using System.ComponentModel;
using System.Runtime.CompilerServices;

public static class EventExtensions
{
    public static void Set<T>(this PropertyChangedEventHandler handler, object sender, T value, ref T storage, [CallerMemberName] string property = null)
    {
        var changed =
            (value == null && storage != null) ||
            (value != null && storage == null) ||
            !value.Equals(storage);
        
        if (changed)
        {
            storage = value;
            handler?.Invoke(sender, new PropertyChangedEventArgs(property));
        }
    }
}
