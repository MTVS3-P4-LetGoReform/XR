using System;
using System.Collections.Generic;
using UnityEngine;

public class DBTest : MonoBehaviour
{
    private string testUserId;

    private void Start()
    {
        // Firebase 초기화 후 CRUD 작업 수행
        RealtimeDatabase.InitializeFirebase(onInitialized: () =>
        {
            Debug.Log("Firebase 초기화 완료");
            RunAllTests();
        },
        onFailure: (exception) => Debug.LogError("Firebase 초기화 실패: " + exception.Message));
    }

    /// <summary>
    /// 모든 CRUD 작업을 테스트합니다.
    /// </summary>
    private void RunAllTests()
    {
        // 1. User 테스트
        testUserId = RealtimeDatabase.GenerateKey("users");
        CreateUser();
        Invoke(nameof(ReadUser), 3f);
        Invoke(nameof(UpdateUser), 6f);
        //Invoke(nameof(DeleteUser), 9f);

        // 2. UserLand 테스트 (유저 생성 이후 테스트)
        Invoke(nameof(CreateUserLand), 9f);
        Invoke(nameof(ReadUserLand), 12f);
        Invoke(nameof(AddObjectToUserLand), 15f);
        //Invoke(nameof(DeleteObjectFromUserLand), 21f);

        /*// 3. FriendList 테스트 (유저 생성 이후 테스트)
        Invoke(nameof(AddFriend), 24f);
        Invoke(nameof(ReadFriendList), 27f);
        //Invoke(nameof(RemoveFriend), 30f);*/
    }

    /// <summary>
    /// 유저 생성 예제
    /// </summary>
    private void CreateUser()
    {
        User newUser = new User("Alice", "alice@example.com", "https://example.com/alice.jpg", true, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        RealtimeDatabase.CreateUser(testUserId, newUser,
            onSuccess: () => Debug.Log("유저 생성 완료"),
            onFailure: (exception) => Debug.LogError("유저 생성 실패: " + exception.Message));
    }

    /// <summary>
    /// 유저 읽기 예제
    /// </summary>
    private void ReadUser()
    {
        RealtimeDatabase.GetUser(testUserId, user =>
        {
            if (user != null)
            {
                Debug.Log($"유저 정보: 이름 = {user.name}, 이메일 = {user.email}, 온라인 상태 = {user.onlineStatus}");
            }
            else
            {
                Debug.Log("유저 정보를 찾을 수 없습니다.");
            }
        },
        onFailure: (exception) => Debug.LogError("유저 정보 읽기 실패: " + exception.Message));
    }

    /// <summary>
    /// 유저 업데이트 예제
    /// </summary>
    private void UpdateUser()
    {
        var updates = new Dictionary<string, object>
        {
            { "onlineStatus", false },
            { "lastLogin", DateTimeOffset.UtcNow.ToUnixTimeSeconds() }
        };

        RealtimeDatabase.UpdateUser(testUserId, updates,
            onSuccess: () => Debug.Log("유저 정보 업데이트 완료"),
            onFailure: (exception) => Debug.LogError("유저 정보 업데이트 실패: " + exception.Message));
    }

    /// <summary>
    /// 유저 삭제 예제
    /// </summary>
    private void DeleteUser()
    {
        RealtimeDatabase.DeleteUser(testUserId,
            onSuccess: () => Debug.Log("유저 삭제 완료"),
            onFailure: (exception) => Debug.LogError("유저 삭제 실패: " + exception.Message));
    }

    /// <summary>
    /// 유저 영지 생성 예제
    /// </summary>
    private void CreateUserLand()
    {
        UserLand userLand = new UserLand();
        
        // 더미 데이터
        LandObject defaultObject = new LandObject("dummy", new Vector3(0, 0, 0), Vector3.zero, Vector3.one);
        userLand.AddObject(defaultObject);
        
        RealtimeDatabase.SetUserLand(testUserId, userLand,
            onSuccess: () => Debug.Log("유저 영지 생성 완료"),
            onFailure: (exception) => Debug.LogError("유저 영지 생성 실패: " + exception.Message));
    }

    /// <summary>
    /// 유저 영지 읽기 예제
    /// </summary>
    private void ReadUserLand()
    {
        RealtimeDatabase.GetUserLand(testUserId, userLand =>
        {
            if (userLand != null)
            {
                Debug.Log($"유저 영지 정보: 오브젝트 개수 = {userLand.objects.Count}");
            }
            else
            {
                Debug.Log("유저 영지 정보를 찾을 수 없습니다.");
            }
        },
        onFailure: (exception) => Debug.LogError("유저 영지 읽기 실패: " + exception.Message));
    }

    /// <summary>
    /// 유저 영지에 오브젝트 추가 예제
    /// </summary>
    private void AddObjectToUserLand()
    {
        LandObject landObject = new LandObject("Tree", new Vector3(1, 1, 1), Vector3.zero, Vector3.one);
        RealtimeDatabase.AddObjectToUserLand(testUserId, landObject,
            onSuccess: () => Debug.Log("영지 오브젝트 추가 완료"),
            onFailure: (exception) => Debug.LogError("영지 오브젝트 추가 실패: " + exception.Message));
    }

    /// <summary>
    /// 유저 영지에서 오브젝트 삭제 예제
    /// </summary>
    private void DeleteObjectFromUserLand()
    {
        RealtimeDatabase.GetUserLand(testUserId, userLand =>
        {
            if (userLand != null && userLand.objects.Count > 0)
            {
                userLand.objects.RemoveAt(0); // 첫 번째 오브젝트 삭제
                RealtimeDatabase.SetUserLand(testUserId, userLand,
                    onSuccess: () => Debug.Log("영지 오브젝트 삭제 완료"),
                    onFailure: (exception) => Debug.LogError("영지 오브젝트 삭제 실패: " + exception.Message));
            }
            else
            {
                Debug.Log("삭제할 영지 오브젝트가 없습니다.");
            }
        },
        onFailure: (exception) => Debug.LogError("유저 영지 조회 실패: " + exception.Message));
    }

    /// <summary>
    /// 친구 추가 예제
    /// </summary>
    private void AddFriend()
    {
        Friend friend = new Friend("friend123", "Bob", true);
        RealtimeDatabase.AddFriend(testUserId, friend,
            onSuccess: () => Debug.Log("친구 추가 완료"),
            onFailure: (exception) => Debug.LogError("친구 추가 실패: " + exception.Message));
    }

    /// <summary>
    /// 친구 목록 읽기 예제
    /// </summary>
    private void ReadFriendList()
    {
        RealtimeDatabase.GetFriendList(testUserId, friendList =>
            {
                if (friendList != null && friendList.friends.Count > 0)
                {
                    Debug.Log("친구 목록:");

                    // 친구 목록 순회하여 ID와 이름 출력
                    foreach (var friendEntry in friendList.friends)
                    {
                        string friendId = friendEntry.Key;
                        Friend friend = friendEntry.Value;
                
                        Debug.Log($"친구 ID: {friendId}, 닉네임: {friend.name}");
                    }
                }
                else
                {
                    Debug.Log("친구 목록이 없습니다.");
                }
            },
            onFailure: (exception) => Debug.LogError("친구 목록 조회 실패: " + exception.Message));
    }


    /// <summary>
    /// 친구 삭제 예제
    /// </summary>
    private void RemoveFriend()
    {
        RealtimeDatabase.RemoveFriend(testUserId, "friend123",
            onSuccess: () => Debug.Log("친구 삭제 완료"),
            onFailure: (exception) => Debug.LogError("친구 삭제 실패: " + exception.Message));
    }
}
