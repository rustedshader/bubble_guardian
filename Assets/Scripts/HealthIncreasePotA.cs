using Cainos.PixelArtTopDown_Basic;
using UnityEngine;

public class HealthIncreasePotA : MonoBehaviour
{
    public int healthIncreaseAmount = 50; // Amount of health to increase when the chest is opened
    public KeyCode interactKey = KeyCode.E; // Key to interact with the chest
    public float interactionRange = 1.5f; // Range within which the player can interact with the chest
    public GameObject openChestPrefab; // Prefab to replace the chest when opened (optional)

    private bool isOpened = false; // Whether the chest has been opened
    private GameObject player; // Reference to the player
    // public GameObject interactionPrompt;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // Find the player by tag
    }

    private void Update()
    {
        if (isOpened) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance <= interactionRange)
        {
            // interactionPrompt.SetActive(true); // Show the prompt
            if (Input.GetKeyDown(interactKey))
            {
                OpenChest();
            }
        }
        else
        {
            // interactionPrompt.SetActive(false); // Hide the prompt

        }

        void OpenChest()
        {
            isOpened = true;

            // Increase the player's health
            TopDownCharacterController playerController = player.GetComponent<TopDownCharacterController>();
            if (playerController)
            {
                playerController.health = Mathf.Min(playerController.health + healthIncreaseAmount,
                    playerController.maxHealth);
                playerController.UpdateHealthBar(); // Update the health bar UI
            }

            // Optionally replace the chest with an open chest prefab
            if (openChestPrefab)
            {
                Instantiate(openChestPrefab, transform.position, transform.rotation);
                Destroy(gameObject); // Destroy the closed chest
            }
            else
            {
                // Play an animation or change the sprite to indicate the chest is open
                Debug.Log("Pot A opened! Health increased.");
            }
        }
    }
}
