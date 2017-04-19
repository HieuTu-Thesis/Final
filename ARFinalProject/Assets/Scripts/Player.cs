using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public Rigidbody rbody;
    public Animator anim;
    private Vector3 movement;
    // Use this for initialization
    void Start () {
        movement = new Vector3((0.4304f - transform.localPosition.x)/10f, 0f, (-0.1917f - transform.localPosition.z)/10f);
        rbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        if (transform.localPosition.x > 0.4304f)
        {
            if (transform.localEulerAngles.y != 0f)
            transform.localEulerAngles = new Vector3(0, 0, 0);
            rbody.AddForce(movement, ForceMode.VelocityChange);
            anim.SetBool("jump", true);
        }
        else
        {
            if (transform.localEulerAngles.y != 134.942f)
            transform.localEulerAngles = new Vector3(0, 134.942f, 0);
            rbody.AddForce(-movement, ForceMode.VelocityChange);
            anim.Play("WALK00_B", -1, 0F);
        }
    }
}
