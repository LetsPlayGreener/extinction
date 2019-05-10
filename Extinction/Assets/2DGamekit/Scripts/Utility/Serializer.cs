using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Serializer
{

    /// <summary>
    /// Serialize obj and save it in a file at Application.persistantDataPath/filePath. If the file already exists it will be overwitten.
    /// </summary>
    /// <param name="filePath">The path where the file will be saved from Application.persistantDataPath</param>
    public static void SaveData(string filePath, System.Object obj)
    {
        BinaryFormatter binary = new BinaryFormatter();
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        FileStream file = File.Create(filePath);
        binary.Serialize(file, obj);
        file.Close();
    }

    /// <summary>
    /// If the file exists, load its data to "obj".
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filePath">The path of the loaded file.</param>
    /// <param name="obj">The object where the data will be stored.</param>
    public static void LoadData<T>(string filePath, out T obj)
    {
        obj = default(T);
        if (File.Exists(filePath))
        {
            BinaryFormatter binary = new BinaryFormatter();
            FileStream file = File.Open(filePath, FileMode.Open);
            obj = (T)binary.Deserialize(file);
            file.Close();
        }
        else
        {
            //Debug.Log("File not found.");
        }
    }

    /// <summary>
    /// Retrun a string containing the object binarized, then converted to base64.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string SerializeData(System.Object obj)
    {
        BinaryFormatter binary = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream();
        binary.Serialize(memoryStream, obj);
        return Convert.ToBase64String(memoryStream.ToArray());
    }

    /// <summary>
    /// Deserialize an object from a string containing binarized data converted to base64;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializedData"></param>
    /// <param name="obj"></param>
    public static void DeserializeData<T>(string serializedData, out T obj)
    {
        BinaryFormatter binary = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream();
        memoryStream.Write(Convert.FromBase64String(serializedData), 0, Convert.FromBase64String(serializedData).Length);
        memoryStream.Position = 0;
        obj = (T)binary.Deserialize(memoryStream);
    }
}
