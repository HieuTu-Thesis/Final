using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect4 : MonoBehaviour {
    public bool _isShow;
    public float _delayTime;
    public GameObject _effect;
    public GameObject[] _show;
	public bool EFFECT = false;
	// Use this for initialization
	void Start () {
        _isShow = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!EFFECT)
			return;
		if (_isShow == true)
        {
            StartCoroutine(StartWait(_delayTime));
            _isShow = false;
        }
	}
    IEnumerator StartWait(float time)
    {
        yield return StartCoroutine(Wait(time));
        
        _effect.SetActive(true);
        _show[0].SetActive(true);
        StartCoroutine(EndEffect(25F));
    }
    IEnumerator EndEffect(float time)
    {
        yield return StartCoroutine(Wait(time));
        _show[0].transform.FindChild("ETF_Eternal Light").GetComponent<ParticleSystem>().Stop();

    }

    IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
