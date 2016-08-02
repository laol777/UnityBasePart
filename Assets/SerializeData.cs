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

    public static string Load(string nameFile)
    {
        StreamReader streamReader = File.OpenText(Application.persistentDataPath + "/" + nameFile + ".json");
        string info = streamReader.ReadToEnd();
        streamReader.Close();
        return info;
    }

    public static List<T> LoadJSON<T>(string nameFile)
    {
        return JsonConvert.DeserializeObject<List<T>>(Load(nameFile));
    }
}
