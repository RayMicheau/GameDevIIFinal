using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnityFlockController : MonoBehaviour
{
    public float minVelocity = 1; //Min Velocity
    public float maxVelocity = 8; //Max Flock speed
    public int flockSize = 0; //Number of flocks in the group
                               //How far the boids should stick to the center (the more
                               //weight stick closer to the center)
    public float centerWeight = 1;
    public float velocityWeight = 1; //Alignment behavior
                                     //How far each boid should be separated within the flock
    public float separationWeight = 1;
    //How close each boid should follow to the leader (the more
    //weight make the closer follow)
    public float followWeight = 1;
    //Additional Random Noise
    public float randomizeWeight = 1;
    public UnityFlock prefab;
    public Transform target;
    //Center position of the flock in the group
    internal Vector3 flockCenter;
    internal Vector3 flockVelocity; //Average Velocity
    public List<UnityFlock> flockList = new List<UnityFlock>();

    public int ListSize;
    void Start()
    {
        for (int i = 0; i < flockSize; i++)
        {
            UnityFlock flock = Instantiate(prefab, transform.position,
            transform.rotation) as UnityFlock;
            flock.transform.parent = transform;
            flock.controller = this;
            flockList.Add(flock);
        }
        ListSize = flockList.Count;
    }
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.J))
        {
            UnityFlock flock = Instantiate(prefab, transform.position, transform.rotation) as UnityFlock;
            flock.transform.parent = transform;
            flock.controller = this;
            flockList.Add(flock);
            ListSize = flockList.Count;
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            Destroy(flockList[ListSize - 1 ].gameObject);
            flockList.Remove(flockList[ListSize - 1]);
            ListSize = flockList.Count;
        }
        //Calculate the Center and Velocity of the whole flock group
        Vector3 center = Vector3.zero;
        Vector3 velocity = Vector3.zero;
        foreach (UnityFlock flock in flockList)
        {
            Debug.DrawRay(flock.transform.position, target.position / 10, Color.blue);

            center += flock.transform.localPosition;
            velocity += flock.GetComponent<Rigidbody>().velocity;
        }
        flockCenter = center / flockSize;
        flockVelocity = velocity / flockSize;
    }
}