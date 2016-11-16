using UnityEngine;
using System.Collections;

public class Avoidance : MonoBehaviour {

    public float speed = 10.0f;
    public float mass = 5.0f;
    public float force = 50.0f;
    public float minDistanceToAvoid = 10f;

    private float curSpeed;
    [SerializeField]
    private Vector3 targetPoint;

	// Use this for initialization
	void Start () {
        mass = 5.0f;
        targetPoint = Vector3.zero;
	}
	
    void OnGUI()
    {
        GUILayout.Label("Click anywhere to move");
    }

	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit, 100.0f))
        {
            targetPoint = hit.point;
        }

        //Direction vector to target position
        Vector3 dir = targetPoint - transform.position;

        dir.Normalize();

        AvoidObstacles(ref dir);

        if (Vector3.Distance(targetPoint, transform.position) < 3.0f)
            return;

        curSpeed = speed * Time.deltaTime;

        Quaternion rotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 5.0f * Time.deltaTime);
        transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);

    }

    public void AvoidObstacles(ref Vector3 dir)
    {
        RaycastHit hit;

        //only detect the 8th layer
        int layerMask = 1 << 8;

        //check that the hit with obstacles are within the min distance to avoid
        if(Physics.Raycast(transform.position, transform.forward, out hit, minDistanceToAvoid, layerMask))
        {
            //get the normal of the hit to calculate a new direction
            Vector3 hitNormal = hit.normal;
            hitNormal.y = 0.0f;

            dir = transform.forward + hitNormal * force;
        }
    }
}
