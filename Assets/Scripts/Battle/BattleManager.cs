using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public enum BattleMenuState
{
    Root,   // たたかう/さくせん/にげる
    Fight,  // こうげき/じゅもん/とくぎ/ぼうぎょ
    Busy    // 演出中(入力不可)
}

public class BattleManager : MonoBehaviour
{
    public static int NextEnemyID = 0;

    [Header("EnemyData")]
    public EnemyDatabase EnemyDB;
    private EnemyData currentEnemy;

    [Header("Enemy Visual")]
    public Transform EnemyModelRoot;
    private GameObject enemyModelInstance;
    private Animator enemyAnimator;

    [Header("PlayerStatusとLevelSystemの参照")]
    public PlayerStatus PlayerStatus;
    public LevelSystem LevelSystem;

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

    [Header("DQ Like Menu")]
    public GameObject RootMenuPanel;
    public Transform RootMenuRoot;
    public GameObject FightMenuPanel;
    public Transform FightMenuRoot;

    public MenuButton MenuButtonPrefab;

    private BattleMenuState menuState =
        BattleMenuState.Root;
    private bool isGuading = false;

    private bool isPlayerTurn = true;


    void Start()
    {
        SetupEnemyFromDB();
        ApplyPlayerStatus();

        UpdateUI();

        BuildRootMenu();

        // 戦闘の開始時に呼び出す
        SpawnEnemy();

        DialogText.text =
            $"{currentEnemy.DisplayName}が現れた！";
    }

    // データからプレイヤーの値を反映する
    public void ApplyPlayerStatus()
    {
        if(PlayerStatus == null)
        {
            return;
        }
        PlayerMaxHP = PlayerStatus.MaxHP;
        PlayerHP = Mathf.Min(PlayerHP, PlayerMaxHP);
        PlayerAttackMin = PlayerStatus.AttackMin;
        PlayerAttackMax = PlayerStatus.AttackMax;
    }

    private void SetupEnemyFromDB()
    {
        if (EnemyDB == null)
        {
            Debug.LogError("EnemyDBが設定されていません");
            return;
        }

        currentEnemy = EnemyDB.GetByID(NextEnemyID);

        if (currentEnemy == null)
        {
            Debug.LogError("NextEnemyIDがEnemyDBに見つかりません");
            return;
        }

        EnemyHP = currentEnemy.MaxHP;
    }

    /// <summary>
    /// 敵のVisualを生成
    /// </summary>
    private void SpawnEnemy()
    {
        if (EnemyModelRoot == null)
        {
            return;
        }
        if (currentEnemy == null)
        {
            return;
        }
        if (currentEnemy.ModelPrefab == null)
        {
            return;
        }
        // ゲーム開始時に既に敵のモデルがあった場合、削除
        if (enemyModelInstance != null)
        {
            Destroy(enemyModelInstance);
        }
        // Instantiateを使って、敵のモデルを、EnemyModelRootに生成
        enemyModelInstance = Instantiate(currentEnemy.ModelPrefab,
            EnemyModelRoot);
        // 敵のAnimatorを取得
        enemyAnimator = enemyModelInstance.GetComponentInChildren<Animator>();

        // 敵の位置情報を設定
        enemyModelInstance.transform.localPosition =
            currentEnemy.ModelPosition;
        enemyModelInstance.transform.localEulerAngles =
            currentEnemy.ModelRotation;
        enemyModelInstance.transform.localScale =
            currentEnemy.ModelScale;
    }

    private void SetMenuState(BattleMenuState state)
    {
        menuState = state;
        // Rootのメニューパネルの表示
        if (RootMenuPanel != null)
        {
            RootMenuPanel.SetActive(
                state == BattleMenuState.Root);
        }
        // 戦闘パネルの表示
        if (FightMenuPanel != null)
        {
            FightMenuPanel.SetActive(
                state == BattleMenuState.Fight);
        }
        if (state == BattleMenuState.Busy)
        {
            if (RootMenuPanel != null)
            {
                RootMenuPanel.SetActive(false);
            }
            if (FightMenuPanel != null)
            {
                FightMenuPanel.SetActive(false);
            }
        }
    }

    private void BuildRootMenu()
    {
        // RootMenuの子の階層にいるgameObjectを削除します
        ClearChildren(RootMenuRoot);

        CreateButton(RootMenuRoot, "たたかう", () =>
        {
            if (!isPlayerTurn)
            {
                return;
            }
            // たたかうメニューを設定します
            BuildFightMenu();
            // Todo:ここに後で設定用のメソッドを追記する
            SetMenuState(BattleMenuState.Fight);
            DialogText.text = "どうする？";
        });

        CreateButton(RootMenuRoot, "さくせん", () =>
          {
              if (!isPlayerTurn)
              {
                  return;
              }
              DialogText.text = "さくせんは　まだ　つかえない！";
          });

        CreateButton(RootMenuRoot, "にげる", () =>
        {
            if (!isPlayerTurn)
            {
                return;
            }
            StartCoroutine(TryEscape());
        });
    }
    private void BuildFightMenu()
    {
        ClearChildren(FightMenuRoot);

        CreateButton(FightMenuRoot, "こうげき", () =>
        {
            if (!isPlayerTurn)
            {
                return;
            }
            StartCoroutine(ExecuteAttack());
        });
        CreateButton(FightMenuRoot, "じゅもん", () =>
        {
            if (!isPlayerTurn)
            {
                return;
            }
            StartCoroutine(ExecuteHealSpell());
        });
        CreateButton(FightMenuRoot, "とくぎ", () =>
        {
            if (!isPlayerTurn)
            {
                return;
            }
            StartCoroutine(ExecutePowerSkill());
        });
        CreateButton(FightMenuRoot, "ぼうぎょ", () =>
        {
            if (!isPlayerTurn)
            {
                return;
            }
            StartCoroutine(ExecuteGuard());
        });
        CreateButton(FightMenuRoot, "もどる", () =>
        {
            SetMenuState(BattleMenuState.Root);
            DialogText.text = "どうする？";
        });
    }

    // こうげきの処理
    private System.Collections.IEnumerator ExecuteAttack()
    {
        isPlayerTurn = false;
        SetMenuState(BattleMenuState.Busy);

        DialogText.text = "こうげき";
        yield return new WaitForSeconds(0.5f);
        // 小数点切り上げで敵からのダメージを計算する
        var damage =
            Mathf.Ceil(
                Random.Range(PlayerAttackMin, PlayerAttackMax)
                );
        EnemyHP -= damage;
        if (EnemyHP <= 0)
        {
            EnemyHP = 0;
        }
        DialogText.text = $"{damage} ダメージ！";
        UpdateUI();
        yield return new WaitForSeconds(0.8f);
        if (EnemyHP <= 0f)
        {
            Victory();
            yield break;
        }
        else
        {
            StartCoroutine(EnemyTurn());
        }
    }

    // じゅもんの処理
    private System.Collections.IEnumerator ExecuteHealSpell()
    {
        isPlayerTurn = false;
        SetMenuState(BattleMenuState.Busy);
        DialogText.text = "キュア！";
        yield return new WaitForSeconds(0.6f);

        // 回復の値を計算
        float heal = Mathf.CeilToInt(PlayerMaxHP * 0.25f) + 2;
        // Mathf.Min(A,B)でどちらが小さい方の値を取得できる
        PlayerHP = Mathf.Min(PlayerMaxHP, PlayerHP + heal);

        DialogText.text = $"{heal} かいふく！";
        UpdateUI();
        yield return new WaitForSeconds(0.8f);
        StartCoroutine(EnemyTurn());
    }

    // とくぎの処理
    private System.Collections.IEnumerator ExecutePowerSkill()
    {
        isPlayerTurn = false;
        SetMenuState(BattleMenuState.Busy);
        DialogText.text = "つよく　きりつけた!";
        yield return new WaitForSeconds(0.6f);
        // 小数点切り上げで敵からのダメージを計算する
        var damage =
            Mathf.Ceil(
                Random.Range(PlayerAttackMin, PlayerAttackMax) * 1.6f + 2
                );
        EnemyHP -= damage;
        if (EnemyHP <= 0)
        {
            EnemyHP = 0;
        }
        DialogText.text = $"{damage} ダメージ！";
        UpdateUI();
        yield return new WaitForSeconds(0.8f);
        if (EnemyHP <= 0f)
        {
            Victory();
            yield break;
        }
        else
        {
            StartCoroutine(EnemyTurn());
        }

    }

    // ぼうぎょの処理
    private System.Collections.IEnumerator ExecuteGuard()
    {
        isPlayerTurn = false;
        SetMenuState(BattleMenuState.Busy);

        // 防御用のフラグを立てる
        isGuading = true;
        DialogText.text = "みを　まもっている！";
        yield return new WaitForSeconds(0.8f);
        StartCoroutine(EnemyTurn());
    }

    private System.Collections.IEnumerator TryEscape()
    {
        // Random.valueは0～1の間の値をランダムに返してくれます
        bool success = Random.value < 0.5f;
        if (success)
        {
            // 逃亡成功
            DialogText.text = "うまく　にげきれた！";
            Invoke(nameof(ReturnToField), 1.2f);
        }
        else
        {
            DialogText.text = "まわりこまれてしまった！";
            yield return new WaitForSeconds(0.8f);
            isPlayerTurn = false;
            SetMenuState(BattleMenuState.Busy);
            StartCoroutine(EnemyTurn());
        }
    }

    /// <summary>
    /// Buttonを生成
    /// </summary>
    void CreateButton(Transform root, string label,
        System.Action onClick)
    {
        if (MenuButtonPrefab == null || root == null)
        {
            return;
        }
        var btn = Instantiate(MenuButtonPrefab, root);
        btn.Setup(label, onClick);
    }

    void ClearChildren(Transform root)
    {
        if (root == null)
        {
            return;
        }
        for (int i = root.childCount - 1; i >= 0; i--)
        {
            Destroy(root.GetChild(i).gameObject);
        }
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

        yield return new WaitForSeconds(1f);

        // EnemyHPを削り切れたら
        if (EnemyHP <= 0f)
        {
            // 勝利
            Victory();
        }
        else
        { // そうじゃなかったら戦闘続行
            StartCoroutine(EnemyTurn());
        }
    }

    private System.Collections.IEnumerator EnemyTurn()
    {
        DialogText.text = $"{currentEnemy.DisplayName}の攻撃！";

        // Attackアニメーションを再生
        if (enemyAnimator != null)
        {
            enemyAnimator.SetTrigger("Attack");
        }

        yield return new WaitForSeconds(1f);

        // 小数点切り上げで敵からのダメージを計算する
        var damage = Mathf.Ceil(
                Random.Range(currentEnemy.AttackMin,
                currentEnemy.AttackMax)
                );

        // Playerが防御中
        if (isGuading)
        {
            damage = Mathf.Ceil(damage * 0.5f);
            isGuading = false;
        }

        PlayerHP -= damage;

        DialogText.text = $"{damage} ダメージ！";

        if (PlayerHP <= 0)
        {
            PlayerHP = 0;
        }

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
            SetMenuState(BattleMenuState.Root);
            DialogText.text = "どうする？";
        }
    }

    /// <summary>
    /// HPなどのUIの更新
    /// </summary>
    public void UpdateUI()
    {
        PlayerHPText.text = $"HP:{PlayerHP}/{PlayerMaxHP}";
        if (currentEnemy != null)
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

        int exp = 0;
        // 敵の情報から経験値を取得します
        if(currentEnemy != null)
        {
            exp = currentEnemy.ExpReward;
        }

        int levelUps = 0;
        if(LevelSystem != null)
        {
            levelUps = LevelSystem.AddExp(exp);
        }

        ApplyPlayerStatus();

        UpdateUI();

        if(levelUps > 0)
        {
            DialogText.text +=
                $"\n{exp} EXP かくとく！" +
                $"\nレベルが {PlayerStatus.Level} になった！";
        }
        else
        {
            DialogText.text +=
                $"\n{exp} EXP かくとく！";
        }

        // Dieアニメーションを再生
        if (enemyAnimator != null)
        {
            enemyAnimator.SetTrigger("Die");
        }

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
