using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public static class SaveSystem
{
    private const string SaveFileName = "save.json";

    private static string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    public static void Save(PlayerSaveData data)
    {
        try
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(SavePath, json);
            Debug.Log($"[SaveSystem] ���� �Ϸ�: {SavePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] ���� ����: {e.Message}");
        }
    }

    public static PlayerSaveData Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("[SaveSystem] ���� ������ �������� ����.");
            return null;
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            return JsonConvert.DeserializeObject<PlayerSaveData>(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] �ε� ����: {e.Message}");
            return null;
        }
    }

    public static void Delete()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("[SaveSystem] ���� ���� ������");
        }
    }
}
