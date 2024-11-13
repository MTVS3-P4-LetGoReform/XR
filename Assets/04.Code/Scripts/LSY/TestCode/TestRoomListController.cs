using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TestRoomListController : MonoBehaviour
{
    public Image RoomListImage;
    public WebApiData webApiData;
    void Start()
    {
        SetGenImage();
    }
    
    public void SetGenImage()
    {
        if(webApiData.ImageName != null){
            ConvertSpriteFromPNG(RoomListImage, webApiData.ImageName);
        }
    }
    
    public void ConvertSpriteFromPNG(Image targetImage, string fName)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "Images");
        string filePath = Path.Combine(folderPath, fName);

        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            // x
            Texture2D texture = new Texture2D(2, 2);

            if (texture.LoadImage(fileData))
            {
                Sprite sprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                targetImage.sprite = sprite;
            }
        }
    }
}
