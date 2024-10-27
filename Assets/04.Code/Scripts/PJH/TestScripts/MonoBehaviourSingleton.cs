using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new object();

    public static T Instance
    {
        get
        {
            // 이미 인스턴스가 존재하면 반환
            if (_instance != null)
                return _instance;

            // 씬에서 인스턴스 찾기
            _instance = FindAnyObjectByType<T>();

            if (_instance == null)
            {
                // 새로운 게임 오브젝트 생성
                GameObject singletonObject = new GameObject(typeof(T).Name);
                _instance = singletonObject.AddComponent<T>();
            }

            // 씬 전환 시에도 파괴되지 않도록 설정
            DontDestroyOnLoad(_instance.gameObject);

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

// Example usage:
/*public class GameManager : Singleton<GameManager>
{
    // Your GameManager code here
    public int score = 0;
} */

// To use GameManager, simply call:
// GameManager.Instance.score = 10; // Example usage of the singleton instance