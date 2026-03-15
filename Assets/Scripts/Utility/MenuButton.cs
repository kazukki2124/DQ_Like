using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuButton : MonoBehaviour
{
    public Button Button;
    public TextMeshProUGUI Label;
    private System.Action onClick;

    /// <summary>
    /// セットアップ
    /// </summary>
    /// <param name="label"></param>
    /// <param name="onClick"></param>
    public void Setup(string label,System.Action onClick)
    {
        // 最低限のサイズを保証する（防衛的実装）
        LayoutElement layout = GetComponent<LayoutElement>();
        if (layout == null)
        {
            layout = gameObject.AddComponent<LayoutElement>();
        }
        layout.minWidth = 200f;
        layout.minHeight = 60f;

        this.onClick = onClick;
        if(label != null)
        {
            Label.text = label;
        }
        if(Button != null)
        {
            Button.onClick.RemoveAllListeners();
            Button.onClick.AddListener(() => this.onClick?.Invoke());
        }
    }

}
