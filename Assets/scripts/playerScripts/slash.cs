using UnityEngine;

public class slash : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Slash hit: " + collision.gameObject.name);
        enemy enemyScript = collision.GetComponent<enemy>();
        if (enemyScript)
        {
            enemyScript.TakeDamage(20);
        }
    }
}
