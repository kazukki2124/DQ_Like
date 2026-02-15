using UnityEngine;

[CreateAssetMenu(menuName ="DQ-Like/Battle/EnemyDatabase",
    fileName ="EnemyDatabase_")]
public class EnemyDatabase : ScriptableObject
{
    public EnemyData[] Enemies;

    public EnemyData GetByID(int id)
    {
        if(Enemies == null)
        {
            return null;
        }
        for (int i = 0; i < Enemies.Length; i++)
        {
            // ‚à‚µEnemies‚É“o˜^‚³‚ê‚Ä‚¢‚é“Gî•ñ‚ª‚ ‚è
            // ‚©‚ÂA‚±‚Ìƒƒ\ƒbƒh‚ªŒÄ‚Î‚ê‚½Û‚Ìˆø”‚ªid‚Æˆê’v
            if(Enemies[i] != null && Enemies[i].EnemyID == id)
            {
                // Enemies‚É“o˜^‚³‚ê‚Ä‚¢‚é“Gî•ñ‚ð•Ô‚µ‚Ä‚ ‚°‚é
                return Enemies[i];
            }
        }
        return null;
    }
}
