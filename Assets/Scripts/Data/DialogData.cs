using UnityEngine;

[CreateAssetMenu(menuName = "DQ-Like/Dialog/DialogData",
    fileName = "DialogData_")]
public class DialogData : ScriptableObject
{
    /// <summary>
    /// 誰が言ったか(NameTextに反映させる)
    /// </summary>
    public string Speaker;

    /// <summary>
    /// ダイアログに表示するテキスト
    /// </summary>
    [TextArea(2, 4)]
    public string[] MessageLines;

    /// <summary>
    /// 選択肢を出すかどうかのフラグ
    /// </summary>
    public bool ShowYesNo = false;

    /// <summary>
    /// イエスを押したときのゲーム進行上のフラグ
    /// </summary>
    public string SetFlagOnYes;

}
