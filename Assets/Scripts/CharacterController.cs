using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TopDownCharacterController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 10f;
    
    [Header("Shooting Settings")]
    public GameObject bubblePrefab;
    public Transform bubbleSpawnPoint;
    public float bubbleSpeed = 10f;
    public float bubbleLifetime = 2f;
    public float bubbleCooldown = 0.2f; // Time between shots
    
    [Header("Stamina Settings")]
    public float stamina = 300f;
    public float maxStamina = 300f;
    public float staminaDepletionRate = 20f;
    public float staminaRegenRate = 10f;
    
    [Header("Health Settings")]
    public float health = 100f;
    public float maxHealth = 100f;
    
    [Header("UI References")]
    public Slider staminaBar;
    public Slider healthBar;
    
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movementInput;
    private float lastShotTime;
    private int trapDamage = 15;
    
    public AudioSource audioSource;
    public AudioClip clip;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        UpdateStaminaBar();
        UpdateHealthBar();
    }
    
    private void Update()
    {
        if (EnemyControllerSpum.IsDialogueActive)
        {
            moveSpeed = 0;
            return;
        }

        if (!EnemyControllerSpum.IsDialogueActive)
        {
            moveSpeed = 5f;
        }
        
        CaptureInput();
        HandleAnimation();
        HandleShooting();
        RegenerateStamina();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Trap"))
        {
            Debug.Log("You Stepped on a Trap");
            health -= trapDamage;
            UpdateHealthBar();
        }
        
        if (other.CompareTag("Win"))
        {
            Debug.Log("You Won the game GG");
            SceneManager.LoadScene(1);
        }

      
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }
    
    private void CaptureInput()
    {
        // Smooth input capture
        movementInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;
    }
    
    private void HandleAnimation()
    {
        if (movementInput != Vector2.zero)
        {
            animator.SetBool("IsMoving", true);
                
            // Determine dominant movement direction
            if (Mathf.Abs(movementInput.x) > Mathf.Abs(movementInput.y))
            {
                animator.SetInteger("Direction", movementInput.x > 0 ? 2 : 3); // Right : Left
            }
            else
            {
                animator.SetInteger("Direction", movementInput.y > 0 ? 1 : 0); // Up : Down
            }
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }
    
    
    private void HandleMovement()
    {
        // Implement smooth acceleration and deceleration
        Vector2 targetVelocity = movementInput * moveSpeed;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, 
            (movementInput != Vector2.zero ? acceleration : deceleration) * Time.fixedDeltaTime);
    }
    
    private void HandleShooting()
    {
        // Allow shooting with spacebar, with cooldown and stamina check
        if (Input.GetKey(KeyCode.Space) && 
            Time.time - lastShotTime >= bubbleCooldown && 
            stamina >= staminaDepletionRate)
        {
            audioSource.PlayOneShot(clip);
            ShootBubble();
            lastShotTime = Time.time;
        }
    }
    
    private void ShootBubble()
    {
        int direction = animator.GetInteger("Direction");
        Vector2 bubbleVelocity = direction switch
        {
            0 => Vector2.down,
            1 => Vector2.up,
            2 => Vector2.right,
            3 => Vector2.left,
            _ => throw new ArgumentOutOfRangeException()
        };
    
        // Instantiate bubble with more precise velocity
        var bubble = Instantiate(bubblePrefab, bubbleSpawnPoint.position, Quaternion.identity);
        bubble.GetComponent<Rigidbody2D>().linearVelocity = bubbleVelocity * bubbleSpeed;
            
        var bubbleBehavior = bubble.GetComponent<BubbleDestroy>();
        if (bubbleBehavior)
        {
            bubbleBehavior.lifetime = bubbleLifetime;
        }
    
        // Deplete stamina
        // Debug.Log(stamina);
        stamina -= staminaDepletionRate;
        UpdateStaminaBar();
    }
    
    private void RegenerateStamina()
    {
        if (stamina < maxStamina)
        {
            stamina += staminaRegenRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
            UpdateStaminaBar();
        }
    }
    
    private void UpdateStaminaBar()
    {
        if (staminaBar)
        {
            staminaBar.value = stamina / maxStamina;
        }
    }
    
    public void UpdateHealthBar()
    {
        if (healthBar)
        {
            healthBar.value = health / maxHealth;
        }
    }
    
    public void TakeDamage(float damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthBar();
    
        if (health <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        SceneManager.LoadScene(0);
    }
}