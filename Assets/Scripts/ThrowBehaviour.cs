using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

public class ThrowBehaviour : MonoBehaviour
{
    private Camera camera;
    public GameObject[] throwablePrefabs; // Array of throwable game object prefabs
    public GameObject arrowPrefab;
    public float throwForce = 0.25f;

    private GameObject currentArrow;
    private GameObject currentThrowable; // The currently selected throwable
    private Vector3 targetPoint;
    private bool isThrowModeActive = false;
    private bool isTouching = false;

    public Action<GameObject> onThrowableThrown; // Action that will be invoked when the throwable is thrown

    private void Start()
    {
        camera = Camera.main;
    }
    
    private void Update()
    {
        if (!isThrowModeActive) return;

        bool isTouchOrClickHeld = false; // Tracks if touch or click is held
        Vector2 inputPosition = Vector2.zero;

        // Check input source
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            // Mobile touch input
            isTouchOrClickHeld = true;
            inputPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            // Desktop mouse input
            isTouchOrClickHeld = true;
            inputPosition = Mouse.current.position.ReadValue();
        }

        if (isTouchOrClickHeld)
        {
            isTouching = true;
            Ray ray = camera.ScreenPointToRay(inputPosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                targetPoint = hit.point;
                UpdateArrow(targetPoint);
            }
        }
        else if (isTouching) // When touch or click is released
        {
            isTouching = false;
            ThrowCurrentThrowable(targetPoint);
            DeactivateThrowMode();
            if (currentArrow != null)
            {
                currentArrow.SetActive(false);
            }
        }
    }
    
    public void ActivateThrowMode(int throwableIndex = 0)
    {
        isThrowModeActive = true;
		SetCurrentThrowable(throwableIndex);
    }

    private void DeactivateThrowMode()
    {
        isThrowModeActive = false;
    }

    private void UpdateArrow(Vector3 target)
    {
        Vector3 startPoint = camera.transform.position;
        Vector3 direction = (target - startPoint).normalized;
        float distance = Vector3.Distance(startPoint, target);

        // var arrowPosition = startPoint + direction * (distance * 0.25f) - Vector3.up * 0.5f;
        var arrowPosition = target;

        if (currentArrow != null)
        {
            if (!currentArrow.activeSelf)
            {
                currentArrow.SetActive(true);
            }
            // Position the arrow midpoint between the camera and target
            currentArrow.transform.position = arrowPosition;
        }
        if (currentArrow == null)
        {
            currentArrow = Instantiate(arrowPrefab, arrowPosition, Quaternion.identity);
        }

        // Rotate the arrow to point toward the target
        currentArrow.transform.rotation = Quaternion.LookRotation(direction - Vector3.up * 0.5f);
    }

    // Method to set the current throwable based on the index
    public void SetCurrentThrowable(int index)
    {
        if (index >= 0 && index < throwablePrefabs.Length)
        {
            currentThrowable = throwablePrefabs[index];
        }
        else
        {
            Debug.LogError("Invalid throwable index");
        }
    }

    // Renamed method to throw the currently selected throwable
    private void ThrowCurrentThrowable(Vector3 target)
    {
        if (currentThrowable == null)
        {
            Debug.LogError("No throwable object set.");
            return;
        }

        GameObject throwable = Instantiate(currentThrowable, camera.transform.position, Quaternion.identity);
        Rigidbody rb = throwable.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 throwDirection = (target - camera.transform.position).normalized;
            rb.AddForce(throwDirection * throwForce);

            // Invoke the action when the throwable is thrown
            onThrowableThrown?.Invoke(throwable);
        }
    }
}
