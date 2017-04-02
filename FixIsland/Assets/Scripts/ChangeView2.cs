using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeView2 : MonoBehaviour
{
    public static int viewMode;
    private GameObject otherCamera;
    // Use this for initialization
    void Start()
    {
        viewMode = 0;
        otherCamera = transform.parent.parent.parent.FindChild("Main Camera").gameObject;
        //this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (viewMode == 0)
            {
                otherCamera.SetActive(true);
                otherCamera.transform.localPosition = new Vector3(0, 1, -1.5f);
                otherCamera.transform.localEulerAngles = new Vector3(0, 0, 0);
                MouseCtrl1.rotationX = MouseCtrl2.rotationX;
                MouseCtrl1.rotationY = 0.0f;
                gameObject.SetActive(false);
                viewMode = 1;
                //Vector3 v = new Vector3(1.5f * Mathf.Sin(-Mathf.PI + transform.rotation.eulerAngles.y * Mathf.PI / 180), 1, 1.5f * Mathf.Cos(-Mathf.PI + transform.rotation.eulerAngles.y * Mathf.PI / 180));
                //transform.position = transform.parent.position + v;
                //viewMode = 1;
            }
            else
            {
                Vector3 v = new Vector3(-0.7f, 0, 0);
                transform.localPosition = v;
                viewMode = 0;
                //viewMode = 0;
                //otherCamera.SetActive(true);
                //gameObject.SetActive(false);
            }
        }
    }
}
