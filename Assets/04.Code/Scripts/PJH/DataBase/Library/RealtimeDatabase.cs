using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public static partial class RealtimeDatabase
{
    private static DatabaseReference databaseReference;
    private static bool isInitialized = false;
    private static Dictionary<string, EventHandler<ValueChangedEventArgs>> listeners = new Dictionary<string, EventHandler<ValueChangedEventArgs>>();

    /// <summary>
    /// Firebase를 초기화합니다.
    /// </summary>
    /// <param name="onInitialized">초기화가 완료되면 호출되는 콜백입니다.</param>
    /// <param name="onFailure">초기화에 실패하면 호출되는 콜백입니다.</param>
    /// <example>
    /// <code>
    /// FirebaseDatabaseAPI.InitializeFirebase(onInitialized: () =>
    /// {
    ///     Debug.Log("Firebase 초기화 완료!");
    ///     // 초기화 완료 후 수행할 작업
    /// },
    /// onFailure: (exception) =>
    /// {
    ///     Debug.LogError("Firebase 초기화 실패: " + exception.Message);
    /// });
    /// </code>
    /// </example>
    public static void InitializeFirebase(Action onInitialized = null, Action<Exception> onFailure = null)
    {
        if (isInitialized)
        {
            onInitialized?.Invoke();
            return;
        }

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Firebase초기화 실패 : "+task.Exception);
                onFailure?.Invoke(task.Exception);
                return;
            }
            if (task.Result == DependencyStatus.Available)
            {
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                isInitialized = true;
                Debug.Log("Firebase Initialized");
                onInitialized?.Invoke();
            }
            else
            {
                Debug.LogError("Firebase 초기화 실패: " + task.Result);
                onFailure?.Invoke(new Exception("Firebase 초기화 실패: " + task.Result));
            }
        });
    }
    
    public async static UniTask InitializeFirebaseAsync()
    {
        if (isInitialized)
        {
            return;
        }
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            isInitialized = true;
            Debug.Log("Firebase Initialized");
        }
        else
        {
            throw new Exception($"Firebase 초기화 실패: {dependencyStatus}");
        }
    }

    /// <summary>
    /// 데이터를 생성하거나 지정된 경로에 저장합니다.
    /// </summary>
    /// <typeparam name="T">저장할 데이터의 타입입니다.</typeparam>
    /// <param name="path">데이터를 저장할 Firebase Database 경로입니다.</param>
    /// <param name="data">저장할 데이터 객체입니다.</param>
    /// <param name="onSuccess">작업이 성공하면 호출되는 콜백입니다.</param>
    /// <param name="onFailure">작업이 실패하면 호출되는 콜백입니다.</param>
    /// <example>
    /// <code>
    /// // 새로운 사용자 생성
    /// User newUser = new User("Alice", "alice@example.com");
    /// string userId = FirebaseDatabaseAPI.GenerateKey("users");
    /// 
    /// // 데이터 생성 호출
    /// FirebaseDatabaseAPI.CreateData($"users/{userId}", newUser,
    ///     onSuccess: () => Debug.Log("사용자 생성 완료"),
    ///     onFailure: (exception) => Debug.LogError("사용자 생성 실패: " + exception.Message)
    /// );
    /// </code>
    /// </example>
    public static void CreateData<T>(string path, T data, Action onSuccess = null, Action<Exception> onFailure = null)
    {
        EnsureInitialized(() =>
        {
            string jsonData = JsonConvert.SerializeObject(data);
            databaseReference.Child(path).SetRawJsonValueAsync(jsonData).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log($"데이터 생성 완료: {path}");
                    onSuccess?.Invoke();
                }
                else
                {
                    Debug.LogError($"데이터 생성 실패: {path}");
                    onFailure?.Invoke(task.Exception);
                }
            });
        }, onFailure);
    }

    /// <summary>
    /// 지정된 경로에서 데이터를 읽어옵니다.
    /// </summary>
    /// <typeparam name="T">읽어올 데이터의 타입입니다.</typeparam>
    /// <param name="path">데이터를 읽어올 Firebase Database 경로입니다.</param>
    /// <param name="onSuccess">데이터를 성공적으로 읽어오면 호출되는 콜백입니다.</param>
    /// <param name="onFailure">데이터 읽기에 실패하면 호출되는 콜백입니다.</param>
    /// <example>
    /// <code>
    /// string userId = "user123";
    /// FirebaseDatabaseAPI.ReadData&lt;User&gt;($"users/{userId}",
    ///     onSuccess: (user) =>
    ///     {
    ///         if (user != null)
    ///         {
    ///             Debug.Log("사용자 정보: " + user.username);
    ///         }
    ///         else
    ///         {
    ///             Debug.Log("사용자를 찾을 수 없습니다.");
    ///         }
    ///     },
    ///     onFailure: (exception) => Debug.LogError("데이터 읽기 실패: " + exception.Message)
    /// );
    /// </code>
    /// </example>
    public static void ReadData<T>(string path, Action<T> onSuccess, Action<Exception> onFailure = null)
    {
        EnsureInitialized(() =>
        {
            databaseReference.Child(path).GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        string jsonData = snapshot.GetRawJsonValue();
                        T data = JsonConvert.DeserializeObject<T>(jsonData);
                        onSuccess?.Invoke(data);
                    }
                    else
                    {
                        Debug.LogWarning($"데이터 없음: {path}");
                        onSuccess?.Invoke(default);
                    }
                }
                else
                {
                    Debug.LogError($"데이터 읽기 실패: {path}");
                    onFailure?.Invoke(task.Exception);
                }
            });
        }, onFailure);
    }
    
    
    
    /// <summary>
    /// 여러 경로에서 데이터를 병렬로 읽어옵니다.
    /// </summary>
    /// <typeparam name="T">읽어올 데이터의 타입입니다.</typeparam>
    /// <param name="paths">읽어올 Firebase Database 경로 목록입니다.</param>
    /// <param name="onSuccess">성공적으로 데이터를 읽어오면 호출되는 콜백 (경로와 데이터 쌍 딕셔너리 반환).</param>
    /// <param name="onFailure">데이터 읽기에 실패하면 호출되는 콜백입니다.</param>
    public static void ReadMultipleData<T>(List<string> paths, Action<Dictionary<string, T>> onSuccess, Action<Exception> onFailure = null)
    {
        EnsureInitialized(() =>
        {
            var results = new Dictionary<string, T>();
            int remainingTasks = paths.Count;

            foreach (var path in paths)
            {
                databaseReference.Child(path).GetValueAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted)
                    {
                        DataSnapshot snapshot = task.Result;
                        if (snapshot.Exists)
                        {
                            string jsonData = snapshot.GetRawJsonValue();
                            T data = JsonConvert.DeserializeObject<T>(jsonData);
                            results[path] = data;
                        }
                        else
                        {
                            Debug.LogWarning($"데이터 없음: {path}");
                            results[path] = default;
                        }
                    }
                    else
                    {
                        Debug.LogError($"데이터 읽기 실패: {path}");
                    }

                    // 모든 경로의 처리가 완료되었는지 확인
                    remainingTasks--;
                    if (remainingTasks == 0)
                    {
                        if (results.Count == paths.Count)
                        {
                            onSuccess?.Invoke(results);
                        }
                        else
                        {
                            onFailure?.Invoke(new Exception("일부 데이터 읽기 실패."));
                        }
                    }
                });
            }
        }, onFailure);
    }


    /// <summary>
    /// 지정된 경로의 데이터를 업데이트합니다.
    /// </summary>
    /// <param name="path">업데이트할 데이터의 Firebase Database 경로입니다.</param>
    /// <param name="updates">업데이트할 데이터의 키-값 쌍입니다.</param>
    /// <param name="onSuccess">업데이트가 성공하면 호출되는 콜백입니다.</param>
    /// <param name="onFailure">업데이트에 실패하면 호출되는 콜백입니다.</param>
    /// <example>
    /// <code>
    /// string userId = "user123";
    /// var updates = new Dictionary&lt;string, object&gt;
    /// {
    ///     { "email", "newemail@example.com" }
    /// };
    /// FirebaseDatabaseAPI.UpdateData($"users/{userId}", updates,
    ///     onSuccess: () => Debug.Log("사용자 정보 업데이트 완료"),
    ///     onFailure: (exception) => Debug.LogError("데이터 업데이트 실패: " + exception.Message)
    /// );
    /// </code>
    /// </example>
    public static void UpdateData(string path, Dictionary<string, object> updates, Action onSuccess = null, Action<Exception> onFailure = null)
    {
        EnsureInitialized(() =>
        {
            databaseReference.Child(path).UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    onFailure?.Invoke(task.Exception);
                    Debug.LogError("데이터 업데이트 실패");
                }
                else if (task.IsCanceled)
                {
                    onFailure?.Invoke(new Exception("데이터 업데이트가 취소되었습니다."));
                    Debug.LogError("데이터 업데이트 취소됨");
                }
                else
                {
                    onSuccess?.Invoke();
                    Debug.Log("데이터 업데이트 완료");
                }
            });
        }, onFailure);
    }



    /// <summary>
    /// 지정된 경로의 데이터를 삭제합니다.
    /// </summary>
    /// <param name="path">삭제할 데이터의 Firebase Database 경로입니다.</param>
    /// <param name="onSuccess">삭제가 성공하면 호출되는 콜백입니다.</param>
    /// <param name="onFailure">삭제에 실패하면 호출되는 콜백입니다.</param>
    /// <example>
    /// <code>
    /// string userId = "user123";
    /// FirebaseDatabaseAPI.DeleteData($"users/{userId}",
    ///     onSuccess: () => Debug.Log("사용자 삭제 완료"),
    ///     onFailure: (exception) => Debug.LogError("데이터 삭제 실패: " + exception.Message)
    /// );
    /// </code>
    /// </example>
    public static void DeleteData(string path, Action onSuccess = null, Action<Exception> onFailure = null)
    {
        EnsureInitialized(() =>
        {
            databaseReference.Child(path).RemoveValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log($"데이터 삭제 완료: {path}");
                    onSuccess?.Invoke();
                }
                else
                {
                    Debug.LogError($"데이터 삭제 실패: {path}");
                    onFailure?.Invoke(task.Exception);
                }
            });
        }, onFailure);
    }

    /// <summary>
    /// 지정된 경로의 데이터 변경 사항을 수신 대기합니다.
    /// </summary>
    /// <typeparam name="T">수신할 데이터의 타입입니다.</typeparam>
    /// <param name="path">데이터 변경 사항을 수신할 Firebase Database 경로입니다.</param>
    /// <param name="onDataChanged">데이터가 변경되면 호출되는 콜백입니다.</param>
    /// <param name="onError">데이터 수신 중 오류가 발생하면 호출되는 콜백입니다.</param>
    /// <example>
    /// <code>
    /// string userId = "user123";
    /// FirebaseDatabaseAPI.ListenForDataChanges&lt;User&gt;($"users/{userId}",
    ///     onDataChanged: (user) =>
    ///     {
    ///         if (user != null)
    ///         {
    ///             Debug.Log("데이터 변경 감지: " + user.username);
    ///         }
    ///         else
    ///         {
    ///             Debug.Log("사용자 데이터가 없습니다.");
    ///         }
    ///     },
    ///     onError: (exception) => Debug.LogError("데이터 수신 오류: " + exception.Message)
    /// );
    /// </code>
    /// </example>
    public static void ListenForDataChanges<T>(string path, Action<T> onDataChanged, Action<Exception> onError = null)
    {
        EnsureInitialized(() =>
        {
            if (listeners.ContainsKey(path))
            {
                Debug.LogWarning($"이미 수신 대기 중인 경로: {path}");
                return;
            }

            EventHandler<ValueChangedEventArgs> listener = (object sender, ValueChangedEventArgs args) =>
            {
                if (args.DatabaseError != null)
                {
                    Debug.LogError($"데이터 변경 수신 오류: {args.DatabaseError.Message}");
                    onError?.Invoke(new Exception(args.DatabaseError.Message));
                    return;
                }

                if (args.Snapshot.Exists)
                {
                    string jsonData = args.Snapshot.GetRawJsonValue();
                    T data = JsonUtility.FromJson<T>(jsonData);
                    onDataChanged?.Invoke(data);
                }
                else
                {
                    onDataChanged?.Invoke(default);
                }
            };

            databaseReference.Child(path).ValueChanged += listener;
            listeners.Add(path, listener);
        }, exception => onError?.Invoke(exception));
    }

    /// <summary>
    /// 지정된 경로의 데이터 변경 사항 수신을 중지합니다.
    /// </summary>
    /// <param name="path">수신 대기를 중지할 Firebase Database 경로입니다.</param>
    /// <example>
    /// <code>
    /// string userId = "user123";
    /// FirebaseDatabaseAPI.StopListeningForDataChanges($"users/{userId}");
    /// </code>
    /// </example>
    public static void StopListeningForDataChanges(string path)
    {
        if (listeners.ContainsKey(path))
        {
            databaseReference.Child(path).ValueChanged -= listeners[path];
            listeners.Remove(path);
            Debug.Log($"데이터 변경 수신 중지: {path}");
        }
        else
        {
            Debug.LogWarning($"수신 대기 중인 리스너 없음: {path}");
        }
    }

    /// <summary>
    /// 고유한 키를 생성합니다.
    /// </summary>
    /// <returns>생성된 고유 키를 반환합니다.</returns>
    /// <example>
    /// <code>
    /// string newKey = FirebaseDatabaseAPI.GenerateKey("users");
    /// Debug.Log("생성된 키: " + newKey);
    /// </code>
    /// </example>
    public static string GenerateKey()
    {
        // Firebase 초기화 확인
        if (!isInitialized)
        {
            Debug.Log("Firebase 초기화 중...");
            InitializeFirebase(() =>
                {
                    Debug.Log("Firebase 초기화 완료");
                },
                exception =>
                {
                    Debug.LogError("Firebase 초기화 실패: " + exception.Message);
                });
        }

        // 초기화 실패 시 null 반환
        if (!isInitialized)
        {
            Debug.LogError("Firebase가 초기화되지 않았습니다.");
            return null;
        }

        // 초기화 완료 후 고유 키 생성
        return databaseReference.Push().Key;
    }

    /// <summary>
    /// Firebase가 초기화되었는지 확인하고, 초기화되었으면 지정된 작업을 수행합니다.
    /// </summary>
    /// <param name="onInitialized">Firebase가 초기화되었을 때 호출되는 콜백입니다.</param>
    /// <param name="onFailure">초기화에 실패하면 호출되는 콜백입니다.</param>
    private static void EnsureInitialized(Action onInitialized, Action<Exception> onFailure)
    {
        if (isInitialized)
        {
            onInitialized?.Invoke();
        }
        else
        {
            InitializeFirebase(() => onInitialized?.Invoke(), onFailure);
        }
    }
}
