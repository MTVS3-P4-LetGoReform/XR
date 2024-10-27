using System.Collections.Generic;
using TMPro;
using UnityEngine;

using static FirebaseDatabaseAPI;

public class DBTest : MonoBehaviour
{
    public TMP_Text userName;
    public TMP_Text friendCount;
    public TMP_Text estateName;

    /*private void Start()
    {
        User newUser = new User
        {
            userId = "user123",
            userName = "JaneDoe",
        };
        CreateData("users",newUser);
    }*/

    public void Create()
    {
        InitializeFirebase(onInitialized: () =>
            {
                Debug.Log("Firebase 초기화 완료!");

                // 사용자 데이터 생성
                User newUser = new User
                {
                    userId = "user123",
                    userName = "JaneDoe",
                    friends = new List<Friend>
                    {
                        new ("friend456", "JohnSmith"),
                        new ("friend789", "AliceJones")
                    },
                    estateInfo = new EstateInfo
                    {
                        EstateName = "Jane's Estate",
                        objects = new List<EstateObject>
                        {
                            new EstateObject
                            {
                                objectId = "obj001",
                                objectType = "Tree",
                                position = new Vector3(1.0f, 0.0f, 3.5f),
                                rotation = new Vector3(0.0f, 45.0f, 0.0f),
                                scale = new Vector3(1.0f, 1.0f, 1.0f)
                            }
                        }
                    }
                };

                // 데이터 저장 경로 
                string path = $"users/{newUser.userId}";

                // 데이터 생성 
                CreateData(path, newUser,
                    onSuccess: () => Debug.Log("사용자 데이터 생성 완료"),
                    onFailure: (exception) => Debug.LogError("데이터 생성 실패: " + exception.Message)
                );
            },
            onFailure: (exception) =>
            {
                Debug.LogError("Firebase 초기화 실패: " + exception.Message);
            });
    }

    public void Read()
    {
        InitializeFirebase(onInitialized: () =>
            {
                Debug.Log("Firebase 초기화 완료!");
                
                var userId = "user123";
                var path = $"users/{userId}";
                
                ReadData<User>(path,
                    onSuccess: (user) =>
                    {
                        if (user != null)
                        {
                            Debug.Log($"사용자 이름: {user.userName}");
                            Debug.Log($"친구 수: {user.friends.Count}");
                            Debug.Log($"에스테이트 이름: {user.estateInfo.EstateName}");
                            
                            userName.text = user.userName;
                            friendCount.text = $"{user.friends.Count}";
                            estateName.text = user.estateInfo.EstateName;
                        }
                        else
                        {
                            Debug.Log("사용자 데이터를 찾을 수 없습니다.");
                        }
                    },
                    onFailure: (exception) => Debug.LogError("데이터 읽기 실패: " + exception.Message)
                );
            },
            onFailure: (exception) =>
            {
                Debug.LogError("Firebase 초기화 실패: " + exception.Message);
            });
    }

    public void UpdateTest()
    {
        InitializeFirebase(onInitialized: () =>
            {
                Debug.Log("Firebase 초기화");

                string userId = "user123";
                string basePath = $"users/{userId}";

                
                var updates = new Dictionary<string, object>();
                updates[$"{basePath}/userName"] = "JaneDoeUpdated";
                
                UpdateData(updates,
                    onSuccess: () => Debug.Log("사용자 데이터 업데이트 완료"),
                    onFailure: (exception) => Debug.LogError("데이터 업데이트 실패: " + exception.Message)
                );
            },
            onFailure: (exception) =>
            {
                Debug.LogError("Firebase 초기화 실패: " + exception.Message);
            });
    }

    public void Delete()
    {
        InitializeFirebase(onInitialized: () =>
            {
                Debug.Log("Firebase 초기화 완료!");

                string userId = "user123";
                string path = $"users/{userId}";

                // 데이터 삭제 호출
                DeleteData(path,
                    onSuccess: () => Debug.Log("사용자 데이터 삭제 완료"),
                    onFailure: (exception) => Debug.LogError("데이터 삭제 실패: " + exception.Message)
                );
            },
            onFailure: (exception) =>
            {
                Debug.LogError("Firebase 초기화 실패: " + exception.Message);
            });
    }

    private string userId = "user123";
    private string path;
    
    public void ListenForDataChanges()
    {
        path = $"users/{userId}";
        
        InitializeFirebase(onInitialized: () =>
            {
                Debug.Log("Firebase 초기화 완료!");

                // 데이터 변경 수신 시작
                FirebaseDatabaseAPI.ListenForDataChanges<User>(path,
                    onDataChanged: (user) =>
                    {
                        if (user != null)
                        {
                            Debug.Log("데이터 변경 감지:");
                            Debug.Log($"사용자 이름: {user.userName}");
                            Debug.Log($"친구 수: {user.friends?.Count ?? 0}");
                            Debug.Log($"에스테이트 이름: {user.estateInfo?.EstateName}");

                            userName.text = user.userName;
                            friendCount.text = $"{user.friends?.Count}";
                            estateName.text = user.estateInfo?.EstateName;
                            
                        }
                        else
                        {
                            Debug.Log("사용자 데이터가 없습니다.");
                        }
                    },
                    onError: (exception) => Debug.LogError("데이터 수신 오류: " + exception.Message)
                );
            },
            onFailure: (exception) =>
            {
                Debug.LogError("Firebase 초기화 실패: " + exception.Message);
            });
    }
}
