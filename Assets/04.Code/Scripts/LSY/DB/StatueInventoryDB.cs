using System.IO;
using GLTFast;
using UnityEngine;

public class StatueInventoryDB: MonoBehaviour
{
    private string _userId;
    private string[] hasModel;
    private string[] hasModelName;
    private StorageDatabase _storageDatabase;
    public WebApiData webApiData;
    private RaycastHit Hit;
    private Vector3 pos;
    [SerializeField] private LayerMask BFLayerMask; 

    public void Start()
    {
        
    }
    public void StatuePlace()
    {
        LoadAndInstantiateGLB(webApiData.ModelName);
        

    }
    
    public async void LoadAndInstantiateGLB(string fName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fName);
        //string filePath = Path.Combine(Application.dataPath, "DownloadedGLB", "958462d6cbd04f441aad81e88529b8a2.glb");
        // 파일이 존재하는지 확인
        if (File.Exists(filePath))
        {
            // glTFast를 이용해 GLB 파일을 로드
            GltfImport gltfImport = new GltfImport();
            bool success = await gltfImport.Load(filePath); // 비동기 로드 처리

            // 파일 로드에 성공하면 씬에 배치
            if (success)
            {
                GameObject glbObject = new GameObject("GLBModel");
                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                if (Physics.Raycast(ray, out Hit, Mathf.Infinity, BFLayerMask))
                {
                    pos = Hit.point;

                    pos = new Vector3(
                        Mathf.Floor(pos.x),
                        Mathf.Floor(pos.y),
                        Mathf.Floor(pos.z));

                    pos += (Vector3.right * 0.5f) + (Vector3.forward * 0.5f);

                    await gltfImport.InstantiateMainSceneAsync(glbObject.transform);
                    glbObject.transform.position = pos;
                    Debug.Log("GLB file instantiated in scene.");
                }
            }
            else
            {
                Debug.LogError("Failed to load GLB file using glTFast.");
            }
        }
        else
        {
            Debug.LogError("File does not exist: " + filePath);
        }
    }
    //private void Start()
    //{
        // _storageDatabase = new StorageDatabase(webApiData);
        // if (UserData.Instance ==null)
        //     return;
        // _userId = UserData.Instance.UserId;
        //
        // LoadUserModelData();
        
    //}

    // public void LoadUserModelData()
    // {
    //     RealtimeDatabase.GetModelList(_userId, modelList =>
    //         {
    //             if (modelList != null)
    //             {
    //                 Debug.Log($"StatueInventoryDB : modelList - {modelList}");
    //                 foreach (var model in modelList.models)
    //                 {
    //                     Debug.Log($"StatueInventoryDB : model - {model}");
    //                 }
    //             }
    //             else
    //             {
    //                 Debug.LogError("StaueInventoryDB : 모델리스트 데이터를 찾을 수 없음.");
    //             }
    //         },
    //         error => { Debug.LogError($"StatueInventoryDB : 데이터 읽는 중 오류 -{error.Message}"); }
    //     );
    // }

    
}