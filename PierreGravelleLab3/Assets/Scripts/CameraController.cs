using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Constant reference to mouse position as a screen point
    // Check if that reference is on the left, right, top, or down
    //  and maybe diagonal.
    Vector3 mouseScreenPos;
    float cameraSpeed = 20f;
    float scrollSpeed = 5f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 0)
        {
            mouseScreenPos = GetComponent<Camera>().ScreenToViewportPoint(Input.mousePosition);

            if (mouseScreenPos.x < 0.01)
            {
                transform.Translate(Vector3.left * Time.deltaTime * cameraSpeed);
            }
            else if (mouseScreenPos.x > 0.99)
            {
                transform.Translate(Vector3.right * Time.deltaTime * cameraSpeed);
            }

            if (mouseScreenPos.y < 0.01)
            {
                transform.Translate(Vector3.down * Time.deltaTime * cameraSpeed);
            }
            else if (mouseScreenPos.y > 0.99)
            {
                transform.Translate(Vector3.up * Time.deltaTime * cameraSpeed);
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                transform.Translate(Vector3.forward * scrollSpeed);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                transform.Translate(Vector3.back * scrollSpeed);
            }
        }
    }
}
