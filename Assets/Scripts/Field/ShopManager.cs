using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("このお店で売っているアイテム")]
    public List<ItemData> ShopItem;

    public void BuyItem(ItemData itemToBuy)
    {
        if(itemToBuy == null)
        {
            return;
        }

        if (PlayerState.Instance.ConsumeGold(itemToBuy.Price))
        {
            // 支払いに成功したらインベントリマネージャーにアイテムを追加する
            InventoryManager.Instance.Add(itemToBuy, 1);
            DialogUI.Instance.ShowSimpleMessage($"{itemToBuy.ItemName} を 1こ　かった");
        }
        else
        {
            DialogUI.Instance.ShowSimpleMessage("おかねが　たりません");
        }
        // 何かインタラクトしたら、ShopのCanvasを閉じる
        this.gameObject.SetActive(false);
    }
}
