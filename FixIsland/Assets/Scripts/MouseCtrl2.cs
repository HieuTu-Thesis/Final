using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCtrl2 : MonoBehaviour
{
    public float minX = -360.0f;
    public float maxX = 360.0f;
    public float minY = -45.0f;
    public float maxY = 45.0f;
    public float sensX = 100.0f;
    public float sensY = 100.0f;
    public static float rotationY = 0.0f;
    public static float rotationX = 0.0f;
    // Use this for initialization
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse X") < 0 || Input.GetAxis("Mouse X") > 0 || Input.GetAxis("Mouse Y") < 0 || Input.GetAxis("Mouse Y") > 0)
        {
            rotationX += Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
            rotationY += Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;
            rotationY = Mathf.Clamp(rotationY, minY, maxY);

            Vector3 rotationVector = transform.rotation.eulerAngles;
            rotationVector = new Vector3(-rotationY, rotationX, 0);
            transform.rotation = Quaternion.Euler(rotationVector);

            Vector3 parentRotation = transform.parent.parent.parent.rotation.eulerAngles;
            parentRotation.y = rotationX;
            transform.parent.parent.parent.rotation = Quaternion.Euler(parentRotation);

            //transform.rotation.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        }
    }
}

