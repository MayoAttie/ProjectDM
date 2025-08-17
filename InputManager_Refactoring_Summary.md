# InputManager Save System Refactoring Summary

## 개요
InputManager를 PlayerPrefs 대신 구현된 Save System을 사용하도록 리팩토링했습니다.

## 주요 변경사항

### 1. PlayerSaveData 구조 확장
- `InputSettingsData` 클래스 추가
- `PlayerSaveData`에 `InputSettings` 필드 추가
- 키 바인딩을 문자열 Dictionary로 저장

### 2. InputManager 리팩토링
- **기존**: PlayerPrefs를 사용한 개별 키 저장
- **변경**: Save System을 통한 통합 저장
- GameData 싱글톤과 연동
- 안전한 초기화 및 에러 처리 추가

### 3. GameData 통합
- InputManager 키 바인딩 새로고침 로직 추가
- 저장 데이터 로드 시 자동으로 키 바인딩 업데이트

## 새로운 기능

### 안전한 초기화
```csharp
// InputManager가 초기화되었는지 확인
bool isReady = InputManager.IsInitialized();

// 안전한 키 바인딩 가져오기 (초기화되지 않은 경우 기본값 반환)
KeyCode key = InputManager.GetKeyBindingSafe(InputType.Jump);
```

### 자동 저장
```csharp
// 키 바인딩 변경 시 자동으로 Save System에 저장
InputManager.SetKeyBinding(InputType.Jump, KeyCode.J);
// GameData.Instance.Save()가 자동으로 호출됨
```

### 키 바인딩 새로고침
```csharp
// GameData 로드 후 키 바인딩 새로고침
InputManager.RefreshKeyBindings();
```

## 사용법

### 기본 사용
```csharp
// 입력 체크 (기존과 동일)
if (InputManager.IsKeyPressed(InputType.Jump))
{
    // 점프 로직
}

// 키 바인딩 변경
InputManager.SetKeyBinding(InputType.Attack, KeyCode.X);

// 모든 키 바인딩 가져오기
var allBindings = InputManager.GetAllKeyBindings();
```

### 초기화 확인
```csharp
// InputManager가 준비되었는지 확인
if (InputManager.IsInitialized())
{
    // 안전하게 사용 가능
    KeyCode jumpKey = InputManager.GetKeyBinding(InputType.Jump);
}
```

## 장점

1. **통합된 저장 시스템**: 모든 게임 데이터가 하나의 파일에 저장
2. **버전 관리**: Save System의 버전 마이그레이션 활용 가능
3. **데이터 일관성**: 키 설정이 다른 게임 데이터와 함께 저장/로드
4. **에러 처리**: GameData가 초기화되지 않은 경우 안전한 폴백
5. **확장성**: 향후 추가 설정을 쉽게 통합 가능

## 테스트

`InputManagerTest.cs` 스크립트를 사용하여 기능을 테스트할 수 있습니다:

- **F1**: 키 바인딩 변경 테스트
- **F2**: 기본값 초기화 테스트
- **GUI**: 현재 상태 확인

## 주의사항

1. GameData가 초기화되기 전에 InputManager를 사용하면 기본값이 사용됩니다
2. 키 바인딩 변경 시 자동으로 저장되므로 별도의 저장 호출이 필요하지 않습니다
3. 기존 PlayerPrefs 데이터는 새로운 Save System으로 마이그레이션되지 않습니다

## 마이그레이션 가이드

기존 PlayerPrefs 데이터를 새로운 Save System으로 마이그레이션하려면:

```csharp
// GameData의 MigrateIfNeeded() 메서드에 추가
void MigrateIfNeeded()
{
    if (SaveData.SaveVersion < 2) // 새로운 버전
    {
        // PlayerPrefs에서 키 바인딩 로드
        foreach (var inputType in System.Enum.GetValues(typeof(InputType)))
        {
            string savedKey = PlayerPrefs.GetString($"KeyBinding_{inputType}", "");
            if (!string.IsNullOrEmpty(savedKey) && System.Enum.TryParse(savedKey, out KeyCode keyCode))
            {
                SaveData.InputSettings.KeyBindings[inputType.ToString()] = keyCode.ToString();
            }
        }
        SaveData.SaveVersion = 2;
    }
}
```
