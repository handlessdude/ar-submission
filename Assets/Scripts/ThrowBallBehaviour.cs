using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ThrowBallManager : MonoBehaviour
{
    public Camera arCamera;
    public GameObject ballPrefab;
    public GameObject arrowPrefab;
    public Button throwModeButton;
    public float throwForce = 0.5f;

    private GameObject currentArrow;
    private Vector3 targetPoint;
    private bool isThrowModeActive = false;
    private bool isTouching = false;
    
    private void Start()
    {
        throwModeButton.onClick.AddListener(ActivateThrowMode);
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
            Ray ray = arCamera.ScreenPointToRay(inputPosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                targetPoint = hit.point;
                UpdateArrow(targetPoint);
            }
        }
        else if (isTouching) // When touch or click is released
        {
            isTouching = false;
            ThrowBall(targetPoint);
            DeactivateThrowMode();
            if (currentArrow != null)
            {
                currentArrow.SetActive(false);
            }
        }
    }
    
    private void ActivateThrowMode()
    {
        isThrowModeActive = true;
    }

    private void DeactivateThrowMode()
    {
        isThrowModeActive = false;
    }

    private void UpdateArrow(Vector3 target)
    {
        Vector3 startPoint = arCamera.transform.position;
        Vector3 direction = (target - startPoint).normalized;
        float distance = Vector3.Distance(startPoint, target);

        var arrowPosition = startPoint + direction * (distance * 0.5f) - Vector3.up * 0.5f;

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
        currentArrow.transform.rotation = Quaternion.LookRotation(direction);
    }

    private void ThrowBall(Vector3 target)
    {
        GameObject ball = Instantiate(ballPrefab, arCamera.transform.position, Quaternion.identity);
        Rigidbody rb = ball.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 throwDirection = (target - arCamera.transform.position).normalized;
            rb.AddForce(throwDirection * throwForce);
        }
    }
}
