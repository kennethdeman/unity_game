using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public float m_Speed = 6f;
    private float m_moveSpeed = 0f;

    public Rigidbody Projectile;
    public float ProjectileSpeed = 300f;
    public float Interval = 0.5f;

    private float m_currentSpeed;
    private Animator m_anim;

    private float m_projectileTimer = 10f;
    private float m_castTimer = 0f;
    private float m_holdTimer = 0f;
    private const float HOLDTIME = 0.3f;

    private float m_Health = 100f;
    private float m_MagicPower = 100f;

    public float m_MagicRegenerationRate = 1f;

    const float FIREBALLCOST = 20f;

    bool m_Dead;

    public List<Item> m_ItemsInInventory = new List<Item>();
    public Quest m_ActiveQuest;

    private float m_RestartAfterDeadTimer = 0f;

    public GameObject DrinkHealthPotionParticleSystem;
    public GameObject DrinkMagicPotionParticleSystem;

    // Use this for initialization
    void Awake()
    {
        m_moveSpeed = m_Speed;
        m_anim = GetComponent<Animator>();

        m_ItemsInInventory.Add((Item)new HealthPotion());
        m_ItemsInInventory.Add((Item)new MagicPotion());

        //m_ActiveQuest = new QuestKillTrolls();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        bool moving = false;
        if (!m_Dead)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            moving = MovementManagement(h, v);






            m_projectileTimer += UnityEngine.Time.deltaTime;

            m_MagicPower += m_MagicRegenerationRate * UnityEngine.Time.deltaTime;



            if (m_projectileTimer > Interval && m_MagicPower >= FIREBALLCOST && Input.GetButton("Fire1"))
            {
                m_castTimer += 1 * Time.deltaTime;
                m_anim.SetBool("Cast", true);

                m_moveSpeed = m_Speed / 2;


                if (m_castTimer > 0.6f)
                {
                    CastMagic();
                    m_holdTimer = HOLDTIME;
                }
            }
            else if (m_holdTimer <= 0f)
            {
                m_castTimer = 0f;
                m_anim.SetBool("Cast", false);

                m_moveSpeed = m_Speed;
            }
            else
            {
                m_castTimer = 0f;
                m_holdTimer -= 1 * Time.deltaTime;
            }
        }
        else
        {
            m_RestartAfterDeadTimer += Time.deltaTime;
            if (m_RestartAfterDeadTimer > 5.0f)
            {
                Application.LoadLevel("MainGame");
            }
        }

        //Extra friction
        Vector3 rayStart = transform.position;
        Vector3 ray = Vector3.down * 0.3f;
        Debug.DrawRay(rayStart, ray);

        RaycastHit hit;
        if (Physics.Raycast(rayStart, ray, out hit, ray.magnitude))
        {
            //Debug.LogError(Mathf.Abs(rigidbody.velocity.y));
            if (!moving && Mathf.Abs(rigidbody.velocity.y) < 0.3f)
            {
                rigidbody.velocity += -rigidbody.velocity * 2f;
            }
        }

        if (m_Health <= 0f)
        {

            m_anim.SetBool("Die", true);
            m_Dead = true;
        }

        if (m_Health > 100f) m_Health = 100f;
        if (m_Health < 0f) m_Health = 0f;

        if (m_MagicPower > 100f) m_MagicPower = 100f;
        if (m_MagicPower < 0f) m_MagicPower = 0f;

        //Check quest
        if (m_ActiveQuest!=null)
            m_ActiveQuest.CheckObjectives();
    }

    bool MovementManagement(float horizontal, float vertical)
    {
        if ((horizontal != 0 || vertical != 0))
        {
            m_currentSpeed = m_moveSpeed;
            Moving(horizontal, vertical);
            m_anim.SetFloat("Speed", m_currentSpeed);
            //Debug.LogError(horizontal + " || " + vertical);
            return true;
        }
        else
        {
            m_currentSpeed = 0f;
            m_anim.SetFloat("Speed", m_currentSpeed);
            return false;
        }
    }

    void Moving(float horizontal, float vertical)
    {
        GameObject camera = GameObject.Find("Main Camera");

        var yVelocity = rigidbody.velocity.y;

        Vector3 targetDirection = new Vector3(horizontal, 0f, vertical);
        targetDirection = camera.transform.forward.normalized * vertical;
        targetDirection += camera.transform.right.normalized * horizontal;
        targetDirection.y = 0;
        targetDirection = targetDirection.normalized;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        Quaternion newRotation = Quaternion.Lerp(rigidbody.rotation, targetRotation, 15f * Time.deltaTime);
        rigidbody.MoveRotation(newRotation);
        rigidbody.velocity = transform.forward * m_currentSpeed + new Vector3(0, yVelocity, 0);
    }

    public void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Projectile")
        {
            m_Health -= 10f;
        }
    }

    void CastMagic()
    {

        m_moveSpeed = m_Speed;

        GameObject camera = GameObject.Find("Main Camera");

        m_projectileTimer = 0;

        m_MagicPower -= FIREBALLCOST;


        Rigidbody clone;
        Vector3 projectilePos = transform.position;
        projectilePos += transform.forward * 1.5f + transform.up * 1.6f + transform.right * 0.3f;

        clone = Instantiate(Projectile, projectilePos, transform.rotation) as Rigidbody;
        clone.GetComponent<Rigidbody>().velocity = camera.transform.forward * ProjectileSpeed * UnityEngine.Time.deltaTime
           + new Vector3(0, 3f, 0);

    }

    public float GetHealth()
    {
        if (m_Health > 100f) m_Health = 100f;
        if (m_Health < 0f) m_Health = 0f;
        return m_Health;
    }

    public float GetMagicPower()
    {
        if (m_MagicPower > 100f) m_MagicPower = 100f;
        if (m_MagicPower < 0f) m_MagicPower = 0f;
        return m_MagicPower;
    }

    public void AddHealth(float amount)
    {
        m_Health += amount;
    }

    public void AddMagicPower(float amount)
    {
        m_MagicPower += amount;
    }

    public bool IsDead()
    {
        return m_Dead;
    }

    public void TakeDamage(float damage, Vector3 impactForce)
    {
        m_Health -= damage;
        rigidbody.AddForce(impactForce*100f);
    }

    public void SpawnHealthPotionParticles()
    {
        GameObject particleSystem = Instantiate(DrinkHealthPotionParticleSystem, transform.position, Quaternion.LookRotation(Vector3.up)) as GameObject;
        particleSystem.transform.parent = this.transform;
    }

    public void SpawnMagicPotionParticles()
    {
        GameObject particleSystem = Instantiate(DrinkMagicPotionParticleSystem, transform.position, Quaternion.LookRotation(Vector3.up)) as GameObject;
        particleSystem.transform.parent = this.transform;
    }
}
