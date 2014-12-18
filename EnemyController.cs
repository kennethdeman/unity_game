using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{

    public float m_MoveSpeed = 4f;

    public Rigidbody m_Projectile;
    public float m_ProjectileSpeed = 300f;
    public float Interval = 2f;

    private float m_health = 100f;

    private float m_currentSpeed;
    private Animator m_anim;

    private bool m_dead = false;

    private float m_projectileTimer = 10f;
    private float m_castTimer = 0f;
    private float m_holdTimer = 0f;
    private const float HOLDTIME = 0.3f;

    private List<Item> m_loot = new List<Item>();

    private const string NAME = "Mage Knight";

    private bool m_seenPlayer = false;

    // Use this for initialization
    void Awake()
    {
        m_anim = GetComponent<Animator>();

        float random = Random.Range(0.0f, 3.0f);

        if (random > 2.0f)
        {
            AddRandomPotion();
            AddRandomPotion();
            AddRandomPotion();
        }
        else if (random > 1.0f)
        {
            AddRandomPotion();
            AddRandomPotion();
        }
        else
        {
            AddRandomPotion();
        }

    }

    void AddRandomPotion()
    {
        if (Random.Range(-10.0f, 10.0f) < 0f)
        {
            m_loot.Add((Item)new HealthPotion());
        }
        else
        {
            m_loot.Add((Item)new MagicPotion());
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 rayStart = transform.position - Vector3.down * 0.3f;
        Vector3 ray = Vector3.down * 0.6f;
        //Debug.DrawRay(rayStart, ray);

        RaycastHit hit;

        if (!m_dead)
        {
            GameObject player = GameObject.Find("Knight");

            Vector3 headPosition = transform.position - Vector3.down * 2.1f;
            Vector3 rayDirection = player.transform.position - transform.position;

            //Debug.DrawRay(headPosition, rayDirection);

            bool inLineOfSight = true;

            if (Physics.Raycast(headPosition, rayDirection, out hit, rayDirection.magnitude))
            {
                //Debug.LogError(hit.collider.tag);
                inLineOfSight = false;
                //if (hit.collider.tag == "Player")
                //    inLineOfSight = true;
            }

            if (inLineOfSight) m_seenPlayer = true;

            if (m_seenPlayer)
            {

                bool moving = MovementManagement();


                if (Physics.Raycast(rayStart, ray, out hit, ray.magnitude))
                {
                    //Debug.LogError(Mathf.Abs(rigidbody.velocity.y));
                    if (!moving && Mathf.Abs(rigidbody.velocity.y) < 0.3f)
                    {
                        rigidbody.velocity += -rigidbody.velocity * 2f;
                        //rigidbody.velocity = Vector3.zero;
                    }
                }



                //Magic
                m_projectileTimer += UnityEngine.Time.deltaTime;



                if (m_projectileTimer > Interval && inLineOfSight && Vector3.Distance(player.transform.position, transform.position) < 30f)
                {
                    m_castTimer += 1 * Time.deltaTime;
                    m_anim.SetBool("Cast", true);
                    if (m_castTimer > 0.6f)
                    {
                        CastMagic();
                        m_holdTimer = HOLDTIME;
                        //m_castTimer = 0f;
                        //m_anim.SetBool("Cast", false);
                    }
                }
                else if (m_holdTimer <= 0f)
                {
                    m_castTimer = 0f;
                    m_anim.SetBool("Cast", false);
                }
                else
                {
                    m_castTimer = 0f;
                    m_holdTimer -= 1 * Time.deltaTime;
                }
            }

            if (m_health <= 0f)
            {
                m_anim.SetBool("Die", true);
                m_dead = true;
            }


        }
        else
        {
            Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            Ray cameraRay = camera.ScreenPointToRay(Input.mousePosition);

            //Debug.DrawRay(cameraRay.origin, cameraRay.direction * 10f);


            if (Physics.Raycast(cameraRay, out hit, 10f) && Input.GetButton("Fire1"))
            {
                if (hit.collider == this.collider)
                {
                    //Debug.LogError("Hit");
                    GameObject guiObject = GameObject.Find("GUIobject");
                    guiObject.GetComponent<GUIHandler>().OpenLootWindow(m_loot);
                }
            }

            if (Physics.Raycast(rayStart, ray, out hit, ray.magnitude))
            {
                //Debug.LogError(m_dead.ToString());
                if (m_dead)
                {
                    collider.isTrigger = true;
                    rigidbody.isKinematic = true;
                }
            }



        }


        if (m_health > 100f) m_health = 100f;
        if (m_health < 0f) m_health = 0f;

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

            if (Vector3.Distance(player.transform.position, transform.position) > 10f)
            {
                m_currentSpeed = m_MoveSpeed;
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

        if (other.gameObject.tag == "m_Projectile" && !m_dead)
        {
            m_health -= 65f;
            if (m_health > 100f) m_health = 100f;
            if (m_health < 0f) m_health = 0f;
            GameObject guiObject = GameObject.Find("GUIobject");
            guiObject.GetComponent<GUIHandler>().SetTargetHealthBar(m_health, NAME);
        }
    }

    void CastMagic()
    {

        //m_projectileTimer += UnityEngine.Time.deltaTime;

        GameObject player = GameObject.Find("Knight");

        m_projectileTimer = 0;

        Vector3 targetDirection = (player.transform.position - transform.position).normalized;


        Rigidbody clone;
        Vector3 projectilePos = transform.position;
        projectilePos += transform.forward * 1.5f + transform.up * 1.6f + transform.right * 0.3f;

        clone = Instantiate(m_Projectile, projectilePos, transform.rotation) as Rigidbody;
        clone.GetComponent<Rigidbody>().velocity = targetDirection * m_ProjectileSpeed * UnityEngine.Time.deltaTime;



    }
}
