using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// PlayerがInteractor(交流)するためのclass
/// </summary>
public class Interactor : MonoBehaviour
{
    /// <summary>
    /// Distanceの値m先まで交流が行えるようにするための変数
    /// </summary>
    public float Distance = 2.0f;

    public LayerMask InteractLayer;

    public float EyeHeight = 0.5f;

    // PlayerInputのInteract(Eキー)が押された時に呼ばれます
    public void OnInteract(InputValue value)
    {
        // ボタンが押されてなかったら何もしない
        if (!value.isPressed)
        {
            return;
        }
        TryInteract();
    }

    private void TryInteract()
    {
        Vector3 origin = transform.position + Vector3.up * EyeHeight;

        // 光線を発射する
        Ray ray = new Ray(origin, transform.forward);

        // 光線がどうなっているかのデバッグ用の線を表示する
        Debug.DrawRay(origin,transform.forward * Distance,
            Color.yellow,0.5f);

        // 光線を射出してみて、当たるか同課の情報、
        // 射出する光線の長さ、当たるべきLayer
        if (Physics.Raycast(ray,out RaycastHit hit,
            Distance, InteractLayer))
        {
            // 当たった場合、
            // 当たり判定からIInteractableを取得する
            var interactable =
                hit.collider.GetComponent<IInteractable>();
            // Interact()を発火させる
            interactable?.Interact();
        }
    }
}
