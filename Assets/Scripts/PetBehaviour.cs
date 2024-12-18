using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PetBehavior : MonoBehaviour
{
    public string ballTag = "Ball";
    public string petHouseTag = "PetHouse";
    private NavMeshAgent agent;
    private Transform ball;
    private Vector3 originalPosition;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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
        
        // Check for the ball's presence in the scene
        GameObject foundBall = GameObject.FindWithTag(ballTag);
        if (foundBall != null)
        {
            ball = foundBall.transform;
            agent.SetDestination(ball.position);
        }
        else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // If no ball or ball reached, return to pet house
            GameObject petHouse = GameObject.FindWithTag(petHouseTag);
            if (petHouse != null)
            {
                agent.SetDestination(petHouse.transform.position);
            }
            else
            {
                // Fall back to original position if PetHouse is missing
                agent.SetDestination(originalPosition);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Handle ball reached event
        if (other.CompareTag(ballTag))
        {
            Destroy(other.gameObject); // Simulate "consuming" the ball
        }
    }
}