using UnityEngine;
using System.Collections;

public class SelfDestruct : MonoBehaviour {
    private float _timer = 0f;
    public float End = 5f;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        _timer += UnityEngine.Time.deltaTime;

        if (_timer > End)
        {
            Destroy(this.gameObject);

        }
	}
}
