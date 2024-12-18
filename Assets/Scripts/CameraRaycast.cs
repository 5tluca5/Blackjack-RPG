using UnityEngine;

public class CameraRaycast : MonoBehaviour
{
    public Camera camera;  // Reference to the camera
    public float raycastRange = 100f;  // Maximum distance of the raycast

    void Update()
    {
        // Ensure the camera is assigned
        if (camera == null)
            camera = Camera.main;

        // Create a ray from the center of the camera's viewport
        Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

        // Perform the raycast
        if (Physics.Raycast(ray, out RaycastHit hitInfo, raycastRange))
        {
            // Log the object that was hit
            Debug.Log("Hit object: " + hitInfo.collider.name);

            // Example: Draw a debug line in the editor for visualization
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red);
        }
        else
        {
            // Debug: Show the ray even if it doesn't hit anything
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * raycastRange, Color.green);
        }
    }
}
