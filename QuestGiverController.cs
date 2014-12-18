using UnityEngine;
using System.Collections;

public class QuestGiverController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void FixedUpdate()
    {   

        GameObject player = GameObject.Find("Knight");

        if (Vector3.Distance(player.transform.position, transform.position) < 10f)
        {

            Vector3 targetDirection = (player.transform.position - transform.position).normalized;
            targetDirection.y = 0;
            targetDirection = targetDirection.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            Quaternion newRotation = Quaternion.Lerp(rigidbody.rotation, targetRotation, 3f * Time.deltaTime);
            rigidbody.MoveRotation(newRotation);
        }

        Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(ray.origin, ray.direction * 10f);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10f) && Input.GetButton("Fire1"))
        {
            if (hit.collider.tag == "QuestGiver")
            {
                //Debug.LogError("Hit");
                GameObject guiObject = GameObject.Find("GUIobject");
                guiObject.GetComponent<GUIHandler>().OpenQuestWindow();
            }
        }

	}
}
