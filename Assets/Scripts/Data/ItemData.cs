using UnityEngine;

[CreateAssetMenu(menuName = "DQ-Like/Items/ItemData",
    fileName = "item_")]
public class ItemData : ScriptableObject
{
    public string ItemName;

    [TextArea(2,4)]
    public string Description;

    // 任意ですが、UIで使うアイコン
    public Sprite Icon;
}
