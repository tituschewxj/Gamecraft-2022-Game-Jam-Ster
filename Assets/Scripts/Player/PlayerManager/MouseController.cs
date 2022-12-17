using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    Vector2 mousePosition;
    Transform mouseTransform;
    Camera cameraObject;
    // Start is called before the first frame update
    void Start()
    {
        cameraObject = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        if (cameraObject == null) Debug.LogWarning("MouseController: camera not found");
        mouseTransform = gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMouseCoordinates();
        UpdateMouseTransform();
    }
    void UpdateMouseCoordinates() {
        /*
        Updates the mouse coordinates in screen space
        to the mouse coordinates in world space
        for perspective camera
        */
        // Ray ray = cameraObject.ScreenPointToRay(Input.mousePosition);
        // Plane xy = new Plane(Vector3.forward, new Vector3 (0, 0, -5));
        // float distance;
        // xy.Raycast(ray, out distance);
        // mousePosition = ray.GetPoint(distance);
        mousePosition = cameraObject.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
    }
    void UpdateMouseTransform() {
        mouseTransform.position = (Vector2)mousePosition;
    }
}
