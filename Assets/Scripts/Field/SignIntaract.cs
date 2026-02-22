using UnityEngine;

public class SignIntaract : MonoBehaviour, IInteractable
{
    [TextArea]
    public string Message = "\u306F\u3058\u307E\u308A\u306E\u3000\u3080\u3089\u3000\u30D5\u30A1\u30B9\u30BF\u30FC";

    public void Interact()
    {
        // DialogData created at runtime
        DialogData dialogData = ScriptableObject.CreateInstance<DialogData>();
        dialogData.Speaker = ""; // No speaker name for sign
        dialogData.MessageLines = new string[] { Message };
        
        DialogUI.Instance.Show(dialogData);
    }
}
