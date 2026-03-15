using UnityEngine;
using TMPro;

public class EnemyUI : MonoBehaviour
{
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI HPText;

    private BattleManager.ActiveEnemy targetEnemy;

    public void Setup(BattleManager.ActiveEnemy enemy)
    {
        targetEnemy = enemy;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (targetEnemy == null) return;

        if (targetEnemy.HP > 0)
        {
            NameText.text = targetEnemy.DisplayName;
            HPText.text = $"HP:{targetEnemy.HP}/{targetEnemy.BaseData.MaxHP}";
        }
        else
        {
            // 死亡時
            NameText.text = $"<color=#888888>{targetEnemy.DisplayName}</color>";
            HPText.text = $"<color=#888888>HP:0/{targetEnemy.BaseData.MaxHP}</color>";
        }
    }

    private void LateUpdate()
    {
        // 常にカメラの方を向くようにする（ビルボード処理）
        if (Camera.main != null)
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                             Camera.main.transform.rotation * Vector3.up);
        }
    }
}
