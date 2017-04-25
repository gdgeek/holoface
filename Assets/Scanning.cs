using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scanning : MonoBehaviour {
    public Image _image;
    private float _time;
    public float _from;
    public float _to;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        _time += Time.deltaTime;
        if (_time > 1.0f)
        {
            _time = 0.0f;
        }
        float r = _time;
      
        float end = _from * (1.0f - r) + _to * r;
       var position = _image.transform.localPosition;
        _image.transform.localPosition = new Vector3(position.x, end, position.z);
    }
}
