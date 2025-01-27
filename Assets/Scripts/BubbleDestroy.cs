using UnityEngine;

public class BubbleDestroy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float lifetime = 2f; // Time in seconds before the bubble disappears
    public int damage = 20; // Damage dealt to the enemy

    private void Start()
    {
        // Destroy the bubble after the specified lifetime
        Destroy(gameObject, lifetime);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the bubble collides with the enemy
        if (collision.CompareTag("Enemy"))
        {
            // Deal damage to the enemy
            EnemyControllerSpum enemy = collision.GetComponent<EnemyControllerSpum>();
            if (enemy != null)
            {
                Debug.Log("Bubble hit");
                enemy.TakeDamage(damage);
            }


            // Destroy the bubble on collision
            Destroy(gameObject);
        }
    }
    
}
