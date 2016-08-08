using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public static class SerializeData{

    public static void Save(string data, string nameFile)
    {
        FileInfo fileInfo = new FileInfo(Application.persistentDataPath + "/" + nameFile + ".json");
        StreamWriter streamWriter;
        if (!fileInfo.Exists)
        {
            streamWriter = fileInfo.CreateText();
        }
        else
        {
            fileInfo.Delete();
            streamWriter = fileInfo.CreateText();
        }
        streamWriter.WriteLine(data);
        streamWriter.Close();
    }

    public static void SaveJSON<T>(List<T> serializeData, string nameFile)
    {
        Save(JsonConvert.SerializeObject(serializeData), nameFile);
    }

    public static string Load(string path, string nameFile)
    {
        StreamReader streamReader = File.OpenText(path + "\\" + nameFile + ".json");
        string info = streamReader.ReadToEnd();
        streamReader.Close();
        return info;
    }

    public static List<T> LoadJSON<T>(string nameFile)
    {
        try
        {
            return JsonConvert.DeserializeObject<List<T>>(Load(Application.persistentDataPath, nameFile));
        }
        catch
        {
             return JsonConvert.DeserializeObject<List<T>>(Load("C:\\BasePart\\Assets\\", nameFile));
        }
    }
}
