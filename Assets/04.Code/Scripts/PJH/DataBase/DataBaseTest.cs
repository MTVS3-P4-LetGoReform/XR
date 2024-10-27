using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;


public class DataBase : MonoBehaviour
{
    private DatabaseReference _reference;

    //private string _databaseUrl = "https://mtvs-p4-default-rtdb.firebaseio.com/";
    private void Start()
    {
        //Firebase();
        _reference = FirebaseDatabase.DefaultInstance.RootReference;
        //UpdateUsername("Updatename");
        //WriteNewUser("User_id3", "testname");
        Test();
    }
    
    private void Firebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                _reference = FirebaseDatabase.DefaultInstance.RootReference;
                //WriteNewUser("testId", "testname", "testemail");
                FbUpdate("updatename2");
            }
        });
    }
    
    private void Create(string id, string name)
    {
        var friends = FriendList();
        User user = new User(id, name, friends);

        string json = JsonConvert.SerializeObject(user);

        _reference.Child("Users").Child($"{id}").SetRawJsonValueAsync(json);
    }
    
    private void FbUpdate(string name)
    {
        _reference.Child("Users").Child("User_id").SetValueAsync(name);
    }

    private void Test()
    {
        _reference.Child("Users").Child("User_id").Child("user_id").SetRawJsonValueAsync("testid1");
    }
    
    

    private List<Friend> FriendList()
    {
        List<Friend> friends = new List<Friend>();
        Friend friend1 = new Friend("testFriendId1", "testFriendname1");
        Friend friend2 = new Friend("testFriendId2", "testFriendname2");
        friends.Add(friend1);
        friends.Add(friend2);

        return friends;
    }
}

