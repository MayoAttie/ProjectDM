using UnityEngine;

public class PlayerableCharacterManager : MonoSingleton<PlayerableCharacterManager>
{
    PlayerableCharacterController currentCharacter;
    public event System.Action<PlayerableCharacterController> OnCharacterChanged;

    public void OnRegisterCharacter(PlayerableCharacterController character)
    {
        if (currentCharacter == character)
            return;
        currentCharacter = character; 
        OnCharacterChanged?.Invoke(currentCharacter);
    }
    public void OnUnRegisterCharacter(PlayerableCharacterController character)
    {
        if(currentCharacter == character)
        {
            currentCharacter = null;
            OnCharacterChanged?.Invoke(null);
        }
    }
    
}
