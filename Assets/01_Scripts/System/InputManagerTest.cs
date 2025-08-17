using UnityEngine;
using Project.Utility;

/// <summary>
/// InputManager 리팩토링 테스트 스크립트
/// Save System과의 통합을 확인하기 위한 데모
/// </summary>
public class InputManagerTest : MonoBehaviour
{
    [Header("테스트 설정")]
    [SerializeField] private bool enableTestLogs = true;
    
    private void Start()
    {
        if (enableTestLogs)
        {
            DebugLog.Log("[InputManagerTest] 테스트 시작");
            TestInputManagerIntegration();
        }
    }

    private void Update()
    {
        // 간단한 입력 테스트
        if (InputManager.IsKeyPressed(InputManager.InputType.Jump))
        {
            DebugLog.Log("[InputManagerTest] 점프 키가 눌렸습니다!");
        }

        if (InputManager.IsKeyPressed(InputManager.InputType.Attack))
        {
            DebugLog.Log("[InputManagerTest] 공격 키가 눌렸습니다!");
        }

        // 테스트용 키 바인딩 변경 (F1 키로)
        if (Input.GetKeyDown(KeyCode.F1))
        {
            TestKeyBindingChange();
        }

        // 테스트용 기본값 초기화 (F2 키로)
        if (Input.GetKeyDown(KeyCode.F2))
        {
            TestResetToDefaults();
        }
    }

    private void TestInputManagerIntegration()
    {
        DebugLog.Log("[InputManagerTest] InputManager 초기화 상태 확인:");
        DebugLog.Log($"- 초기화됨: {InputManager.IsInitialized()}");
        
        // 현재 키 바인딩 출력
        var allBindings = InputManager.GetAllKeyBindings();
        DebugLog.Log($"[InputManagerTest] 현재 키 바인딩 수: {allBindings.Count}");
        
        foreach (var binding in allBindings)
        {
            DebugLog.Log($"- {InputManager.GetActionName(binding.Key)}: {binding.Value}");
        }

        // GameData 상태 확인
        if (GameData.Instance != null)
        {
            DebugLog.Log($"[InputManagerTest] GameData 초기화됨: {GameData.Instance.SaveData != null}");
            if (GameData.Instance.SaveData?.InputSettings != null)
            {
                DebugLog.Log($"[InputManagerTest] 저장된 키 바인딩 수: {GameData.Instance.SaveData.InputSettings.KeyBindings.Count}");
            }
        }
        else
        {
            DebugLog.Warning("[InputManagerTest] GameData가 초기화되지 않았습니다.");
        }
    }

    private void TestKeyBindingChange()
    {
        DebugLog.Log("[InputManagerTest] 키 바인딩 변경 테스트");
        
        // 점프 키를 J로 변경
        InputManager.SetKeyBinding(InputManager.InputType.Jump, KeyCode.J);
        DebugLog.Log("점프 키를 J로 변경했습니다.");
        
        // 변경된 키 바인딩 확인
        KeyCode newJumpKey = InputManager.GetKeyBinding(InputManager.InputType.Jump);
        DebugLog.Log($"새로운 점프 키: {newJumpKey}");
        
        // 저장 확인
        if (GameData.Instance?.SaveData?.InputSettings != null)
        {
            DebugLog.Log($"저장된 점프 키: {GameData.Instance.SaveData.InputSettings.KeyBindings["Jump"]}");
        }
    }

    private void TestResetToDefaults()
    {
        DebugLog.Log("[InputManagerTest] 기본값 초기화 테스트");
        
        InputManager.ResetToDefaults();
        DebugLog.Log("키 설정을 기본값으로 초기화했습니다.");
        
        // 초기화 후 점프 키 확인
        KeyCode jumpKey = InputManager.GetKeyBinding(InputManager.InputType.Jump);
        DebugLog.Log($"초기화된 점프 키: {jumpKey}");
    }

    private void OnGUI()
    {
        if (!enableTestLogs) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("InputManager 테스트");
        GUILayout.Label($"초기화됨: {InputManager.IsInitialized()}");
        GUILayout.Label($"점프 키: {InputManager.GetKeyBinding(InputManager.InputType.Jump)}");
        GUILayout.Label($"공격 키: {InputManager.GetKeyBinding(InputManager.InputType.Attack)}");
        GUILayout.Label("F1: 키 바인딩 변경 테스트");
        GUILayout.Label("F2: 기본값 초기화 테스트");
        GUILayout.EndArea();
    }
}
