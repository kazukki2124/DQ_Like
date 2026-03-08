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
