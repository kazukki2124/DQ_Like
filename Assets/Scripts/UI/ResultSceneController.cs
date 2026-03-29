using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultSceneController : MonoBehaviour
{
    /// <summary>
    /// RetryButtonが押されたときに呼ばれるメソッド
    /// Field_01シーンをロードします
    /// </summary>
    public void OnRetryButtonClicked()
    {
        Debug.Log("Retry Button Clicked. Loading Field_01...");
        SceneManager.LoadScene("Field_01");
    }

    /// <summary>
    /// ExitButtonが押されたときに呼ばれるメソッド
    /// ゲームを終了します
    /// </summary>
    public void OnExitButtonClicked()
    {
        Debug.Log("Exit Button Clicked. Exiting game...");
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
