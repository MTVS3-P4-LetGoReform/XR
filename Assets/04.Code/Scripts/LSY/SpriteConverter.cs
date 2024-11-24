using System.IO;
using UnityEngine;
using UnityEngine.UI;

public static class SpriteConverter
{
    public static Sprite ConvertFromPNG(string fName)
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
                return sprite;
            }
        }
        else
        {
            Debug.LogError("File does not Exist!!");
        }

        return null;
    }
    
}