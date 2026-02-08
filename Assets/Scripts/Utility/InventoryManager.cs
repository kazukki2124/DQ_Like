using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // どこからでも呼べるようにstatic修飾子を付ける
    public static InventoryManager Instance;

    // Listという増減できる配列の宣言を行います
    private List<ItemData> items = new List<ItemData>();

    private void Awake()
    {
        Instance = this;
    }

    public void Add(ItemData item)
    {
        if(item == null)
        {
            return;
        }
        items.Add(item);
        Debug.Log($"[インベントリー]アイテム追加:{item.ItemName}");
    }

    /// <summary>
    /// 引数のアイテムを持っているかどうか
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Has(ItemData item)
    {
        return items.Contains(item);
    }

    /// <summary>
    /// ほかのclassからitemを見たいときに呼ぶ
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<ItemData> GetAll()
    {
        return items;
    }
}
