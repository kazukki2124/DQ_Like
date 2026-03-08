using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // どこからでも呼べるようにstatic修飾子を付ける
    public static InventoryManager Instance;

    // Listという増減できる配列の宣言を行います
    private List<InventryEntry> items =
        new List<InventryEntry>();

    private void Awake()
    {
        // シーンをまたいで使えるように設定
        if (Instance == null)
        {
            Instance = this;
            // シーンの破棄に巻き込まれないようにする
            // 設定の書き方
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 管理するマネージャーが複数いたら困るので
            // 絶対に一つだけ存在するようにする
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// インベントリにアイテムを追加する
    /// </summary>
    /// <param name="item"></param>
    public void Add(ItemData item, int amount)
    {
        if(item == null || amount <= 0)
        {
            return;
        }
        // 追加されたアイテムを、現在持っているアイテムの
        // 中から調べる
        var entry = items.Find(
            x => x.Item = item);

        // すでに取得しているアイテムだった場合
        if(entry != null)
        {
            // amountで設定されている個数を追加します
            entry.Count += amount;
        }
        else
        {

            // アイテムデータをインベントリーに追加
            items.Add(new InventryEntry
            {
                Item = item,
                Count = amount
            });
        }

        Debug.Log($"[インベントリー]アイテム追加:{item.ItemName}");
    }

    /// <summary>
    /// 引数のアイテムを持っているかどうか
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Has(ItemData item)
    {
        // 引数のアイテムを、現在持っているアイテムの
        // 中から調べる
        var entry = items.Find(
            x => x.Item = item);
        return entry != null;
    }

    /// <summary>
    /// アイテムの名称でitemDataを取得する
    /// </summary>
    /// <param name="itemName"></param>
    /// <returns></returns>
    public ItemData GetItemData(string itemName)
    {
        var entry = items.Find(
            x => x.Item.ItemName == itemName);
        return entry.Item;
    }

    /// <summary>
    /// ほかのclassからitemを見たいときに呼ぶ
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<InventryEntry> GetAll()
    {
        return items;
    }

    /// <summary>
    /// アイテムを使用する
    /// </summary>
    /// <returns></returns>
    public bool UseItem(ItemData item)
    {
        var entry = items.Find(
            x => x.Item = item);

        // アイテムをそもそも未所持か、
        // 過去に持っていたとしても個数が0以下
        if (entry == null || entry.Count <= 0)
        {
            return false;
        }

        entry.Count--;

        return true;
    }

    public bool UseItem(string itemName)
    {
        var entry = items.Find(
            x => x.Item.ItemName == itemName);

        // アイテムをそもそも未所持か、
        // 過去に持っていたとしても個数が0以下
        if (entry == null || entry.Count <= 0)
        {
            return false;
        }

        entry.Count--;

        return true;
    }

    public int Getcount(ItemData item)
    {
        var entry = items.Find(
            x => x.Item = item);
        if (entry == null)
        {
            return 0;
        }
        return entry.Count;
    }
}
