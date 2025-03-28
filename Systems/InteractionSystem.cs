using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// --------------------------------------------------------
/// InteractionSystem
///
/// Manages player interactions with world objects:
/// - Picking up books from the drop-off box
/// - Shelving books on correct bookshelves
/// - Accessing the shop
/// Also handles:
/// - Highlighting the next target shelf
/// - Pointing an arrow to objectives
/// - Displaying "Press E" UI prompts
/// --------------------------------------------------------
public class InteractionSystem : MonoBehaviour
{
    public float interactionDistance = 0.8f;             // Max distance to interact with objects
    public LayerMask interactableLayer;                  // Layer mask for valid interactables

    public TextMeshProUGUI interactionText;              // UI prompt shown on valid target
    public Material highlightMaterial;                   // Material used to highlight a shelf
    public Material defaultMaterial;                     // Default shelf material

    private Camera playerCamera;                         // Player's camera for raycasting
    private List<Transform> bookshelves;                 // All available bookshelves in the scene
    private Queue<Transform> bookAssignments;            // Queued shelf targets for current books

    private int bookCount;                               // Number of books the player is carrying
    private const int maxBooks = 5;                      // Max books player can carry

    private ArrowUIController arrowUIController;         // UI arrow that points to objectives
    private Transform dropOffBox;                        // Drop-off box transform
    private PlayerPoints playerPoints;                   // Reference for awarding points
    private GameObject perkMenuUI;                       // (Reserved for future shop menu)
    private bool isPerkMenuOpen;                         // (Unused toggle for shop menu)

    private Transform currentlyHighlightedShelf;         // Shelf currently highlighted
    private int booksShelved;                            // Total number of books placed

    private void Start()
    {
        // --- Grab references and initialize ---
        playerCamera = Camera.main;
        interactionText.gameObject.SetActive(false);

        bookshelves = new List<Transform>();
        bookAssignments = new Queue<Transform>();

        arrowUIController = FindFirstObjectByType<ArrowUIController>();
        playerPoints = FindFirstObjectByType<PlayerPoints>();

        // --- Find drop-off box in scene ---
        GameObject dropOffBoxObject = GameObject.FindGameObjectWithTag("Drop-Off Box");
        if (dropOffBoxObject != null)
        {
            dropOffBox = dropOffBoxObject.transform;
        }

        // --- Find and register all bookshelves ---
        GameObject[] bookshelfObjects = GameObject.FindGameObjectsWithTag("Bookshelf");
        foreach (GameObject shelf in bookshelfObjects)
        {
            bookshelves.Add(shelf.transform);
        }

        // --- Point arrow to drop-off to start ---
        if (arrowUIController != null && dropOffBox != null)
        {
            arrowUIController.SetTarget(dropOffBox);
        }
    }

    private void Update()
    {
        HandleInteraction();
    }

    /// Detects interactable objects in front of the player and shows prompt
    private void HandleInteraction()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            interactionText.gameObject.SetActive(true);
            interactionText.text = "Press E";

            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact(hit.collider.gameObject);
            }
        }
        else
        {
            interactionText.gameObject.SetActive(false);
        }
    }

    /// Handles the interaction logic for different world objects
    private void Interact(GameObject interactableObject)
    {
        if (interactableObject.CompareTag("Drop-Off Box"))
        {
            TryPickUpBook();
        }
        else if (interactableObject.CompareTag("Bookshelf"))
        {
            HandleBookShelving(interactableObject);
        }
        else if (interactableObject.CompareTag("Shop"))
        {
            ShopSystem shopSystem = interactableObject.GetComponent<ShopSystem>();

            if (shopSystem != null)
            {
                shopSystem.OpenShop();
            }
            else
            {
                Debug.LogError("Shop system component missing on Shop object!");
            }
        }
    }

    /// Picks up a book and assigns a random shelf as the target
    private void TryPickUpBook()
    {
        if (bookCount < maxBooks)
        {
            bookCount++;
            Debug.Log("Picked up a book. Current books: " + bookCount);

            if (bookshelves.Count > 0)
            {
                Transform randomShelf = bookshelves[Random.Range(0, bookshelves.Count)];
                bookAssignments.Enqueue(randomShelf);
                UpdateShelfHighlight();

                // Point to next shelf
                if (arrowUIController != null && bookAssignments.Count > 0)
                {
                    arrowUIController.SetTarget(bookAssignments.Peek());
                }
            }
        }
        else
        {
            Debug.Log("You cannot pick up more than 5 books!");
        }
    }

    /// Attempts to place a book on a shelf if it's the correct one
    private void HandleBookShelving(GameObject bookshelf)
    {
        if (bookCount > 0 && bookAssignments.Count > 0)
        {
            if (bookAssignments.Peek() == bookshelf.transform)
            {
                bookAssignments.Dequeue();
                UpdateShelfHighlight();
                bookCount--;
                booksShelved++;

                playerPoints.AddPoints(250);

                Debug.Log($"Book placed correctly! Books left: {bookCount}, Points: {playerPoints.points}");

                // Point to next shelf or back to drop-off
                if (arrowUIController != null)
                {
                    if (bookAssignments.Count > 0)
                    {
                        arrowUIController.SetTarget(bookAssignments.Peek());
                    }
                    else
                    {
                        arrowUIController.SetTarget(dropOffBox);
                    }
                }
            }
        }
        else
        {
            Debug.Log("You have no books to place!");
        }
    }

    /// Returns number of books shelved for game over stats
    public int GetBooksShelved()
    {
        return booksShelved;
    }

    /// Updates the material on the next assigned shelf to highlight it
    private void UpdateShelfHighlight()
    {
        // Remove highlight from previous shelf
        if (currentlyHighlightedShelf != null)
        {
            MeshRenderer previousRenderer = currentlyHighlightedShelf.GetComponent<MeshRenderer>();
            if (previousRenderer != null && defaultMaterial != null)
            {
                previousRenderer.material = defaultMaterial;
            }
        }

        // Highlight the next shelf in queue
        if (bookAssignments.Count > 0)
        {
            Transform nextShelf = bookAssignments.Peek();
            MeshRenderer meshRenderer = nextShelf.GetComponent<MeshRenderer>();

            if (meshRenderer != null && highlightMaterial != null)
            {
                meshRenderer.material = highlightMaterial;
                currentlyHighlightedShelf = nextShelf;
            }
        }
        else
        {
            currentlyHighlightedShelf = null;
        }
    }
}
