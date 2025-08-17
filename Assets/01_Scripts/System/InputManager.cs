using Project.Utility;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager
{
    public enum InputType
    {
        None,
        MoveLeft,
        MoveRight,
        MoveUp,
        MoveDown,
        Jump,
        Attack,
        Smash,
        Interact,
        SpecialAbility,
        Pause,
        Inventory,
        Run,
        Special_1,
    }

    // 기본 키 설정
    private static Dictionary<InputType, KeyCode> defaultKeyBindings = new Dictionary<InputType, KeyCode>()
    {
        { InputType.None, KeyCode.None },
        { InputType.MoveLeft, KeyCode.LeftArrow },
        { InputType.MoveRight, KeyCode.RightArrow },
        { InputType.MoveUp, KeyCode.UpArrow },
        { InputType.MoveDown, KeyCode.DownArrow },
        { InputType.Jump, KeyCode.C },
        { InputType.Attack, KeyCode.Z },
        { InputType.Smash, KeyCode.X },
        { InputType.Interact, KeyCode.E },
        { InputType.SpecialAbility, KeyCode.Space },
        { InputType.Pause, KeyCode.Escape },
        { InputType.Inventory, KeyCode.Tab },
        { InputType.Run, KeyCode.LeftShift },
        { InputType.Special_1, KeyCode.LeftControl }
    };

    // 현재 키 설정 (런타임에 변경 가능)
    private static Dictionary<InputType, KeyCode> currentKeyBindings;

    // 초기화
    static InputManager()
    {
        LoadKeyBindings();
    }

    // 키 바인딩 설정
    public static void SetKeyBinding(InputType inputType, KeyCode keyCode)
    {
        if (currentKeyBindings.ContainsKey(inputType))
        {
            currentKeyBindings[inputType] = keyCode;
            SaveKeyBindings();
            DebugLog.Log($"{inputType} 키가 {keyCode}로 변경되었습니다.");
        }
    }

    // 키 바인딩 가져오기
    public static KeyCode GetKeyBinding(InputType inputType)
    {
        return currentKeyBindings.ContainsKey(inputType) ? currentKeyBindings[inputType] : KeyCode.None;
    }

    // 입력 체크 메서드들
    public static bool IsKeyPressed(InputType inputType)
    {
        KeyCode key = GetKeyBinding(inputType);
        return key != KeyCode.None && Input.GetKeyDown(key);
    }

    public static bool IsKeyHeld(InputType inputType)
    {
        KeyCode key = GetKeyBinding(inputType);
        return key != KeyCode.None && Input.GetKey(key);
    }

    public static bool IsKeyReleased(InputType inputType)
    {
        KeyCode key = GetKeyBinding(inputType);
        return key != KeyCode.None && Input.GetKeyUp(key);
    }

    // 키 중복 체크
    public static bool IsKeyAlreadyUsed(KeyCode keyCode, InputType excludeType = InputType.None)
    {
        foreach (var binding in currentKeyBindings)
        {
            if (binding.Key != excludeType && binding.Value == keyCode)
            {
                return true;
            }
        }
        return false;
    }

    // 특정 키를 사용하는 InputType 찾기
    public static InputType GetInputTypeByKey(KeyCode keyCode)
    {
        foreach (var binding in currentKeyBindings)
        {
            if (binding.Value == keyCode)
            {
                return binding.Key;
            }
        }
        return InputType.None;
    }

    // 키 설정 저장
    public static void SaveKeyBindings()
    {
        foreach (var binding in currentKeyBindings)
        {
            PlayerPrefs.SetString($"KeyBinding_{binding.Key}", binding.Value.ToString());
        }
        PlayerPrefs.Save();
    }

    // 키 설정 로드
    public static void LoadKeyBindings()
    {
        currentKeyBindings = new Dictionary<InputType, KeyCode>();

        foreach (var defaultBinding in defaultKeyBindings)
        {
            string savedKey = PlayerPrefs.GetString($"KeyBinding_{defaultBinding.Key}", defaultBinding.Value.ToString());
            if (System.Enum.TryParse(savedKey, out KeyCode keyCode))
            {
                currentKeyBindings[defaultBinding.Key] = keyCode;
            }
            else
            {
                currentKeyBindings[defaultBinding.Key] = defaultBinding.Value;
            }
        }
    }

    // 기본 설정으로 초기화
    public static void ResetToDefaults()
    {
        currentKeyBindings.Clear();
        foreach (var defaultBinding in defaultKeyBindings)
        {
            currentKeyBindings[defaultBinding.Key] = defaultBinding.Value;
        }
        SaveKeyBindings();
        Debug.Log("키 설정이 기본값으로 초기화되었습니다.");
    }

    // 모든 키 바인딩 가져오기 (UI에서 사용)
    public static Dictionary<InputType, KeyCode> GetAllKeyBindings()
    {
        return new Dictionary<InputType, KeyCode>(currentKeyBindings);
    }

    // 액션 이름을 한글로 변환 (UI 표시용)
    public static string GetActionName(InputType inputType)
    {
        switch (inputType)
        {
            case InputType.MoveLeft: return "왼쪽 이동";
            case InputType.MoveRight: return "오른쪽 이동";
            case InputType.MoveUp: return "위로 이동";
            case InputType.MoveDown: return "아래로 이동";
            case InputType.Jump: return "점프";
            case InputType.Attack: return "공격";
            case InputType.Smash: return "스매쉬";
            case InputType.Interact: return "상호작용";
            case InputType.SpecialAbility: return "특수능력";
            case InputType.Pause: return "일시정지";
            case InputType.Inventory: return "인벤토리";
            case InputType.Run: return "달리기";
            case InputType.Special_1: return "특수키";
            default: return inputType.ToString();
        }
    }
}