using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;

public class TestUpdate : MonoBehaviour
{
    private DatabaseReference mDatabase;
    void Start() 
    {
        // Get the root reference location of the database.
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        WriteNewScore("updatetest",6);
    }
    
    private void WriteNewScore(string userId, int score) {
        // Create new entry at /user-scores/$userid/$scoreid and at
        // /leaderboard/$scoreid simultaneously
        string key = mDatabase.Child("scores").Push().Key;
        LeaderboardEntry entry = new LeaderboardEntry(userId, score);
        Dictionary<string, object> entryValues = entry.ToDictionary();

        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        childUpdates["/scores/" + key] = entryValues;
        childUpdates["/user-scores/" + userId + "/" + key] = entryValues;

        mDatabase.UpdateChildrenAsync(childUpdates);
    }
    
    
    public class LeaderboardEntry {
        public string uid;
        public int score = 0;

        public LeaderboardEntry() 
        {
            
        }

        public LeaderboardEntry(string uid, int score) {
            this.uid = uid;
            this.score = score;
        }

        public Dictionary<string, object> ToDictionary() {
            Dictionary<string, object> result = new Dictionary<string, object>();
            result["uid"] = uid;
            result["score"] = score;

            return result;
        }
    }
}
