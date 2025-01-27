using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    public int healthIncreaseAmount = 20; // Amount of health to increase when the chest is opened
    public KeyCode interactKey = KeyCode.E; // Key to interact with the chest
    public float interactionRange = 1.5f; // Range within which the player can interact with the chest
    public GameObject openChestPrefab; // Prefab to replace the chest when opened (optional)
    public GameObject chestInteractionUI; // Reference to the UI box

    private bool _isOpened = false; // Whether the chest has been opened
    private GameObject player; // Reference to the player

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // Find the player by tag

        // Hide the UI box at the start
        if (chestInteractionUI)
        {
            chestInteractionUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (_isOpened) return; // Don't do anything if the chest is already opened

        // Check if the player is within interaction range
        if (Vector2.Distance(transform.position, player.transform.position) <= interactionRange)
        {
            // Show the UI box
            if (chestInteractionUI && !chestInteractionUI.activeSelf)
            {
                chestInteractionUI.SetActive(true);
            }

            // Check if the player presses the interact key
            if (Input.GetKeyDown(interactKey))
            {
                OpenChest();
            }
        }
        else
        {
            // Hide the UI box if the player is out of range
            if (chestInteractionUI && chestInteractionUI.activeSelf)
            {
                chestInteractionUI.SetActive(false);
            }
        }
    }

    private void OpenChest()
    {
        _isOpened = true;

        // Hide the UI box
        if (chestInteractionUI)
        {
            chestInteractionUI.SetActive(false);
        }

        // Increase the player's health
        TopDownCharacterController playerController = player.GetComponent<TopDownCharacterController>();
        if (playerController)
        {
            playerController.health =
                Mathf.Min(playerController.health + healthIncreaseAmount, playerController.maxHealth);
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
            Debug.Log("Chest opened! Health increased.");
        }
    }
}
