using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Project.Utility;

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
            DebugLog.Log($"[SaveSystem] 저장 완료: {SavePath}");
        }
        catch (Exception e)
        {
            DebugLog.Error($"[SaveSystem] 저장 실패: {e.Message}");
        }
    }

    public static PlayerSaveData Load()
    {
        if (!File.Exists(SavePath))
        {
            DebugLog.Warning("[SaveSystem] 저장 파일이 존재하지 않음.");
            return null;
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            return JsonConvert.DeserializeObject<PlayerSaveData>(json);
        }
        catch (Exception e)
        {
            DebugLog.Error($"[SaveSystem] 로드 실패: {e.Message}");
            return null;
        }
    }

    public static void Delete()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            DebugLog.Log("[SaveSystem] 저장 파일 삭제됨");
        }
    }
}
