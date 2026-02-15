using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public static int NextEnemyID = 0;

    [Header("EnemyData")]
    public EnemyDatabase EnemyDB;
    private EnemyData currentEnemy;

    [Header("PlayerData")]
    public float PlayerMaxHP = 30f;
    public float PlayerHP = 30;
    public float PlayerAttackMin = 5;
    public float PlayerAttackMax = 10;

    [Header("Enemy HP")]
    public float EnemyHP;

    [Header("UI")]
    public TextMeshProUGUI PlayerHPText;
    public TextMeshProUGUI EnemyNameText;
    public TextMeshProUGUI EnemyHPText;
    public TextMeshProUGUI DialogText;

    private bool isPlayerTurn = true;


    void Start()
    {
        SetupEnemyFromDB();
        UpdateUI();
        DialogText.text =
            $"{currentEnemy.DisplayName}が現れた！";
    }

    private void SetupEnemyFromDB()
    {
        if (EnemyDB == null)
        {
            Debug.LogError("EnemyDBが設定されていません");
            return;
        }

        currentEnemy = EnemyDB.GetByID(NextEnemyID);

        if(currentEnemy == null)
        {
            Debug.LogError("NextEnemyIDがEnemyDBに見つかりません");
            return;
        }

        EnemyHP = currentEnemy.MaxHP;
    }

    /// <summary>
    /// UnityEditor上のAttackButtonのOnClickに設定
    /// </summary>
    public void OnAttackButton()
    {
        // プレイヤーのターンじゃなかったら何もしません
        if (!isPlayerTurn)
        {
            return;
        }
        StartCoroutine(PlayerAttack());
    }

    private System.Collections.IEnumerator PlayerAttack()
    {
        isPlayerTurn = false;

        DialogText.text = "プレイヤーの攻撃！";

        // 1秒待つ
        yield return new WaitForSeconds(1f);
        // ダメージ計算で小数点切り上げ
        var damage 
            = Mathf.Ceil(
                Random.Range(PlayerAttackMin, PlayerAttackMax)
                );
        EnemyHP -= damage;

        if (EnemyHP <= 0)
        {
            EnemyHP = 0;
        }

        DialogText.text = $"{damage} ダメージ！";

        UpdateUI();

        yield return new WaitForSeconds(1f) ;

        // EnemyHPを削り切れたら
        if (EnemyHP <= 0f)
        {
            // 勝利
            Victory();
        }
        else
        { // そうじゃなかったら銭湯続行
            StartCoroutine(EnemyTurn());
        }
    }

    private System.Collections.IEnumerator EnemyTurn()
    {
        DialogText.text = $"{currentEnemy.DisplayName}の攻撃！";

        yield return new WaitForSeconds(1f);

        // 小数点切り上げで敵からのダメージを計算する
        var damage = Mathf.Ceil(
                Random.Range(currentEnemy.AttackMin,
                currentEnemy.AttackMax)
                );

        PlayerHP -= damage;

        if(PlayerHP <= 0)
        {
            PlayerHP = 0;
        }

        DialogText.text = $"{damage} ダメージ！";

        UpdateUI();

        yield return new WaitForSeconds(1f);

        if (PlayerHP <= 0f)
        {
            // 敗北
            GameOver();
        }
        else
        {
            isPlayerTurn = true;
            DialogText.text = "どうする？";
        }
    }

    /// <summary>
    /// HPなどのUIの更新
    /// </summary>
    private void UpdateUI()
    {
        PlayerHPText.text = $"HP:{PlayerHP}/{PlayerMaxHP}";
        if(currentEnemy != null)
        {
            EnemyNameText.text = currentEnemy.DisplayName;
            EnemyHPText.text =
                $"HP:{EnemyHP}/{currentEnemy.MaxHP}";
        }
        else
        {
            EnemyNameText.text = "Enemy";
            EnemyHPText.text = $"HP:{EnemyHP}";
        }
    }

    private void Victory()
    {
        DialogText.text = "勝利！";
        Invoke(nameof(ReturnToField), 2f);
    }
    private void GameOver()
    {
        DialogText.text = "全滅した……";
        Invoke(nameof(ReturnToField), 2f);
    }
    private void ReturnToField()
    {
        SceneManager.LoadScene("Field_01");
    }
}
