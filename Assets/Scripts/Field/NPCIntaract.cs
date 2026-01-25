using UnityEngine;
using UnityEngine.Events;

public class NPCIntaract : MonoBehaviour, IInteractable
{
    public string NPCName = "‘ºlA";

    [TextArea]
    public string TalkMessage = "‚±‚ñ‚É‚¿‚Í";

    /// <summary>
    /// UnityEvent‚Íˆ—‚ğUnityEditor‚©‚çİ’è‚Å‚«‚Ü‚·
    /// </summary>
    public UnityEvent NPCEvent;

    public void Interact()
    {
        Debug.Log($"[NPC]{TalkMessage} „‚Í{NPCName}‚Å‚·");
        // NPCEvent‚ªİ’è‚³‚ê‚Ä‚¢‚ê‚Î(Null‚¶‚á‚È‚©‚Á‚½‚ç)A
        // İ’è‚³‚ê‚½ˆ—‚ğ”­“®‚·‚é
        NPCEvent?.Invoke();
    }
}
