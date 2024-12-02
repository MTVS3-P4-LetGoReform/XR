using Cysharp.Threading.Tasks;
using Firebase.Storage;
using UnityEngine;

public class DBTestCode : MonoBehaviour
{
    private FirebaseStorage storage;
    private StorageReference storage_ref;
    private StorageReference isstorage_ref;
    private string local_url;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    public async UniTask DownImage(string imageName)
    {
        isstorage_ref = storage_ref.Child("images" + "/" + imageName);
        Debug.Log("StorageDatabase : isstorage_ref - "+ isstorage_ref);
        local_url = Application.persistentDataPath + "/" + imageName;
        Debug.Log(string.Format("{0}", local_url));
        await isstorage_ref.GetFileAsync(local_url);
    }
}
