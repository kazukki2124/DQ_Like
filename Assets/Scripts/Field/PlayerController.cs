using UnityEngine;
using UnityEngine.InputSystem;

/// RequireComponent(typeof(Hogehoge))は
/// 必須のコンポーネントを強制出来る
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// 移動速度(m/s)
    /// </summary>
    public float MoveSpeed = 4f;

    /// <summary>
    /// 回転速度(度/秒)
    /// </summary>
    public float RotateSpeed = 540f;

    public float Gravity = -9.81f;

    /// <summary>
    /// カメラ基準移動に使う
    /// (未設定ならプレイヤー基準)
    /// </summary>
    public Transform CameraTransform;

    private CharacterController controller;

    private Vector3 velocity;

    /// <summary>
    /// InputSystemから反映された値
    /// </summary>
    private Vector2 moveInput;

    /// <summary>
    /// Startより前に実行される処理を書く場所
    /// </summary>
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // 毎フレーム実行される処理を書く場所
    void Update()
    {
        Move();
        ApplyGravity();
    }

    /// <summary>
    /// InputSystemからMove向けに入力された値を処理する
    /// メソッド
    /// </summary>
    /// <param name="value"></param>
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void Move()
    {
        Vector3 move;

        // カメラが設定されていればカメラ基準の移動
        if(CameraTransform != null)
        {
            Vector3 camForward = CameraTransform.forward;
            Vector3 camRight = CameraTransform.right;

            camForward.y = 0;
            camRight.y = 0;

            // Normalizeは0～1の値に正規化してくれます
            camForward.Normalize();
            camRight.Normalize();

            // 例えばAキーを押されたらmoveinputの値は
            // (-1,0)になります。
            // それをカメラの画角から見て左(x軸-1の方向)
            // というデータに変換します
            move = camForward * moveInput.y +
                   camRight * moveInput.x;
        }
        else
        {
            // 未設定時にはプレイヤー基準で移動
            move = transform.forward * moveInput.y +
                   transform.right * moveInput.x;
        }

        if(move.sqrMagnitude > 1f)
        {
            // x,y,zの各値を0～1の値に収めます
            move.Normalize();
        }

        controller.Move(move * MoveSpeed * Time.deltaTime);

        // 入力がある時は移動方法に回転させる
        if (move.sqrMagnitude > 0.01f)
        {
            // 向く先の回転情報
            Quaternion targetRot =
                Quaternion.LookRotation(move, Vector3.up);
            // Playerに向く先を設定する
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, // Playerの回転を
                targetRot, // 向く先の回転に
                RotateSpeed * Time.deltaTime // RotateSpeedで向く
                );
        }
    }

    private void ApplyGravity()
    {
        if(controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -1f; // 地面に吸いつける
        }

        velocity.y += Gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
