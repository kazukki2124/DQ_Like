using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance;

    [Header("基礎のステータス情報")]
    public PlayerStatus PlayerStatus;

    [Header("現在の所持金")]
    public int CurrentGold = 500;

    /// <summary>
    /// 所持金を増やす
    /// </summary>
    public void AddGold(int amount)
    {
        CurrentGold += amount;
    }

    /// <summary>
    /// お金が足りるかのチェック
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool ConsumeGold(int amount)
    {
        if(CurrentGold >= amount)
        {
            CurrentGold -= amount;
            return true; //支払い出来ます
        }
        return false;   //支払い不可
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public float MaxHP
    {
        get
        {
            float bonus = 0;
            if(EquipmentManager.Instance != null)
            {
                bonus =
                    EquipmentManager.Instance.GetBonusMaxHP();
            }
            return PlayerStatus.MaxHP + bonus;
        }
    }

    public float AttackMax
    {
        get
        {
            float bonus = 0;
            if (EquipmentManager.Instance != null)
            {
                bonus =
                    EquipmentManager.Instance.GetBounsAttack();
            }
            return PlayerStatus.AttackMax + bonus;
        }
    }

    public float AttackMin
    {
        get
        {
            float bonus = 0;
            if (EquipmentManager.Instance != null)
            {
                bonus =
                    EquipmentManager.Instance.GetBounsAttack();
            }
            return PlayerStatus.AttackMin + bonus;
        }
    }

    public float Defence
    {
        get
        {
            if(EquipmentManager.Instance != null)
            {
                return
                    EquipmentManager.Instance.GetBonsDefence();
            }
            return 0;
        }
    }

}
