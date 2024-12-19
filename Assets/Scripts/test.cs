using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class test : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 originalPosition;
    private bool isUserDragging = false;
    private Vector3 targetPosition;
    private bool isPerformingAction = false;
    
    public Animator animator;
    public GameObject arrowPrefab;
    public Transform mouthSlot; // Reference to the "MouthSlot" Transform

    private GameObject arrowInstance;
    private GameObject currentObjectInMouth; // Store the object in the pet's mouth
    private int ANIMATION_ID_BREATHING = 0;
    private int ANIMATION_ID_RUN = 4;
    private int ANIMATION_ID_EAT = 5;
    private string FOOD_TAG = "Food";
    private string BALL_TAG = "Ball";

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        originalPosition = transform.position;
        mouthSlot = ComponentFinder.FindChildWithTag(gameObject, "MouthSlot").transform;
            
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
        if (
            !agent.pathPending
            && agent.remainingDistance <= agent.stoppingDistance
            && !isUserDragging
            && !isPerformingAction
            )
        {
            HandleArrivalAtTarget();
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
                if (hit.collider.gameObject == gameObject && !isPerformingAction)
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

            float distance = direction.magnitude;
            arrowInstance.transform.localScale = new Vector3(arrowInstance.transform.localScale.x, arrowInstance.transform.localScale.y, distance);
            arrowInstance.transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void HandleArrivalAtTarget()
    {
        animator.SetInteger("AnimationID", ANIMATION_ID_BREATHING);

        Collider[] colliders = Physics.OverlapSphere(transform.position, agent.stoppingDistance);
       
        foreach (var collider in colliders)
        {
            if (collider.CompareTag(FOOD_TAG))
            {
                FaceThrowable(collider.transform);
                StartCoroutine(HandleEating(collider.gameObject));
                break;
            }

            if (collider.CompareTag(BALL_TAG))
            {
                FaceThrowable(collider.transform);
                StartCoroutine(HandleEating(collider.gameObject));
                break;
            }
        }
    }

    void FaceThrowable(Transform boneTransform)
    {
        Vector3 directionToBone = (boneTransform.position - transform.position).normalized;
        directionToBone.y = 0; // Ignore vertical differences
        transform.rotation = Quaternion.LookRotation(directionToBone);
    }

    void PlaceInMouth(GameObject objectToEat)
    {
        if (objectToEat != null)
        {
            Debug.Log("PlaceInMouth");
             currentObjectInMouth = objectToEat;
             currentObjectInMouth.transform.SetParent(mouthSlot);
             currentObjectInMouth.transform.localPosition = Vector3.zero; // Place at MouthSlot's local origin
     
             Vector3 mouthSlotForward = mouthSlot.forward;
             Vector3 mouthSlotUp = mouthSlot.up;
     
             // Make the ball's local forward axis match the MouthSlot's forward axis
             currentObjectInMouth.transform.rotation = Quaternion.LookRotation(mouthSlotForward, mouthSlotUp);
             
             // Disable the collider and Rigidbody of the ball while it's in the mouth
             Collider ballCollider = currentObjectInMouth.GetComponent<Collider>();
             if (ballCollider != null)
             {
                 ballCollider.enabled = false;
             }
             Rigidbody ballRb = currentObjectInMouth.GetComponent<Rigidbody>();
             if (ballRb != null)
             {
                 Destroy(ballRb);
             }
        }
    }
    
    IEnumerator HandleEating(GameObject throwable)
    {
        if (throwable != null)
        {
            isPerformingAction = true;
            animator.SetInteger("AnimationID", ANIMATION_ID_EAT);
            yield return new WaitForSeconds(0.5f);
            animator.SetInteger("AnimationID", ANIMATION_ID_BREATHING);
            
            float probability = Random.Range(0f, 1f);
    
            if (currentObjectInMouth != null)
            {
                DropCurrentObject();
            }
            
            // Perform an action if the probability is less than or equal to 0.5 (50% chance)
            if (probability <= 0.5f)
            {
                Debug.Log("destroy");
                Destroy(throwable);
            }
            else
            {
                PlaceInMouth(throwable);
            }
            
            isPerformingAction = false;
        }
    }

    void DropCurrentObject()
    {
        if (currentObjectInMouth != null)
        {
            Collider ballCollider = currentObjectInMouth.GetComponent<Collider>();
            if (ballCollider != null)
            {
                ballCollider.enabled = true;
            }
            
            currentObjectInMouth.AddComponent<Rigidbody>();
            Rigidbody rb = currentObjectInMouth.GetComponent<Rigidbody>();
            rb.isKinematic = false;
                
            currentObjectInMouth.transform.SetParent(null);
            currentObjectInMouth = null;
        }
    }
}
