using UnityEngine;
using TMPro;

public class StatusUI : MonoBehaviour
{
    public GameObject Root;

    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI HPText;
    public TextMeshProUGUI AttackText;
    public TextMeshProUGUI DefenceText;
    public TextMeshProUGUI WeaponText;
    public TextMeshProUGUI ArmorText;

    private void OnEnable()
    {
        Refresh();
    }
    public void Refresh()
    {
        if (PlayerState.Instance == null)
        {
            return;
        }
        LevelText.text =
            $"LV：{PlayerState.Instance.PlayerStatus.Level}";
        // ほかの項目についてもデータを反映させてください。
        HPText.text =
            $"HP：{PlayerState.Instance.MaxHP}";

        AttackText.text =
            $"こうげき：{PlayerState.Instance.AttackMin}" +
            $"-{PlayerState.Instance.AttackMax}";

        DefenceText.text =
            $"ぼうぎょ：{PlayerState.Instance.Defence}";

        if (EquipmentManager.Instance != null)
        {
            WeaponText.text =
                $"ぶき：{EquipmentManager.Instance.EquipmentWeapon.DisplayName}";

            ArmorText.text =
                $"ぼうぐ：{EquipmentManager.Instance.EquipmentArmor.DisplayName}";
        }
    }

    public void Open()
    {
        gameObject.SetActive(true);
        Refresh();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
