
public class InstancedClass<T> where T : new()
{
    public static T Instance
    {
        get
        {
            if (_instance == null)
                _instance = new T();

            return _instance;
        }

    }

    protected static T _instance;
}
