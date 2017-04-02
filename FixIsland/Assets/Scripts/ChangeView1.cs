using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeView1 : MonoBehaviour
{
    public static int viewMode;
    private GameObject otherCamera;
    // Use this for initialization
    void Start()
    {
        viewMode = 0;
        otherCamera = transform.parent.FindChild("Character1_Reference").FindChild("Character1_Hips").FindChild("Head Camera").gameObject;
        otherCamera.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (viewMode == 0)
            {
                otherCamera.SetActive(true);
                gameObject.SetActive(false);
                viewMode = 1;
                //Vector3 v = new Vector3(0, 1.5f, 0);
                //transform.position = transform.parent.position + v;
                //viewMode = 0;
            }
            else
            {
                //viewMode = 1;
                //Vector3 v = new Vector3(1.5f * Mathf.Sin(-Mathf.PI + transform.rotation.eulerAngles.y * Mathf.PI / 180), 1, 1.5f * Mathf.Cos(-Mathf.PI + transform.rotation.eulerAngles.y * Mathf.PI / 180));
                //transform.localPosition = new Vector3(0, 1, -1.5f);
                //transform.localEulerAngles = new Vector3(0, 0, 0);
                viewMode = 0;
                //transform.parent.parent.FindChild("Main Camera").gameObject.SetActive(true);
                //gameObject.SetActive(false);
            }
        }
    }
}