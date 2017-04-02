using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	// Animator of Unity-Chan
	public Animator anim; 
	public Rigidbody rbody;

	// input of change axis, H: horizontal, V: vertical
	private float inputH;
	private float inputV;

	private bool run;

	// Use this for initialization
	void Start () 
	{
		anim = GetComponent<Animator>();
		rbody = GetComponent<Rigidbody>();
		run = false;
	}

	// Update is called once per frame
	void Update () 
	{
		// Press 1, 2, 3, 4 to play idle animation
		if(Input.GetKeyDown("1"))
		{
			anim.Play("WAIT01",-1,0f);
		}
		if(Input.GetKeyDown("2"))
		{
			anim.Play("WAIT02",-1,0f);
		}
		if(Input.GetKeyDown("3"))
		{
			anim.Play("WAIT03",-1,0f);
		}
		if(Input.GetKeyDown("4"))
		{
			anim.Play("WAIT04",-1,0f);
		}

		// A, D to turn around unity-chan
		if(Input.GetKeyDown(KeyCode.A))
		{
			transform.Rotate (new Vector3 (0, 1, 0), 90 * Time.deltaTime);
		}
		if(Input.GetKeyDown(KeyCode.D))
		{
			transform.Rotate (new Vector3 (0, 1, 0), -90 * Time.deltaTime);
		}

		// S, W should not use for turn around camera, use others key instead. A, D, S, W ares the same as arrow keys!!! 
		
		if(Input.GetKey(KeyCode.W))
		{
            Vector3 rotationVector = transform.rotation.eulerAngles;
            rotationVector.y = transform.FindChild("Main Camera").rotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(rotationVector);

            Vector3 v = new Vector3(0, 1, -1.5f);
            transform.FindChild("Main Camera").localPosition = v;
            transform.FindChild("Main Camera").localEulerAngles = new Vector3(transform.FindChild("Main Camera").localEulerAngles.x, 0, 0);
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
        {
            if (run == false) gameObject.GetComponents<AudioSource>()[0].Play();
            else gameObject.GetComponents<AudioSource>()[1].Play();
        }

        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
        {
            gameObject.GetComponents<AudioSource>()[0].Stop();
            gameObject.GetComponents<AudioSource>()[1].Stop();
        }

		// Shift to run
		if(Input.GetKey(KeyCode.LeftShift))
		{
            if (!Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.W))
            {
                run = true;
                if (gameObject.GetComponents<AudioSource>()[0].isPlaying)
                {
                    gameObject.GetComponents<AudioSource>()[0].Stop();
                    gameObject.GetComponents<AudioSource>()[1].Play();
                }
            }else
            {
                //gameObject.GetComponents<AudioSource>()[0].Stop();
                gameObject.GetComponents<AudioSource>()[1].Stop();
            }

        }
		else if (Input.GetKeyUp(KeyCode.LeftShift))
		{
			run = false;
            if (gameObject.GetComponents<AudioSource>()[1].isPlaying)
            {
                gameObject.GetComponents<AudioSource>()[1].Stop();
                gameObject.GetComponents<AudioSource>()[0].Play();
            }
        }

		// Space to jump
		if(Input.GetKey(KeyCode.Space) && !gameObject.GetComponents<AudioSource>()[3].isPlaying)
		{
            gameObject.GetComponents<AudioSource>()[2].Stop();
            gameObject.GetComponents<AudioSource>()[3].Stop();
            anim.SetBool("jump",true);
            gameObject.GetComponents<AudioSource>()[2].Play();
            gameObject.GetComponents<AudioSource>()[3].Play();
        }
		else
		{
			anim.SetBool("jump", false);
		}

		inputH = Input.GetAxis ("Horizontal");
		inputV = Input.GetAxis ("Vertical");

		anim.SetFloat("inputH",inputH);
		anim.SetFloat("inputV",inputV);
		anim.SetBool ("run",run);

		// Convert change axis to move
		float moveX = inputH * 10f * Time.deltaTime;
		float moveZ = inputV * 30f * Time.deltaTime;

		if (moveZ < 0f) { // make move backward slowly
			moveZ /= 2;
			moveX /= 3;
		} else if (moveZ == 0) { 
			moveX = 0; // can not move horizontally
		}
		else if(run) // if run: move faster
		{
			moveX *= -3f;
			moveZ *= -3f;
		}

		// move Unity-chan after handling
		rbody.AddForce (new Vector3 (moveX, 0f, moveZ), ForceMode.VelocityChange);
	}
}