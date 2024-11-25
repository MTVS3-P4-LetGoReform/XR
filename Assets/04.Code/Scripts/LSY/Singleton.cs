using UnityEngine;

public class Singleton : MonoBehaviour
{
    private static Singleton instance;

    private void Awake()
    {
        // 이미 존재하는 인스턴스가 있다면 새로 생성하지 않고 파괴
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 싱글톤 인스턴스 초기화 및 DontDestroyOnLoad 설정
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}