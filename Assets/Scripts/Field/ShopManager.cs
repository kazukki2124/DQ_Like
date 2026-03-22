using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("この店で売っているアイテム")]
    public List<ItemData> ShopItem;
    public List<EquipmentData> ShopEquipment;

    public void BuyItem(ItemData itemToBuy)
    {
        if(itemToBuy == null)
        {
            return;
        }

        // お金が足りるかチェック
        if (PlayerState.Instance.ConsumeGold(itemToBuy.Price))
        {
            // インベントリマネージャーにアイテムを追加
            InventoryManager.Instance.Add(itemToBuy, 1);
            DialogUI.Instance.ShowSimpleMessage($"{itemToBuy.ItemName} を 1個買った");
        }
        else
        {
            DialogUI.Instance.ShowSimpleMessage("お金が足りません");
        }
        
        // 購入後、ShopのCanvasを閉じる
        this.gameObject.SetActive(false);
    }

    public void BuyEquipment(EquipmentData equipmentToBuy)
    {
        if(equipmentToBuy == null)
        {
            return;
        }

        // お金が足りるかチェック
        if (PlayerState.Instance.ConsumeGold(equipmentToBuy.Price))
        {
            // インベントリマネージャーに装備を追加
            InventoryManager.Instance.Add(equipmentToBuy, 1);
            DialogUI.Instance.ShowSimpleMessage($"{equipmentToBuy.DisplayName} を 1つ買った \n{equipmentToBuy.DisplayName}をそうびした");
        }
        else
        {
            DialogUI.Instance.ShowSimpleMessage("お金が足りません");
        }
        
        // 購入後、ShopのCanvasを閉じる
        this.gameObject.SetActive(false);
    }
}
