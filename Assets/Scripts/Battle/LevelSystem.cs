using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    [Header("参照")]
    public PlayerStatus Status;
    public ExpTable ExpTable;

    [Header("レベルが上がった時の加算値(シンプル版)")]
    public float HpUpPerLevel = 5;
    public float AttackMinPerLevel = 1;
    public float AttackMaxPerLevel = 1;
   
    public int AddExp(int amount)
    {
        if(Status == null || ExpTable == null)
        {
            return 0;
        }
        if(amount <= 0)
        {
            return 0;
        }
        // データに対して経験値を加算
        Status.Exp += amount;
        int levelUpCount = 0;
        // 複数回レベルアップに対応
        while (true)
        {
            int needExp =
                ExpTable.GetNeedExpToNext(Status.Level);
            // プレイヤーの経験値がレベルアップに
            // 必要な経験値に足りていない場合は
            if(Status.Exp < needExp)
            {
                // 処理を抜ける
                break;
            }
            Status.Exp -= needExp;
            Status.Level += 1;
            levelUpCount++;
            // レベルアップで能力を伸ばす
            ApplyGrouwthOnce();
        }
        return levelUpCount;
    }
    
    private void ApplyGrouwthOnce()
    {
        Status.MaxHP += HpUpPerLevel;
        Status.AttackMin += AttackMinPerLevel;
        Status.AttackMax += AttackMaxPerLevel;
    }
}
