using System.IO;
using UnityEngine;

public static class PathConverter{
    public static string GetImagePath(string fName){
        return Path.Combine(Application.persistentDataPath,"Images",fName);
    }
    public static string GetModelPath(string fName){
        return Path.Combine(Application.persistentDataPath,"Models",fName);
    }
}