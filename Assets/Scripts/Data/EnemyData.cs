using UnityEngine;

[CreateAssetMenu(menuName = "DQ-Like/Battle/EnemyData",
    fileName ="Enemy_")]
public class EnemyData : ScriptableObject
{
    public int EnemyID;         // 識別用(0,1,2...)
    public string DisplayName;  // 敵の表示名
    public float MaxHP;         // 最大体力
    public float AttackMin;     // 最小攻撃力
    public float AttackMax;     // 最大攻撃力

    [Header("battle Visual")]
    public GameObject ModelPrefab;// 敵モデルのPrefab
    public Vector3 ModelPosition = new Vector3(0, 0, 2f);   // 敵の位置
    public Vector3 ModelRotation = new Vector3(0, 180f, 0); // 敵の回転
    public Vector3 ModelScale = Vector3.one;

    [Header("Reward")]
    public int ExpReward = 5;   // 倒したらもらえる経験値
    public int GoldReward = 10; // 倒したらもらえるゴールド

    [TextArea(2,4)]
    public string Description;  // 敵についての説明

    [Header("出現数の設定")]
    public bool IsRandomCount = true; // ランダムにするか
    public int FixedCount = 1;        // ランダムにしない場合の出現数
    public int MinCount = 1;          // ランダムにする場合の最小出現数
    public int MaxCount = 3;          // ランダムにする場合の最大出現数

    [Header("ボスの設定")]
    public bool IsBoss = false;

    [Header("Audio (サウンド)")]
    public AudioClip AttackSE; // 敵固有の攻撃時のSE
}
