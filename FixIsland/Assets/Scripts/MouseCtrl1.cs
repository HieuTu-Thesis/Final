using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCtrl1 : MonoBehaviour
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
            transform.localEulerAngles = new Vector3(-rotationY, rotationX - transform.parent.rotation.eulerAngles.y, 0);
            //transform.localEulerAngles = new Vector3(-rotationY, 0, 0);
            if (Input.GetAxis("Mouse X") < 0 || Input.GetAxis("Mouse X") > 0)
            {
                
                //Vector3 rotationVector = transform.parent.rotation.eulerAngles;
                //rotationVector.y = rotationX;
                //transform.parent.rotation = Quaternion.Euler(rotationVector);
                Vector3 v = new Vector3(1.5f * Mathf.Sin(-Mathf.PI + (transform.localEulerAngles.y) * Mathf.PI / 180), 1, 1.5f * Mathf.Cos(-Mathf.PI + (transform.localEulerAngles.y) * Mathf.PI / 180));
                Debug.Log(Input.GetAxis("Mouse X"));
                transform.localPosition = v;
            }
        }
    }
}

