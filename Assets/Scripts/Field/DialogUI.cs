using UnityEngine;
using TMPro;
using System.Collections;

public class DialogUI : MonoBehaviour
{
    /// <summary>
    /// どこからでもDialogUIにアクセスできるように
    /// staticで宣言する
    /// </summary>
    public static DialogUI Instance;

    public GameObject Panel;

    public TextMeshProUGUI NameText;

    public TextMeshProUGUI MessageText;

    [Header("▼などのNextHint (Optional)")]
    public GameObject NextHint;

    /// <summary>
    /// はい、いいえボタンの変数
    /// </summary>
    public GameObject YesNoButtonBG;

    /// <summary>
    /// 1文字を待つ時間
    /// </summary>
    private float charInterval = 0.15f;

    /// <summary>
    /// メッセージの現在の行
    /// </summary>
    private string[] currentLines;

    /// <summary>
    /// 何行目か
    /// </summary>
    private int lineIndex;

    private Coroutine typingCoroutine;

    /// <summary>
    /// 文字送り中か
    /// </summary>
    private bool isTyping = false;

    private void Awake()
    {
        Instance = this;
        // ゲーム開始時はダイアログは表示しない
        Panel.SetActive(false);
        if (NextHint != null)
        {
            NextHint.SetActive(false);
        }
    }
    /// <summary>
    /// 外部からダイアログの設定をして見せる
    /// </summary>
    /// <param name="name">名前</param>
    /// <param name="message">本文</param>
    public void Show(DialogData dialogData)
    {
        // DialogDataのデータが正常に動かない場合
        if (dialogData == null ||
            dialogData.MessageLines == null ||
            dialogData.MessageLines.Length == 0)
        {
            Debug.LogWarning("DialogDataが不正です");
            return;
        }

        // boolの値を直接GameObjectのActivの値に変更する
        YesNoButtonBG.SetActive(dialogData.ShowYesNo);

        GameState.IsDialogOpen = true;

        NameText.text = dialogData.Speaker;

        currentLines = dialogData.MessageLines;
        lineIndex = 0;

        Panel.SetActive(true);
        // 現在の行を表示していく
        ShowLine(lineIndex);
    }

    /// <summary>
    /// UIのCloseボタンから呼びます
    /// </summary>
    public void Close()
    {
        // まず文字送りを止めて
        StopTypingIfNeeded();

        GameState.IsDialogOpen = false;
        Panel.SetActive(false);
        if (NextHint != null)
        {
            NextHint.SetActive(false);
        }
        currentLines = null;
        lineIndex = 0;
    }

    /// <summary>
    /// 会話中の次への入力
    /// </summary>
    public void Next()
    {
        // パネルが有効じゃなかったら
        if (!Panel.activeSelf)
        {
            return;
        }
        // 文字送り中なら
        if (isTyping)
        {
            // 即表示
            FinishCurrentLineInstant();
            return;
        }
        // 次の行へ
        lineIndex++; // インクリメント(n + 1すること)

        if (currentLines != null &&
            lineIndex < currentLines.Length)
        {
            ShowLine(lineIndex);
        }
        else
        {
            Close();
        }
    }

    /// <summary>
    /// index行を表示する
    /// </summary>
    /// <param name="index"></param>
    private void ShowLine(int index)
    {
        // まず文字送りを止めて
        StopTypingIfNeeded();
        // いま表示されているものを空にします
        MessageText.text = string.Empty;
        // ▼などの送り表示のオブジェクトがあれば表示します
        if(NextHint != null)
        {
            NextHint.SetActive(true);
        }

        typingCoroutine =
            StartCoroutine(TypeLine(currentLines[index]));
    }
    /// <summary>
    /// 文字送り
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        // 行の中の1文字を取り出していく
        foreach (char c in line)
        {
            MessageText.text += c;
            yield return new WaitForSeconds(charInterval);
        }
        isTyping = false;

        if(NextHint != null)
        {
            NextHint.SetActive(true);
        }
        // 自動で次の行へ行くように
        //Next();
        typingCoroutine = null;
    }

    private void FinishCurrentLineInstant()
    {
        if (currentLines == null)
        {
            return;
        }
        // まず文字送りをとめて
        StopTypingIfNeeded();
        // 現在の行を全て表示
        MessageText.text = currentLines[lineIndex];
        // 文字送り中じゃなくす
        isTyping = false;
        if (NextHint != null)
        {
            NextHint.SetActive(true);
        }
    }

    /// <summary>
    /// 必要だったら文字送りを止める
    /// </summary>
    private void StopTypingIfNeeded()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        isTyping = false;
    }
    public void OnYes()
    {
        Close();
    }

    public void OnNo()
    {
        Close();
    }
}
