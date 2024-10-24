public abstract class Singleton<T> where T : Singleton<T>, new()
{
    private static T _instance;
    private static readonly object _lock = new object();

    public static T Instance
    {
        get
        {
            if (_instance != null)
                return _instance;
            
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new T();
                    _instance.Initialize();
                }
            }
            return _instance;
        }
    }

    // 초기화 메서드
    protected virtual void Initialize()
    {
    }
}