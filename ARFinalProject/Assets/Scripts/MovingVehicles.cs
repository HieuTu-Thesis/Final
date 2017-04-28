using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingVehicles : MonoBehaviour {
    public int _startAngles;
    private int _angles;
    public int _k;
    public Vector3 _point1;
    public Vector3 _point2;
    public Vector3 _point3;
    public Vector3 _point4;
    public float _localX1;
    public float _localX2;
    public float _localZ1;
    public float _localZ2;
    private int speed;
    private int originSpeed;
	// Use this for initialization
	void Start () {
        speed = 100;
        originSpeed = speed;
        _angles = _startAngles;
        //k = 0;
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(transform.localEulerAngles.y);
        if (transform.localEulerAngles.y >= 358)
        {
            //_angles = 0;
            //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0, transform.localEulerAngles.z);
            //Debug.Log("Blah");
        }
        if (transform.localEulerAngles.y >= _angles + 88)
        {
            _k++;
            _angles += 90;
            if (_angles >= 360)
            {
                _angles = 0;
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0, transform.localEulerAngles.z);
            }
            if (_k == 4) _k = 0;
           
        }

        //if (transform.localEulerAngles.y >= 360) transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0, transform.localEulerAngles.z); ;

        if (_k == 0)
        {
            if (transform.localPosition.x <= _localX1)
            {
                transform.RotateAround(new Vector3(_point1.x, _point1.y, _point1.z), Vector3.up, 80 * Time.deltaTime);
                speed = 0;
                //k++;
                //Debug.Log("AAK");
            }
            //Debug.Log("AA");
            transform.localPosition = new Vector3(transform.localPosition.x - speed, transform.localPosition.y, transform.localPosition.z);
        }
        else if (_k == 1)
        {
            if (transform.localPosition.z >= _localZ1)
            {
                transform.RotateAround(new Vector3(_point2.x, _point2.y, _point2.z), Vector3.up, 80 * Time.deltaTime);
                speed = 0;
               
            }
           
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + speed);
            //Debug.Log("BB");
        }
        else if (_k == 2)
        {
            if (transform.localPosition.x >= _localX2)
            {
                transform.RotateAround(new Vector3(_point3.x, _point3.y, _point3.z), Vector3.up, 80 * Time.deltaTime);
                speed = 0;
              
            }
           
            transform.localPosition = new Vector3(transform.localPosition.x + speed, transform.localPosition.y, transform.localPosition.z);
           // Debug.Log("CC");
        }
        else if (_k == 3)
        {
            //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, startAngles - 90, transform.localEulerAngles.z);
            if (transform.localPosition.z <= _localZ2)
            {
                transform.RotateAround(new Vector3(_point4.x, _point4.y, _point4.z), Vector3.up, 80 * Time.deltaTime);
                speed = 0;
            }
           
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - speed);
           //   Debug.Log("DD");
        }
        speed = originSpeed;
	}
}
