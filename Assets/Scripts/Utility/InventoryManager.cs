using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // どこからでも呼べるようにstatic修飾子を付ける
    public static InventoryManager Instance;

    // Listという増減できる配列の宣言を行います
    private List<InventryEntry> items = new List<InventryEntry>();
    
    // 装備品用のリストを追加
    private List<EquipmentEntry> equipments = new List<EquipmentEntry>();

    private void Awake()
    {
        // シーンをまたいで使えるように設定
        if (Instance == null)
        {
            Instance = this;
            // シーンの破棄に巻き込まれないようにする
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 管理するマネージャーが複数あったら困るので
            // 絶対に一つだけ存在するようにする
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// インベントリにアイテムを追加する
    /// </summary>
    public void Add(ItemData item, int amount)
    {
        if(item == null || amount <= 0)
        {
            return;
        }
        var entry = items.Find(x => x.Item == item);

        if(entry != null)
        {
            entry.Count += amount;
        }
        else
        {
            items.Add(new InventryEntry
            {
                Item = item,
                Count = amount
            });
        }
        Debug.Log($"[インベントリー]アイテム追加:{item.ItemName}");
    }

    /// <summary>
    /// インベントリに装備品を追加する
    /// </summary>
    public void Add(EquipmentData equipment, int amount)
    {
        if (equipment == null || amount <= 0)
        {
            return;
        }
        var entry = equipments.Find(x => x.Equipment == equipment);

        if (entry != null)
        {
            entry.Count += amount;
        }
        else
        {
            equipments.Add(new EquipmentEntry
            {
                Equipment = equipment,
                Count = amount
            });
        }
        Debug.Log($"[インベントリー]装備追加:{equipment.DisplayName}");
    }

    /// <summary>
    /// 引数のアイテムを持っているかどうか
    /// </summary>
    public bool Has(ItemData item)
    {
        var entry = items.Find(x => x.Item == item);
        return entry != null;
    }

    /// <summary>
    /// 引数の装備品を持っているかどうか
    /// </summary>
    public bool Has(EquipmentData equipment)
    {
        var entry = equipments.Find(x => x.Equipment == equipment);
        return entry != null;
    }

    /// <summary>
    /// アイテムの名称でitemDataを取得する
    /// </summary>
    public ItemData GetItemData(string itemName)
    {
        var entry = items.Find(x => x.Item.ItemName == itemName);
        if (entry == null) return null;
        return entry.Item;
    }

    /// <summary>
    /// ほかのclassからitemを見たいときに呼ぶ
    /// </summary>
    public IReadOnlyList<InventryEntry> GetAll()
    {
        return items;
    }

    /// <summary>
    /// ほかのclassから装備品を見たいときに呼ぶ
    /// </summary>
    public IReadOnlyList<EquipmentEntry> GetAllEquipments()
    {
        return equipments;
    }

    /// <summary>
    /// カテゴリ別にアイテムを取得する
    /// </summary>
    public List<InventryEntry> GetItemsByCategory(ItemData.ItemCategory category)
    {
        return items.FindAll(x => x.Item.Category == category);
    }

    /// <summary>
    /// アイテムを使用する
    /// </summary>
    public bool UseItem(ItemData item)
    {
        var entry = items.Find(x => x.Item == item);

        if (entry == null || entry.Count <= 0)
        {
            return false;
        }

        entry.Count--;
        return true;
    }

    public bool UseItem(string itemName)
    {
        var entry = items.Find(x => x.Item.ItemName == itemName);

        if (entry == null || entry.Count <= 0)
        {
            return false;
        }

        entry.Count--;
        return true;
    }

    public int Getcount(ItemData item)
    {
        var entry = items.Find(x => x.Item == item);
        if (entry == null) return 0;
        return entry.Count;
    }

    public int Getcount(EquipmentData equipment)
    {
        var entry = equipments.Find(x => x.Equipment == equipment);
        if (entry == null) return 0;
        return entry.Count;
    }
}
