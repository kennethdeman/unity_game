using UnityEngine;
using System.Collections;

public class Fireball : MonoBehaviour {

    private float m_timer = 0f;
    public float m_End = 10f;

    public GameObject ExplosionParticleSystem;

	// Use this for initialization
	void Start () {
        transform.rotation = Quaternion.LookRotation(rigidbody.velocity);
	}
	
	// Update is called once per frame
	void Update () {
        m_timer += UnityEngine.Time.deltaTime;

        if (m_timer > m_End)
        {
            Destroy(this.gameObject);

        }


        transform.rotation = Quaternion.LookRotation(rigidbody.velocity);
    }

    public void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag != "Projectile")
        {
            
            GameObject particleSystem = Instantiate(ExplosionParticleSystem, transform.position, transform.rotation) as GameObject;
            particleSystem.transform.parent = other.transform;
            Destroy(this.gameObject);
        }
    }
}
