using UnityEngine;
using UnityEngine.Events;

public class NPCIntaract : MonoBehaviour, IInteractable
{
    public DialogData FirstDialogData;
    public DialogData AfterDialogData;

    /// <summary>
    /// UnityEventは処理をUnityEditorから設定できます
    /// </summary>
    public UnityEvent NPCEvent;

    public void Interact()
    {
        // まだ村人に話しかけていない場合
        if (!QuestFlag.TalkedVillager)
        {
            // ダイアログの表示を行います
            DialogUI.Instance.Show(FirstDialogData);
            QuestFlag.TalkedVillager = true;
        }
        else // 村人に過去に話しかけていた場合
        {
            DialogUI.Instance.Show(AfterDialogData);
        }
        // NPCEventが設定されていれば(Nullじゃなかったら)、
        // 設定された処理を発動する
        NPCEvent?.Invoke();
    }
}
