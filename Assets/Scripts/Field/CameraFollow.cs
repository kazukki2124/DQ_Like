using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    /// <summary>
    /// カメラが追従するターゲット(Player)
    /// </summary>
    public Transform Target;

    /// <summary>
    /// Targetをどう見るかのオフセット
    /// </summary>
    public Vector3 Offset = new Vector3(0, 4, -6f);

    public float FollowSpeed = 10f;

    public float LookHeight = 1.5f;

    // Updateが処理された後に実行される処理を書く場所
    private void LateUpdate()
    {
        if (Target == null)
        {
            return;
        }

        Vector3 desired = Target.position + Offset;
        // カメラのPositionを線形補完で更新する
        transform.position = Vector3.Lerp(
            transform.position,
            desired,
            FollowSpeed * Time.deltaTime
            );
        // カメラの回転の更新も行う
        Vector3 lookPos = Target.position + Vector3.up * LookHeight;
        transform.LookAt(lookPos);
    }
}
