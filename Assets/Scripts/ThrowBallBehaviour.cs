using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ThrowBallManager : MonoBehaviour
{
    public Camera arCamera; // Assign your AR Camera here
    public GameObject ballPrefab; // Prefab of the ball to throw
    public LineRenderer aimGizmo; // A LineRenderer for the aiming arrow
    public Button throwModeButton; // UI button to activate throw mode
    public float throwForce = 1f;

    private Vector3 targetPoint;
    private bool isThrowModeActive = false;
    private bool isTouching = false;

    private void Start()
    {
        throwModeButton.onClick.AddListener(ActivateThrowMode);
        aimGizmo.enabled = false;
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
                DrawAimGizmo(targetPoint);
            }
        }
        else if (isTouching) // When touch or click is released
        {
            isTouching = false;
            ThrowBall(targetPoint);
            DeactivateThrowMode();
        }
    }
    
    private void ActivateThrowMode()
    {
        isThrowModeActive = true;
        aimGizmo.enabled = true;
    }

    private void DeactivateThrowMode()
    {
        isThrowModeActive = false;
        aimGizmo.enabled = false;
    }

    private void DrawAimGizmo(Vector3 target)
    {
        aimGizmo.SetPosition(0, arCamera.transform.position);
        aimGizmo.SetPosition(1, target);
    }

    private void ThrowBall(Vector3 target)
    {
        GameObject ball = Instantiate(ballPrefab, arCamera.transform.position, Quaternion.identity);
        Rigidbody rb = ball.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 throwDirection = (target - arCamera.transform.position).normalized;
            Debug.Log(throwDirection.ToString());
            rb.AddForce(throwDirection * throwForce);
        }
    }
}
