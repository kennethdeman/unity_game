using UnityEngine;
using System.Collections;

public class TrollHand : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player" && transform.position.magnitude > 0.8f)
        {
            Debug.LogError("Hit");
            GameObject troll = transform.parent.gameObject;
            troll.GetComponent<TrollController>().DamagePlayer();
        }
    }
}
