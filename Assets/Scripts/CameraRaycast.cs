using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class CameraRaycast : MonoBehaviour
{
    public HUDController hudController;

    public Camera camera;  // Reference to the camera
    public float raycastRange = 100f;  // Maximum distance of the raycast

    GameObject aimTarget;
    bool raycastEnabled = true;

    void Update()
    {
        // Ensure the camera is assigned
        if (camera == null)
            camera = Camera.main;

        if (hudController == null)
            hudController = GameObject.FindGameObjectWithTag("HUDCanvas").GetComponent<HUDController>();

        // Create a ray from the center of the camera's viewport
        Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

        // Perform the raycast
        if (raycastEnabled && Physics.Raycast(ray, out RaycastHit hitInfo, raycastRange))
        {
            // Log the object that was hit
            Debug.Log("Hit object: " + hitInfo.collider.name);

            aimTarget = hitInfo.collider.gameObject;

            // Example: Draw a debug line in the editor for visualization
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red);
        }
        else
        {
            // Debug: Show the ray even if it doesn't hit anything
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * raycastRange, Color.green);

            aimTarget = null;
        }

        hudController.SetAimTarget(aimTarget);
    }

    public GameObject GetRaycastedObject() => aimTarget;
    public bool setRaycastEnable(bool set) => raycastEnabled = set;
}
