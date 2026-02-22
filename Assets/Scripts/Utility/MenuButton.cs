using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuButton : MonoBehaviour
{
    public Button Button;
    public TextMeshProUGUI Label;
    private System.Action onClick;

    /// <summary>
    /// BattleManagerからセットアップを行う
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
            // Buttonに設定されているクリックしたときの挙動を削除
            Button.onClick.RemoveAllListeners();
            // Script側からクリックしたときの挙動を設定する
            Button.onClick.AddListener(() => this.onClick?.Invoke());
        }
    }

}
