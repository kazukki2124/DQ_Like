using System.Collections;
using UnityEngine;

public class ChestInteract : MonoBehaviour,IInteractable
{
    [Header("中身")]
    public ItemData RewardItem;

    [Header("演出")]
    public Animator ChestAnimator;
    public string OpenTriggerName = "Open";

    [Header("状態")]
    public bool IsOpened = false;

    [Header("鍵がないと開かない")]
    public bool IsLocked = false;

    [Header("当たり判定")]
    public Collider InteractCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (ChestAnimator == null)
        {
            ChestAnimator =
                this.transform.GetComponentInChildren<Animator>();
        }
        if(InteractCollider == null)
        {
            InteractCollider = GetComponent<Collider>();
        }
    }

    public void Interact()
    {
        // 既に空いている宝箱だったら何もしない
        if (IsOpened)
        {
            return;
        }
        // 施錠されていて
        if (IsLocked)
        {
            // 万能鍵が無かったら何もしない
            if (!QuestFlag.HasKey)
            {
                DialogUI.Instance.ShowSimpleMessage(RewardItem.ItemName);
                return;
            }
        }
        StartCoroutine(OpenRoutine());
    }

    private IEnumerator OpenRoutine()
    {
        IsOpened = true;
        // 設定されているitemDataが万能鍵だった場合
        if(RewardItem != null &&
           RewardItem.ItemName == "万能鍵")
        {
            QuestFlag.OpendChestB = true;
            QuestFlag.HasKey = true;
        }
        else
        {
            QuestFlag.OpendChestA = true;
        }
        

        // 連打防止：当たり判定を切る
        if(InteractCollider != null)
        {
            InteractCollider.enabled = false;
        }

        // アニメ再生
        if(ChestAnimator != null)
        {
            ChestAnimator.ResetTrigger(OpenTriggerName);
            ChestAnimator.SetTrigger(OpenTriggerName);
        }

        // 1秒待ちます
        yield return new WaitForSeconds(1f);

        // インベントリにこの宝箱に設定されているItemDataを追加
        if(InventoryManager.Instance != null)
        {
            InventoryManager.Instance.Add(RewardItem);
        }

        // ダイアログに表示を指示します
        if(DialogUI.Instance != null)
        {
            DialogUI.Instance.ShowItemDialog(RewardItem.ItemName);
        }
    }
}
