using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySymbol : MonoBehaviour
{
    private string BattleSceneName = "BattleScene";

    [SerializeField]
    private float detectionRadius = 5.0f; // Range to start chasing
    [SerializeField]
    private float moveSpeed = 3.0f;       // Speed of movement

    private Vector3 initialPosition;
    private Transform playerTransform;

    private void Start()
    {
        initialPosition = transform.position;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance < detectionRadius)
        {
            // Chase Player
            Vector3 targetPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            transform.LookAt(targetPosition);
        }
        else
        {
            // Return to Initial Position
            Vector3 returnPosition = new Vector3(initialPosition.x, transform.position.y, initialPosition.z);
            if (Vector3.Distance(transform.position, returnPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, returnPosition, moveSpeed * Time.deltaTime);
                transform.LookAt(returnPosition);
            }
        }
    }

    /// <summary>
    /// 侵入判定でPlayerが入ってきたときに処理を行う
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        // PlayerのTag以外のGameObjectが侵入してきたら何もしない
        if (!other.CompareTag("Player"))
        {
            return;
        }
        SceneManager.LoadScene(BattleSceneName);
    }
}
