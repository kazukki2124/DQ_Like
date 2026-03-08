using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("UI要素")]
    public GameObject InventoryPanel;
    public Transform ItemListRoot;
    public GameObject ItemButtonPrefab;

    [Header("タブボタン")]
    public Button ConsumableTabButton;
    public Button ImportantTabButton;

    private ItemData.ItemCategory currentCategory = ItemData.ItemCategory.Consumable;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // すでに存在する場合は、新しく作られた方を削除する
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 初期状態では閉じている
        if (InventoryPanel != null)
        {
            InventoryPanel.SetActive(false);
        }

        // タブボタンのイベント登録
        if (ConsumableTabButton != null)
        {
            ConsumableTabButton.onClick.AddListener(() => SwitchCategory(ItemData.ItemCategory.Consumable));
        }
        if (ImportantTabButton != null)
        {
            ImportantTabButton.onClick.AddListener(() => SwitchCategory(ItemData.ItemCategory.Important));
        }
    }

    private void Update()
    {
        // Iキーでインベントリを開閉
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            if (InventoryPanel.activeSelf)
            {
                Close();
            }
            else
            {
                Open();
            }
        }
    }

    public void Open()
    {
        InventoryPanel.SetActive(true);
        GameState.IsDialogOpen = true; // 他の入力を制限
        Refresh();
    }

    public void Close()
    {
        InventoryPanel.SetActive(false);
        GameState.IsDialogOpen = false;
    }

    public void SwitchCategory(ItemData.ItemCategory category)
    {
        currentCategory = category;
        Refresh();
    }

    public void Refresh()
    {
        // リストをクリア
        foreach (Transform child in ItemListRoot)
        {
            Destroy(child.gameObject);
        }

        // 指定カテゴリのアイテムを取得
        var entries = InventoryManager.Instance.GetItemsByCategory(currentCategory);

        // ボタンを生成
        foreach (var entry in entries)
        {
            GameObject obj = Instantiate(ItemButtonPrefab, ItemListRoot);
            MenuButton menuButton = obj.GetComponent<MenuButton>();
            
            // 表示名に個数を追加: "ポーション x3" のような形式
            string label = $"{entry.Item.ItemName} x{entry.Count}";
            
            menuButton.Setup(label, () => {
                // ここでアイテム使用や説明表示などの処理を拡張可能
                Debug.Log($"{entry.Item.ItemName} を選択しました");
            });
        }
    }
}
