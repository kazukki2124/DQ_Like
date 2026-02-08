using UnityEngine;

public class SignIntaract : MonoBehaviour, IInteractable
{
    [TextArea]
    public string Message = "はじまりの　むら　ファスター";

    public void Interact()
    {
        Debug.Log($"[Sign]{Message}");
    }
}
