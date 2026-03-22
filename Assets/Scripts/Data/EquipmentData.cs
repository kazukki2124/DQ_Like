using UnityEngine;
[CreateAssetMenu(menuName = "DQ-Like/Equipment/EquipmentData",
    fileName = "Equip_")]
public class EquipmentData : ScriptableObject
{
    public enum EquipmentType
    {
        Weapon, //武器
        Armor   //防具
    }

    public string DisplayName;
    public EquipmentType Type;

    [Header("能力補正")]
    public float BonusHp;
    public float BonusAttack;
    public float BonusDefence;

    [Header("Shop用のデータ")]
    public int Price;   //値段

}
