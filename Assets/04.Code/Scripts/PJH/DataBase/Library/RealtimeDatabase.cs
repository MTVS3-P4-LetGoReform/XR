using Firebase;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public static partial class RealtimeDatabase
{
    private static DatabaseReference _databaseReference;
    private static bool _isInitialized = false;
    
    /// <summary>
    /// Firebase를 비동기적으로 초기화합니다.
    /// </summary>
    /// <returns>초기화 완료를 나타내는 UniTask</returns>
    public static async UniTask InitializeFirebaseAsync()
    {
        if (_isInitialized)
            return;
            
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            _databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            _isInitialized = true;
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
    public static async UniTask CreateDataAsync<T>(string path, T data)
    {
        await EnsureInitializedAsync();
        
        string jsonData = JsonConvert.SerializeObject(data);
        await _databaseReference.Child(path).SetRawJsonValueAsync(jsonData);
        Debug.Log($"데이터 생성 완료: {path}");
    }

    /// <summary>
    /// 지정된 경로에서 데이터를 읽어옵니다.
    /// </summary>
    /// <typeparam name="T">읽어올 데이터의 타입입니다.</typeparam>
    /// <param name="path">데이터를 읽어올 Firebase Database 경로입니다.</param>
    /// <returns>읽어온 데이터 객체</returns>
    public static async UniTask<T> ReadDataAsync<T>(string path)
    {
        await EnsureInitializedAsync();
        
        var snapshot = await _databaseReference.Child(path).GetValueAsync();
        if (!snapshot.Exists)
        {
            Debug.LogWarning($"데이터 없음: {path}");
            return default;
        }
        
        string jsonData = snapshot.GetRawJsonValue();
        return JsonConvert.DeserializeObject<T>(jsonData);
    }

    /// <summary>
    /// 지정된 경로의 데이터를 업데이트합니다.
    /// </summary>
    /// <param name="path">업데이트할 데이터의 Firebase Database 경로입니다.</param>
    /// <param name="updates">업데이트할 데이터의 키-값 쌍입니다.</param>
    public static async UniTask UpdateDataAsync(string path, Dictionary<string, object> updates)
    {
        await EnsureInitializedAsync();
        
        await _databaseReference.Child(path).UpdateChildrenAsync(updates);
        Debug.Log("데이터 업데이트 완료");
    }

    /// <summary>
    /// 지정된 경로의 데이터를 삭제합니다.
    /// </summary>
    /// <param name="path">삭제할 데이터의 Firebase Database 경로입니다.</param>
    public static async UniTask DeleteDataAsync(string path)
    {
        await EnsureInitializedAsync();
        
        await _databaseReference.Child(path).RemoveValueAsync();
        Debug.Log($"데이터 삭제 완료: {path}");
    }

    /// <summary>
    /// 여러 경로에서 데이터를 병렬로 읽어옵니다.
    /// </summary>
    /// <typeparam name="T">읽어올 데이터의 타입입니다.</typeparam>
    /// <param name="paths">읽어올 Firebase Database 경로 목록입니다.</param>
    /// <returns>경로와 데이터 쌍의 딕셔너리</returns>
    public static async UniTask<Dictionary<string, T>> ReadMultipleDataAsync<T>(List<string> paths)
    {
        await EnsureInitializedAsync();
        
        var tasks = paths.Select(path => ReadDataAsync<T>(path));
        var results = await UniTask.WhenAll(tasks);
        
        return paths.Zip(results, (path, data) => new { path, data })
                    .ToDictionary(x => x.path, x => x.data);
    }

    /// <summary>
    /// Firebase가 초기화되었는지 확인하고, 초기화되지 않았다면 초기화를 수행합니다.
    /// </summary>
    private static async UniTask EnsureInitializedAsync()
    {
        if (!_isInitialized)
        {
            await InitializeFirebaseAsync();
        }
    }

    /// <summary>
    /// 고유한 키를 생성합니다.
    /// </summary>
    /// <returns>생성된 고유 키</returns>
    public static string GenerateKey()
    {
        if (!_isInitialized)
        {
            throw new Exception("Firebase가 초기화되지 않았습니다.");
        }
        return _databaseReference.Push().Key;
    }
    
    private static Dictionary<string, EventHandler<ValueChangedEventArgs>> _listeners = new Dictionary<string, EventHandler<ValueChangedEventArgs>>();

    /// <summary>
    /// 지정된 경로의 데이터 변경 사항을 수신 대기합니다.
    /// </summary>
    /// <typeparam name="T">수신할 데이터의 타입입니다.</typeparam>
    /// <param name="path">데이터 변경 사항을 수신할 Firebase Database 경로입니다.</param>
    /// <param name="onDataChanged">데이터가 변경되면 호출되는 콜백입니다.</param>
    /// <param name="onError">데이터 수신 중 오류가 발생하면 호출되는 콜백입니다.</param>
    public static void ListenForDataChanges<T>(string path, Action<T> onDataChanged, Action<Exception> onError = null)
    {
        if (_listeners.ContainsKey(path))
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
                T data = JsonConvert.DeserializeObject<T>(jsonData);
                onDataChanged?.Invoke(data);
            }
            else
            {
                onDataChanged?.Invoke(default);
            }
        };

        _databaseReference.Child(path).ValueChanged += listener;
        _listeners.Add(path, listener);
    }

    /// <summary>
    /// 지정된 경로의 데이터 변경 사항 수신을 중지합니다.
    /// </summary>
    /// <param name="path">수신 대기를 중지할 Firebase Database 경로입니다.</param>
    public static void StopListeningForDataChanges(string path)
    {
        if (_listeners.ContainsKey(path))
        {
            _databaseReference.Child(path).ValueChanged -= _listeners[path];
            _listeners.Remove(path);
            Debug.Log($"데이터 변경 수신 중지: {path}");
        }
        else
        {
            Debug.LogWarning($"수신 대기 중인 리스너 없음: {path}");
        }
    }

}
