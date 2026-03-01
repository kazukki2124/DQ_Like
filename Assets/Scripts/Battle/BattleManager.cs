using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
public enum BattleMenuState
{
    Root,   // たたかう/さくせん/にげる
    Fight,  // こうげき/じゅもん/とくぎ/ぼうぎょ
    Busy    // 演出中(入力不可)
}

public class BattleManager : MonoBehaviour
{
    public static int NextEnemyID = 0;

    public class ActiveEnemy
    {
        public EnemyData BaseData;
        public float HP;
        public GameObject Instance;
        public Animator Anim;
        public string DisplayName;
    }

    private List<ActiveEnemy> activeEnemies = new List<ActiveEnemy>();

    [Header("EnemyData")]
    public EnemyDatabase EnemyDB;

    [Header("Enemy Visual")]
    public Transform EnemyModelRoot;

    [Header("PlayerStatusとLevelSystemの参照")]
    public PlayerStatus PlayerStatus;
    public LevelSystem LevelSystem;

    [Header("PlayerData")]
    public float PlayerMaxHP = 30f;
    public float PlayerHP = 30;
    public float PlayerAttackMin = 5;
    public float PlayerAttackMax = 10;

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

    private BattleMenuState menuState = BattleMenuState.Root;
    private bool isGuading = false;
    private bool isPlayerTurn = true;

    void Start()
    {
        SetupEnemyFromDB();
        ApplyPlayerStatus();

        UpdateUI();

        BuildRootMenu();

        // 複数体の敵名を表示するため、ループを使用
        string appearNames = "";
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            if (i > 0) appearNames += "と ";
            appearNames += activeEnemies[i].DisplayName;
        }
        DialogText.text = $"{appearNames}が現れた！";
    }

    // データからプレイヤーの値を反映する
    public void ApplyPlayerStatus()
    {
        if(PlayerStatus == null) return;
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

        EnemyData encounteredEnemy = EnemyDB.GetByID(NextEnemyID);
        if (encounteredEnemy == null)
        {
            Debug.LogError("NextEnemyIDがEnemyDBに見つかりません");
            return;
        }

        SpawnEnemy(encounteredEnemy);
    }

    /// <summary>
    /// 敵のVisualを生成
    /// </summary>
    private void SpawnEnemy(EnemyData enemyData)
    {
        if (EnemyModelRoot == null || enemyData == null || enemyData.ModelPrefab == null)
        {
            return;
        }

        // ゲーム開始時に既に敵のモデルがあった場合、削除
        for (int i = EnemyModelRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(EnemyModelRoot.GetChild(i).gameObject);
        }
        activeEnemies.Clear();

        int spawnCount = Random.Range(1, 4); // 1〜3体をランダム生成
        string[] suffixes = { "A", "B", "C" };
        
        // 敵の間隔を少し広めに取る（2.0f -> 3.5fなどに調整）
        float spacingX = 3.5f; 
        float startX = -(spawnCount - 1) * spacingX / 2.0f;

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject modelInstance = Instantiate(enemyData.ModelPrefab, EnemyModelRoot);
            Animator anim = modelInstance.GetComponentInChildren<Animator>();

            Vector3 position = enemyData.ModelPosition;
            position.x += startX + (i * spacingX);
            
            modelInstance.transform.localPosition = position;
            modelInstance.transform.localEulerAngles = enemyData.ModelRotation;
            modelInstance.transform.localScale = enemyData.ModelScale;

            string displayName = spawnCount > 1 ? $"{enemyData.DisplayName} {suffixes[i]}" : enemyData.DisplayName;

            activeEnemies.Add(new ActiveEnemy
            {
                BaseData = enemyData,
                HP = enemyData.MaxHP,
                Instance = modelInstance,
                Anim = anim,
                DisplayName = displayName
            });
        }
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
        // 小数点切り上げでプレイヤーの攻撃力を計算する
        var damage = Mathf.Ceil(Random.Range(PlayerAttackMin, PlayerAttackMax));

        // 生存している最初の敵を取得
        ActiveEnemy target = null;
        foreach (var enemy in activeEnemies)
        {
            if (enemy.HP > 0)
            {
                target = enemy;
                break;
            }
        }

        if (target != null)
        {
            DialogText.text = $"{target.DisplayName}に {damage} ダメージ！";
            target.HP -= damage;
            if (target.HP <= 0)
            {
                target.HP = 0;
                if (target.Anim != null) target.Anim.SetTrigger("Die");
            }
        }
        else
        {
            DialogText.text = "しかし 誰もいなかった！";
        }

        UpdateUI();
        yield return new WaitForSeconds(0.8f);

        // すべての敵が倒されたかチェック
        bool allDead = true;
        foreach (var enemy in activeEnemies)
        {
            if (enemy.HP > 0)
            {
                allDead = false;
                break;
            }
        }

        if (allDead)
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
        // 小数点切り上げでプレイヤーの攻撃力を計算する
        var damage = Mathf.Ceil(Random.Range(PlayerAttackMin, PlayerAttackMax) * 1.6f + 2);

        ActiveEnemy target = null;
        foreach (var enemy in activeEnemies)
        {
            if (enemy.HP > 0)
            {
                target = enemy;
                break;
            }
        }

        if (target != null)
        {
            DialogText.text = $"{target.DisplayName}に {damage} ダメージ！";
            target.HP -= damage;
            if (target.HP <= 0)
            {
                target.HP = 0;
                if (target.Anim != null) target.Anim.SetTrigger("Die");
            }
        }
        else
        {
            DialogText.text = "しかし 誰もいなかった！";
        }

        UpdateUI();
        yield return new WaitForSeconds(0.8f);

        bool allDead = true;
        foreach (var enemy in activeEnemies)
        {
            if (enemy.HP > 0)
            {
                allDead = false;
                break;
            }
        }

        if (allDead)
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
        var damage = Mathf.Ceil(Random.Range(PlayerAttackMin, PlayerAttackMax));

        ActiveEnemy target = null;
        foreach (var enemy in activeEnemies)
        {
            if (enemy.HP > 0)
            {
                target = enemy;
                break;
            }
        }

        if (target != null)
        {
            DialogText.text = $"{target.DisplayName}に {damage} ダメージ！";
            target.HP -= damage;
            if (target.HP <= 0)
            {
                target.HP = 0;
                if (target.Anim != null) target.Anim.SetTrigger("Die");
            }
        }
        else
        {
            DialogText.text = "しかし 誰もいなかった！";
        }

        UpdateUI();
        yield return new WaitForSeconds(1f);

        bool allDead = true;
        foreach (var enemy in activeEnemies)
        {
            if (enemy.HP > 0)
            {
                allDead = false;
                break;
            }
        }

        if (allDead)
        {
            Victory();
        }
        else
        { // そうじゃなかったら戦闘続行
            StartCoroutine(EnemyTurn());
        }
    }

    private System.Collections.IEnumerator EnemyTurn()
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy.HP <= 0) continue; // 死んでいる敵はスキップ

            DialogText.text = $"{enemy.DisplayName}の攻撃！";

            // Attackアニメーションを再生
            if (enemy.Anim != null)
            {
                enemy.Anim.SetTrigger("Attack");
            }

            yield return new WaitForSeconds(1f);

            // 小数点切り上げで敵からのダメージを計算する
            var damage = Mathf.Ceil(
                    Random.Range(enemy.BaseData.AttackMin,
                    enemy.BaseData.AttackMax)
                    );

            // Playerが防御中
            if (isGuading)
            {
                damage = Mathf.Ceil(damage * 0.5f);
                // 1回目の攻撃で防御を消費せず、ターン終了時に解除するようにする
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
                yield break;
            }
        }
        
        // 全滅しなかったので防御フラグ解除とメニュー戻り
        isGuading = false;
        isPlayerTurn = true;
        SetMenuState(BattleMenuState.Root);
        DialogText.text = "どうする？";
    }

    /// <summary>
    /// HPなどのUIの更新
    /// </summary>
    public void UpdateUI()
    {
        PlayerHPText.text = $"HP:{PlayerHP}/{PlayerMaxHP}";

        if (activeEnemies.Count > 0)
        {
            string names = "";
            string hps = "";
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                var enemy = activeEnemies[i];
                if (enemy.HP > 0)
                {
                    names += $"{enemy.DisplayName}\n";
                    hps += $"HP:{enemy.HP}/{enemy.BaseData.MaxHP}\n";
                }
                else
                {
                    // 倒された敵はグレー表示など工夫可能だが、今回はそのまま0表記
                    names += $"<color=#888888>{enemy.DisplayName}</color>\n";
                    hps += $"<color=#888888>HP:0/{enemy.BaseData.MaxHP}</color>\n";
                }
            }
            EnemyNameText.text = names.TrimEnd('\n');
            EnemyHPText.text = hps.TrimEnd('\n');
        }
        else
        {
            EnemyNameText.text = "Enemy";
            EnemyHPText.text = "HP:0";
        }
    }

    private void Victory()
    {
        DialogText.text = "勝利！";

        int totalExp = 0;
        // 生存・死亡問わずすべての敵から経験値を合算します
        foreach (var enemy in activeEnemies)
        {
            if (enemy.BaseData != null)
            {
                totalExp += enemy.BaseData.ExpReward;
            }
        }

        int levelUps = 0;
        if(LevelSystem != null)
        {
            levelUps = LevelSystem.AddExp(totalExp);
        }

        ApplyPlayerStatus();

        UpdateUI();

        if(levelUps > 0)
        {
            DialogText.text +=
                $"\n{totalExp} EXP かくとく！" +
                $"\nレベルが {PlayerStatus.Level} になった！";
        }
        else
        {
            DialogText.text +=
                $"\n{totalExp} EXP かくとく！";
        }

        // Dieアニメーションは敵を倒した時に個別に再生するためここは削除または何もしない

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
