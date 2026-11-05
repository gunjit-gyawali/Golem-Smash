using UnityEngine;

public class enemy : MonoBehaviour
{
    public float health = 100f;
    public Transform player;
    [SerializeField] private bool spriteFacesRight = false;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public void LookAtPlayer()
    {
        float distanceX = player.position.x - transform.position.x;

        if (Mathf.Abs(distanceX) < 0.01f) return;

        float facing = Mathf.Sign(distanceX);

        if (!spriteFacesRight)
        {
            facing = -facing;
        }

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * facing;
        transform.localScale = scale;
    }
}