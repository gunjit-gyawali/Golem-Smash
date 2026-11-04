using UnityEngine;

public class slash : MonoBehaviour
{
    [SerializeField] private float damage = 20f;

    void OnTriggerEnter2D(Collider2D collision)
    {
        enemy enemyScript = collision.GetComponentInParent<enemy>();

        if (enemyScript)
        {
            enemyScript.TakeDamage(damage);
        }
    }
}