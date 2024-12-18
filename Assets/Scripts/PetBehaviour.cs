using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PetBehavior : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 originalPosition;
    private bool isUserDragging = false;
    private Vector3 targetPosition;

    public Animator animator;
    public GameObject arrowPrefab;

    private GameObject arrowInstance;
    private int ANIMATION_ID_BREATHING = 0;
    private int ANIMATION_ID_RUN = 4;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        originalPosition = transform.position;

        // Ensure the NavMeshAgent is placed on a NavMesh
        if (!agent.isOnNavMesh)
        {
            Debug.LogError("NavMeshAgent is not placed on a valid NavMesh!");
            enabled = false; // Disable script if agent is not on NavMesh
        }
    }

    void Update()
    {
        // Ensure agent is on NavMesh before calling NavMesh methods
        if (!agent.isOnNavMesh)
            return;

        // Check for user input
        HandleUserInput();

        // Move the pet to the target position if dragging has stopped
        if (!isUserDragging && targetPosition != Vector3.zero)
        {
            agent.SetDestination(targetPosition);
            animator.SetInteger("AnimationID", ANIMATION_ID_RUN);
        }

        // Check if pet has reached its destination
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && !isUserDragging)
        {
            animator.SetInteger("AnimationID", ANIMATION_ID_BREATHING);
        }

        // Update arrow direction while dragging
        if (isUserDragging && arrowInstance != null)
        {
            UpdateArrowDirection();
        }
    }

    void HandleUserInput()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame || Touchscreen.current?.primaryTouch.press.isPressed == true)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Check if the pet was tapped
                if (hit.collider.gameObject == gameObject)
                {
                    isUserDragging = true;

                    // Show arrow if not already shown
                    if (arrowInstance == null)
                    {
                        arrowInstance = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
                    }
                    arrowInstance.SetActive(true);
                }
            }
        }

        if ((Mouse.current.leftButton.isPressed || Touchscreen.current?.primaryTouch.press.isPressed == true) && isUserDragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                targetPosition = hit.point; // Update the target position
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame || Touchscreen.current?.primaryTouch.press.isPressed == false)
        {
            if (isUserDragging)
            {
                isUserDragging = false;

                // Hide arrow once dragging stops
                if (arrowInstance != null)
                {
                    arrowInstance.SetActive(false);
                }
            }
        }
    }

    void UpdateArrowDirection()
    {
        if (arrowInstance != null)
        {
            arrowInstance.transform.position = transform.position;
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0; // Keep the arrow horizontal
            arrowInstance.transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
