using UnityEngine;

public class ScreenshotButtonBehaviour : MonoBehaviour
{
    public string screenshotFileName = "Screenshot";

    public void TakeScreenshot()
    {
        // Generate a unique filename with a timestamp
        string filePath = $"{Application.persistentDataPath}/{screenshotFileName}_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
        
        // Capture the screenshot and save it to the specified path
        ScreenCapture.CaptureScreenshot(filePath);
        
        Debug.Log($"Screenshot saved to: {filePath}");
    }
}