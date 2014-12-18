using UnityEngine;

using System.Collections;


public class FreeCamera : MonoBehaviour
{

    public Transform Target;

    public float m_Distance = 5.0f;
    public float m_SpeedX = 250.0f;
    public float m_SpeedY = 120.0f;

    public float m_MinLimitY = -20.0f;
    public float m_MaxLimitY = 80.0f;

    public float m_FrameWidth = 2.0f;
    public float m_FrameHeight = 1.4f;

    private float m_x;
    private float m_y;

    private float m_rayStartHeight = 2f;

    private bool m_previouslyCollided = false;
    private float m_previouslyCollidedTimer = 0f;
    private float m_previousRayLength = 0f;

    private Quaternion m_previousRotation = Quaternion.identity;



    void Awake()
    {

        Vector3 angles = transform.eulerAngles;

        m_x = angles.x;

        m_y = angles.y;



        if (GetComponent<Rigidbody>() != null)
        {

            rigidbody.freezeRotation = true;

        }
        
    }



    void LateUpdate()
    {

        if (Target != null)
        {
            Quaternion rotation = Quaternion.identity;
            if (Input.GetButton("Fire2"))
            {
                m_x += (float)(Input.GetAxis("Mouse X") * m_SpeedX * 0.02f);

                m_y -= (float)(Input.GetAxis("Mouse Y") * m_SpeedY * 0.02f);

                m_y = ClampAngle(m_y, m_MinLimitY, m_MaxLimitY);

                rotation = Quaternion.Euler(m_y, m_x, 0);
            }
            else
            {
                rotation = m_previousRotation;
            }

            m_previousRotation = rotation;

            Vector3 desiredPosition = rotation * (new Vector3(0.0f, 0.0f, -m_Distance)) + Target.position;


            Vector3 rayStart = Target.transform.position + Vector3.up * m_rayStartHeight;
            Vector3 ray = desiredPosition - rayStart;
            Debug.DrawRay(rayStart, ray);

            RaycastHit hit;
            if (Physics.Raycast(rayStart, ray, out hit, ray.magnitude))
            {
                if (hit.collider.tag != "Enemy" && hit.collider.tag != "Troll")
                {
                    Vector3 cameraRay = (hit.point - ray.normalized * 0.5f) - rayStart;

                    ray = cameraRay;

                    Debug.DrawRay(rayStart, cameraRay);

                    DrawDebugPoint(hit.point);
                    DrawDebugPoint(desiredPosition);

                    //Debug.LogError("collided");

                    if (!m_previouslyCollided && ray.magnitude > m_previousRayLength)
                    {
                        m_previousRayLength = cameraRay.magnitude;
                        m_previouslyCollided = true;
                        m_previouslyCollidedTimer = 0;
                        //Debug.LogError("Previously collided set to true");
                    }
                }
            }

            Vector3 extraRay = desiredPosition - rayStart;

           
            for (int i=0; i<4; ++i)
            {
                Vector3 frameOffset = new Vector3(0,0,0);
                if (i == 0)
                {
                    frameOffset = transform.right * -m_FrameWidth / 2 + transform.up * m_FrameHeight / 2;
                }
                else if (i == 1)
                {
                    frameOffset = transform.right * m_FrameWidth / 2 + transform.up * m_FrameHeight / 2;
                }
                else if (i == 2)
                {
                    frameOffset = transform.right * -m_FrameWidth / 2 + transform.up * -m_FrameHeight / 2;
                }
                else if (i == 3)
                {
                    frameOffset = transform.right * m_FrameWidth / 2 + transform.up * -m_FrameHeight / 2;
                }

                extraRay = (desiredPosition + frameOffset) - rayStart;

                Debug.DrawRay(rayStart, extraRay);


                RaycastHit hit2;
                if (Physics.Raycast(rayStart, extraRay, out hit2, extraRay.magnitude))
                {
                    if (hit2.collider.tag != "Enemy" && hit2.collider.tag != "Troll")
                    {
                        Vector3 cameraRay = (hit2.point - extraRay.normalized * 0.5f) - rayStart;

                        extraRay = cameraRay;

                        Debug.DrawRay(rayStart, cameraRay);

                        DrawDebugPoint(hit2.point);
                        DrawDebugPoint(desiredPosition);
                    }
                }

                if (ray.magnitude > extraRay.magnitude)
                {
                    ray = ray.normalized * (extraRay - frameOffset).magnitude;
                }
            }

            Debug.DrawRay(rayStart, ray);

            if (m_previouslyCollided)
            {
                m_previouslyCollidedTimer += Time.deltaTime;
                //Debug.LogError("Previously collided timer running");
            }

            if (m_previouslyCollidedTimer > 0.1f)
            {
                //Debug.LogError("Previously collided set to false");
                m_previouslyCollided = false;
            }

            if (m_previouslyCollided && ray.magnitude > m_previousRayLength)
            {
                ray = ray.normalized * m_previousRayLength;
                //Debug.LogError("Shortening camera ray");
            }

            desiredPosition = rayStart + ray;


            transform.rotation = Quaternion.LookRotation(-ray);
            transform.position = desiredPosition;


        }

    }



    private float ClampAngle(float angle, float min, float max)
    {

        if (angle < -360)
        {

            angle += 360;

        }

        if (angle > 360)
        {

            angle -= 360;

        }

        return Mathf.Clamp(angle, min, max);

    }

    void DrawDebugPoint(Vector3 point)
    {
        Debug.DrawRay(point, Vector3.up * 0.5f);
        Debug.DrawRay(point, Vector3.right * 0.5f);
        Debug.DrawRay(point, Vector3.left * 0.5f);
        Debug.DrawRay(point, Vector3.down * 0.5f);
        Debug.DrawRay(point, Vector3.forward * 0.5f);
        Debug.DrawRay(point, Vector3.back * 0.5f);
    }


}
