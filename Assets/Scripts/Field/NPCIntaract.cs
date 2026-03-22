using UnityEngine;
using UnityEngine.Events;

public class NPCIntaract : MonoBehaviour, IInteractable
{
    public enum NPCType
    {
        Invalid = -1,
        NPC,
        Shop
    }

    public NPCType Type = NPCType.NPC;

    public DialogData FirstDialogData;
    public DialogData AfterDialogData;

    // 鍵を持っていた場合のダイアログデータ
    public DialogData HasKeyDialogData;

    /// <summary>
    /// UnityEventは処理をUnityEditorから設定できます
    /// </summary>
    public UnityEvent NPCEvent;


    public UnityEvent NPCShopEvent;

    public void Interact()
    {
        // 既にダイアログ表示中なら「次へ進む」
        if(DialogUI.Instance != null &&
            DialogUI.Instance.TryNextIfOpen())
        {
            return;
        }

        // 商人だったら、ダイアログと同時に、ShopCamvasを出す
        if(Type == NPCType.Shop)
        {
            // ダイアログの表示を行います
            DialogUI.Instance.Show(FirstDialogData);
            NPCShopEvent?.Invoke();
            return;
        }

        // 鍵を持って話しかけた場合
        if (QuestFlag.HasKey)
        {
            DialogUI.Instance.Show(HasKeyDialogData);
        }
        else if (!QuestFlag.TalkedVillager)
        {
            // ダイアログの表示を行います
            DialogUI.Instance.Show(FirstDialogData);
            QuestFlag.TalkedVillager = true;
        }
        else // 村人に過去に話しかけていた場合
        {
            DialogUI.Instance.Show(AfterDialogData);
        }
        // 鍵を持っていなかったら、NPCEventは発生させない
        if (!QuestFlag.HasKey)
        {
            return;
        }
        // NPCEventが設定されていれば(Nullじゃなかったら)、
        // 設定された処理を発動する
        NPCEvent?.Invoke();
    }
}
