using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrollController : MonoBehaviour {

    public float m_moveSpeed = 4f;

    private float m_Health = 100f;

    private float m_currentSpeed;
    private Animator m_anim;

    private bool m_Dead = false;
    private bool m_CheckedIfDead = false;

    private float m_DamageInterval = 0.5f;
    private float m_DamageTimer = 0f;
    private bool m_RecentlyDamagedPlayer = false;

    private List<Item> m_Loot = new List<Item>();

    private const string NAME = "Troll";

	// Use this for initialization
    void Awake()
    {
        m_anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () 
    {
        Vector3 rayStart = transform.position - Vector3.down * 0.3f;
        Vector3 ray = Vector3.down * 0.6f;
        Debug.DrawRay(rayStart, ray);

        

        if (!m_Dead)
        {
            GameObject player = GameObject.Find("Knight");
            bool moving = MovementManagement();

            var LeftHand = transform.Find("Hips/Hips 1/Spine 01/Spine 02/Right Clavicle/Right Upper Arm/Right Lower Arm/Bone018(mirrored)/Right Hand");
            var LeftHandCollision = transform.Find("HandCollision");

            LeftHandCollision.position = LeftHand.position;

            RaycastHit hit;
            if (Physics.Raycast(rayStart, ray, out hit, ray.magnitude))
            {
                //Debug.LogError(Mathf.Abs(rigidbody.velocity.y));
                if (!moving && Mathf.Abs(rigidbody.velocity.y) < 0.3f)
                {
                    rigidbody.velocity += -rigidbody.velocity * 2f;
                    //rigidbody.velocity = Vector3.zero;
                }
            }

            if (m_RecentlyDamagedPlayer)
            {
                m_DamageTimer += Time.deltaTime;

                if (m_DamageTimer > m_DamageInterval)
                {
                    m_DamageTimer = 0f;
                    m_RecentlyDamagedPlayer = false;
                }
            }
            else
            {
                m_DamageTimer = 0f;
            }

            if (Vector3.Distance(player.transform.position, transform.position) < 2f)
            {            
                m_anim.SetBool("Attack", true);
            }
            else
            {
                m_anim.SetBool("Attack", false);
            }

            if (m_Health <= 0f)
            {
                m_anim.SetBool("Die", true);
                m_Dead = true;
            }

        }
        else
        {
            Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            Ray cameraRay = camera.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(cameraRay.origin, cameraRay.direction * 10f);

            RaycastHit hit;
            if (Physics.Raycast(cameraRay, out hit, 10f) && Input.GetButton("Fire1"))
            {
                if (hit.collider == this.collider)
                {
                    GameObject guiObject = GameObject.Find("GUIobject");
                    guiObject.GetComponent<GUIHandler>().OpenLootWindow(m_Loot);
                }
            }

            if (Physics.Raycast(rayStart, ray, out hit, ray.magnitude))
            {
                if (m_Dead)
                {
                    collider.isTrigger = true;
                    rigidbody.isKinematic = true;
                }
            }



        }

        if (m_Health > 100f) m_Health = 100f;
        if (m_Health < 0f) m_Health = 0f;

	}

    bool MovementManagement()
    {
        GameObject player = GameObject.Find("Knight");
        if (Vector3.Distance(player.transform.position, transform.position) < 30f)
        {
            Vector3 targetDirection = (player.transform.position - transform.position).normalized;
            targetDirection.y = 0;
            targetDirection = targetDirection.normalized;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            Quaternion newRotation = Quaternion.Lerp(rigidbody.rotation, targetRotation, 15f * Time.deltaTime);
            rigidbody.MoveRotation(newRotation);

            if (Vector3.Distance(player.transform.position, transform.position) > 2f)
            {
                m_currentSpeed = m_moveSpeed;
                m_anim.SetFloat("Speed", m_currentSpeed);
                rigidbody.velocity = transform.forward * m_currentSpeed + new Vector3(0, rigidbody.velocity.y, 0);
                return true;
            }
            else
            {
                m_currentSpeed = 0f;
            }
        }
        else
        {

            m_currentSpeed = 0f;
        }

        m_anim.SetFloat("Speed", m_currentSpeed);
        return false;
    }

    public void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Projectile" && !m_Dead)
        {
            m_Health -= 35f;
            if (m_Health > 100f) m_Health = 100f;
            if (m_Health < 0f) m_Health = 0f;
            GameObject guiObject = GameObject.Find("GUIobject");
            guiObject.GetComponent<GUIHandler>().SetTargetHealthBar(m_Health, NAME);
        }
    }

    public bool CheckIfDead()
    {
        //Debug.LogError("checked");
        if (!m_CheckedIfDead && m_Dead)
        {
            m_CheckedIfDead = true;
            return true;
        }

        return false;
    }

    public void DamagePlayer()
    {
        GameObject player = GameObject.Find("Knight");
        player.GetComponent<PlayerController>().TakeDamage(12f, transform.forward * 10.5f);
    }
}
