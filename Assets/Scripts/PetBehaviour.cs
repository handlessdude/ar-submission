﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

using Unity.Collections;
public class PetBehavior : MonoBehaviour
{
    public ThrowBehaviour ThrowBehaviour;
    public SurfaceManager SurfaceManager;
    private NavMeshAgent agent;
    private Vector3 originalPosition;
    private bool isUserDragging = false;
    private Vector3 targetPosition;
    private bool isPerformingAction = false;
    
    public Animator animator;
    public GameObject arrowPrefab;

    public Transform mouthSlot;
    private GameObject arrowInstance;
    private int ANIMATION_ID_BREATHING = 0;
    private int ANIMATION_ID_WIGGLING_TAIL = 1;
    private int ANIMATION_ID_RUN = 4;
    private int ANIMATION_ID_EAT = 5;
    private string THROWABLE_FOOD_TAG = "ThrowableFood";
    private string THROWABLE_NON_FOOD_TAG = "ThrowableNonFood";
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        originalPosition = transform.position;
        mouthSlot = ComponentFinder.FindChildWithTag(gameObject, "MouthSlot").transform;
        
        // SurfaceManager
        GameObject surfaceManagerObject = GameObject.FindWithTag("SurfaceManager");
        if (surfaceManagerObject != null)
        {
            SurfaceManager = surfaceManagerObject.GetComponent<SurfaceManager>();
            // MyLogger.Log("SurfaceManager found");
        }
        else
        {
            MyLogger.Log("SurfaceManager not found in the scene. Ensure it has the correct tag.");
            Debug.LogError("SurfaceManager not found in the scene. Ensure it has the correct tag.");
            enabled = false;
        }
        
        // ThrowBehaviour
        GameObject throwBehaviourObject = GameObject.FindWithTag("ThrowBehaviour");
        if (throwBehaviourObject != null)
        {
            ThrowBehaviour = throwBehaviourObject.GetComponent<ThrowBehaviour>();
        }
        else
        {
            MyLogger.Log("ThrowBehaviour not found in the scene. Ensure it has the correct tag.");
            Debug.LogError("ThrowBehaviour not found in the scene. Ensure it has the correct tag.");
            enabled = false;
        }
        
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
        {
            return;
        }

        // if it's throwing mode, do not drag the pet
        if (ThrowBehaviour.IsTouching)
        {
            return;
        }
        
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
            HandleArrivalAtTarget();
        }

        // Update arrow direction while dragging
        if (isUserDragging && arrowInstance != null)
        {
            UpdateArrowDirection();
        }
    }

    private Vector2 getInputPosition()
    {
        Vector2 inputPosition;
        if (Touchscreen.current?.primaryTouch.press.isPressed == true)
        {
            inputPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else if (Mouse.current?.leftButton.isPressed == true)
        {
            inputPosition = Mouse.current.position.ReadValue();
        }
        else
        {
            return new Vector2();
        }
        return inputPosition;
    }
    
    private Ray getScreenPointToRay()
    {
        Vector2 inputPosition = getInputPosition();
        return Camera.main.ScreenPointToRay(inputPosition);
    }
    
    void HandleUserInput()
    {
        if (Mouse.current?.leftButton.wasPressedThisFrame == true || Touchscreen.current?.primaryTouch.press.isPressed == true)
        {
            Ray ray = getScreenPointToRay();
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

        if (
            (Mouse.current?.leftButton.isPressed == true || Touchscreen.current?.primaryTouch.press.isPressed == true)
            && isUserDragging
            )
        {
            if (SurfaceManager.LockedPlane == null)
            {
                MyLogger.Log("SurfaceManager.LockedPlane is null!");
            }
            else
            {
                var touchPosition = getInputPosition();
                var hits = new List<ARRaycastHit>();
                if (SurfaceManager.RaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinBounds))
                {
                    foreach (var hit in hits)
                    {
                        if (hit.trackableId == SurfaceManager.LockedPlane.trackableId)
                        {
                            Vector3 hitPosition = hit.pose.position;
    
                            targetPosition = hitPosition;
                            return;
                        }
                    }
                }
            }
        }

        if (Mouse.current?.leftButton.wasReleasedThisFrame == true || Touchscreen.current?.primaryTouch.press.isPressed == false)
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
            if (collider.CompareTag(THROWABLE_FOOD_TAG) || collider.CompareTag(THROWABLE_NON_FOOD_TAG))
            {
                FaceBone(collider.transform); // Make the pet look at the bone
                var throwable = collider.gameObject;
                StartCoroutine(HandleEatingAnimation(throwable));
                break;
            }
        }
    }

    void FaceBone(Transform boneTransform)
    {
        Vector3 directionToBone = (boneTransform.position - transform.position).normalized;
        directionToBone.y = 0; // Ignore vertical differences
        transform.rotation = Quaternion.LookRotation(directionToBone);
    }
    
    IEnumerator HandleEatingAnimation(GameObject throwable)
    {
        isPerformingAction = true;
        animator.SetInteger("AnimationID", ANIMATION_ID_EAT);
        yield return new WaitForSeconds(1.25f);
        animator.SetInteger("AnimationID", ANIMATION_ID_BREATHING);
        Destroy(throwable);
        isPerformingAction = false;
    }
    
    IEnumerator HandleWigglingTailAnimation()
    {
        isPerformingAction = true;
        animator.SetInteger("AnimationID", ANIMATION_ID_WIGGLING_TAIL);
        yield return new WaitForSeconds(1f);
        animator.SetInteger("AnimationID", ANIMATION_ID_BREATHING);
        isPerformingAction = false;
    }
}
