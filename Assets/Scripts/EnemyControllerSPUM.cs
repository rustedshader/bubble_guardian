using Cainos.PixelArtTopDown_Basic;
using UnityEngine;

public class EnemyControllerSpum : MonoBehaviour
{
    public float moveSpeed = 3f; 
    public float chaseRange = 5f; 
    public float attackRange = 1.5f; 
    public int damage = 10; 
    public float attackCooldown = 2f; 
    public int maxHealth = 50; 

    public Vector2 boundsMin; 
    public Vector2 boundsMax; 

    [SerializeField] private Transform player; 
    private Animator animator; 
    private int currentHealth; 
    private float lastAttackTime; 
    private bool isDead = false;
    private DialogueUI dialogueUI;
    public DialogueObject dialogue_death;
    public DialogueObject dialogue;

    // New field for attack range visualization
    public GameObject attackRangeIndicator;

    // Flag to track if dialogue has been shown
    private bool hasDialogueBeenShown = false;

    // Static flag to control movement
    public static bool IsDialogueActive = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        // Initialize DialogueUI
        dialogueUI = FindObjectOfType<DialogueUI>();
        if (dialogueUI == null)
        {
            Debug.LogError("DialogueUI not found in the scene.");
        }

        // Create attack range indicator if not assigned
        if (attackRangeIndicator == null)
        {
            attackRangeIndicator = CreateAttackRangeIndicator();
        }
    }

    private void Update()
    {
        if (isDead || IsDialogueActive) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Update attack range indicator position
        if (attackRangeIndicator)
        {
            attackRangeIndicator.transform.position = transform.position;
            attackRangeIndicator.SetActive(distanceToPlayer <= chaseRange);
        }

        if (distanceToPlayer <= chaseRange)
        {
            if (!hasDialogueBeenShown)
            {
                IsDialogueActive = true;
                dialogueUI.ShowDialogue(dialogue);
                hasDialogueBeenShown = true;
            }
            
            if (distanceToPlayer <= attackRange)
            {
                
                Attack();
            }
            else
            {
                Chase();
            }
        }
        else
        {
            Idle();
        }
    }

    private GameObject CreateAttackRangeIndicator()
    {
        GameObject indicator = new GameObject("AttackRangeIndicator");
        SpriteRenderer spriteRenderer = indicator.AddComponent<SpriteRenderer>();
        
        // Create a circular sprite for the attack range
        Texture2D circleTexture = new Texture2D(128, 128, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[128 * 128];
        
        for (int y = 0; y < 128; y++)
        {
            for (int x = 0; x < 128; x++)
            {
                // Calculate distance from center
                float distanceFromCenter = Vector2.Distance(new Vector2(x, y), new Vector2(64, 64));
                
                // Create a ring effect
                if (distanceFromCenter >= 50 && distanceFromCenter <= 58)
                {
                    pixels[y * 128 + x] = new Color(1f, 0f, 0f, 0.5f); // Semi-transparent red
                }
                else
                {
                    pixels[y * 128 + x] = Color.clear;
                }
            }
        }
        
        circleTexture.SetPixels(pixels);
        circleTexture.Apply();
        
        Sprite circleSprite = Sprite.Create(circleTexture, 
            new Rect(0, 0, 128, 128), 
            new Vector2(0.5f, 0.5f));
        
        spriteRenderer.sprite = circleSprite;
        spriteRenderer.sortingOrder = -1; // Ensure it's behind other objects
        
        indicator.transform.localScale = new Vector3(attackRange * 2, attackRange * 2, 1);
        
        return indicator;
    }

    private void Idle()
    {
        animator.speed = 1f; // Ensure idle animation plays at normal speed
        animator.Play("IDLE");
    }

    private void Chase()
    {
        // Reduce animation speed to slow down running
        animator.speed = 0.5f; // Adjust this value to control running speed

        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 newPosition = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        transform.position = newPosition;

        if (direction.x > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        animator.Play("MOVE");
    }

    private void Attack()
    {
        // Check if enough time has passed since the last attack
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            // Play attack animation
            animator.Play("ATTACK");

            // Deal damage to the player if within attack range
            if (Vector2.Distance(transform.position, player.position) <= attackRange)
            {
                player.GetComponent<TopDownCharacterController>().TakeDamage(damage);
            }

            // Update last attack time
            lastAttackTime = Time.time;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // Don't take damage if already dead

        // Reduce health
        currentHealth -= damage;

        // Log the enemy's health
        Debug.Log("Enemy Health: " + currentHealth);

        // Play hurt animation
        animator.Play("DAMAGED");

        // Check if the enemy is dead
        if (currentHealth <= 0)
        {
            IsDialogueActive = true;
            dialogueUI.ShowDialogue(dialogue_death);
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        // Play death animation
        animator.Play("DEATH");

        // Disable collider and rigidbody
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;

        // Destroy the enemy after the death animation finishes
        Destroy(gameObject, 1f); // Adjust the delay based on the animation length
        
    }
}