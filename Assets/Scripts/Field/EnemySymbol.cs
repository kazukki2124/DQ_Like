using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySymbol : MonoBehaviour
{
    private string BattleSceneName = "BattleScene";

    /// <summary>
    /// N“ü”»’è‚ÅPlayer‚ª“ü‚Á‚Ä‚«‚½‚Æ‚«‚Éˆ—‚ğs‚¤
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        // Player‚ÌTagˆÈŠO‚ÌGameObject‚ªN“ü‚µ‚Ä‚«‚½‚ç‰½‚à‚µ‚È‚¢
        if (!other.CompareTag("Player"))
        {
            return;
        }
        SceneManager.LoadScene(BattleSceneName);
    }
}
