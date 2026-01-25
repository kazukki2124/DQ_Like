using UnityEngine;

public class NPCIntaract : MonoBehaviour, IInteractable
{
    public string NPCName = "ë∫êlA";

    [TextArea]
    public string TalkMessage = "Ç±ÇÒÇ…ÇøÇÕ";

    public void Interact()
    {
        Debug.Log($"[NPC]{TalkMessage} éÑÇÕ{NPCName}Ç≈Ç∑");
    }
}
