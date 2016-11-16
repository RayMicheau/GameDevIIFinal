

using UnityEngine;
using System.Collections;

public class PathFollower : MonoBehaviour
{

    public Path path;

    public float speed = 100.0f;

    //Flag to determine if it will continually follow a path
    public bool isLooping = true;

    //float to determine acceleration
    public float mass = 5.0f;

    //Acceleration vector for object
    public Vector3 acceleration;

    //actual speed of the object
    [SerializeField]
    private float curSpeed;

    private int curPathIndex;

    //Length of the path
    /*
     * Stores the number of objects
     * inside of the path.Length variable
     */
    private float pathLength;

    //target point to travel towards
    [SerializeField]
    private Vector3 targetPoint;


    public float m_CurrentRotationSpeed;
    //Velocity of the object
    Vector3 velocity;


    // Use this for initialization
    void Start()
    {
        m_CurrentRotationSpeed = 10;
        pathLength = path.Length;
        curPathIndex = 0;

        //get the current velocity of the object
        velocity = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        curSpeed = speed * Time.deltaTime;

        targetPoint = path.GetPoint(curPathIndex);

        //if you reach the object within the radius, move to the next point in the path
        if (Vector3.Distance(transform.position, targetPoint) < path.Radius)
        {
            //Determine if the path is still going
            if (curPathIndex < pathLength)
            {
                curPathIndex++;
            }
            //Determine if we start the loop over again
            if (isLooping && curPathIndex == pathLength)
            {
                curPathIndex = 0;
            }
        }
        if(curPathIndex > pathLength) { return; }
        /*      
        //Move the vehicle by velocity
        transform.position += velocity;

        //Rotate to the desired velocity
        transform.rotation = Quaternion.LookRotation(velocity);
        */
        Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * m_CurrentRotationSpeed);

        //GOOOOOOO
        transform.Translate(Vector3.forward * curSpeed);
    }

    //Couldn't manage to get this working, going to hammer away at it
    public Vector3 Steer(Vector3 target, bool bFinalPoint = false)
    {
        //Calculate direction from current position to target position

        Vector3 desiredVel = target - transform.position;
        float distance = desiredVel.magnitude;

        //Normalize the desired velocity
        desiredVel.Normalize();

        //Calculate velocity by using speed
        if (bFinalPoint && distance < 10.0f)
        {
            desiredVel *= (curSpeed * (distance / 10.0f));
        }
        else
        {
            desiredVel *= curSpeed;
        }

        Vector3 steeringForce = desiredVel - velocity;
        acceleration = steeringForce / mass;

        return acceleration;
    }
}
