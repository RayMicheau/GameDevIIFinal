using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SimpleFSM : FSM {

    public enum FSMState
    {
        NONE,
        PATROL,
        FLEE,
        ARRIVE,
        CHASE
    }

    //Current NPC state
    public FSMState m_CurrentState;

    //Minimum distance to do obstacle avoidance
    public float minDistanceToAvoid = 5.0f;
    public float force = 350.0f;
    //Text to display the current visible state
    public Text m_StateText;
    //current speed of the enemy unit
    public float m_CurrentSpeed;

    ///Speed at which the enemy will rotate
    [SerializeField]
    private float m_CurrentRotationSpeed;

    [SerializeField]
    private float dist;

    [SerializeField]
    private float fleeTimer = 0.0f;

    public GameObject targetSphere;
    public float fleeTime = 10.0f;

    public float approachRadius = 10.0f;

    public float FollowerCount = 3;
    List<GameObject> FollowerList = new List<GameObject>();
    List<Vector3> FollowerOffsetList = new List<Vector3>();
    Vector3 tmpDest;

    Vector3 dir;
    // Use this for initialization
    protected override void Initialize() {
        FollowerOffsetList.Add(new Vector3(5.0f, 0.0f, 0.0f));
        FollowerOffsetList.Add(new Vector3(-5.0f, 0.0f, 0.0f));
        FollowerOffsetList.Add(new Vector3(-10.0f, 0.0f, 0.0f));
        SpawnFollowers(); 

        //Set the FSMState to be patrolling immdeiately
        m_CurrentState = FSMState.PATROL;

        //set the text to the current state
        m_StateText.text = "State: Patrol";

        //set the speed
        m_CurrentSpeed = 10f;

        //set rotational speed
        m_CurrentRotationSpeed = 5f;

        //Grab every wander point on the map
        m_patrolPointList = GameObject.FindGameObjectsWithTag("WanderPoint");

        //Grab the next point to travel towards
        FindNextPoint();

        ////Find the player object
        //GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        
        //set the transform to the objects transform
        //m_playerTransform = playerObj.transform;

        if (!m_playerTransform)
        {
            Debug.Log("No player exists, please create one with the 'Player' tag");
        }
    }

    protected override void FSMUpdate()
    {

        //Switch set on m_CurrentState to determine which method to call to get the logic
        switch (m_CurrentState)
        {
            case FSMState.PATROL:
                //set the text to the current state
                m_StateText.text = "State: PATROL";

                UpdatePatrolState();
                break;
            case FSMState.CHASE:
                //set the text to the current state
                m_StateText.text = "State: CHASE";

                UpdateChaseState();
                break;
            case FSMState.ARRIVE:
                //set the text to the current state
                m_StateText.text = "State: ARRIVE";

                UpdateArrival();
                break;
            case FSMState.FLEE:
                //set the text to the current state
                m_StateText.text = "State: FLEE";

                UpdateFleeState();
                break;
            case FSMState.NONE: break;
        }
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(transform.position, forward * 2, Color.green);
        UpdateFollowers();


        //Press key to change formation

        //Start in Diamond, press to go to Line
    }

    protected void UpdateFleeState()
    {
        Debug.Log("RUN THE HELL AWAY");
        //find a new point to flee towards
        if(dist <= 5.0f)
            FindNextPoint();

        //Rotate to target
        Quaternion targetRotation = Quaternion.LookRotation(m_destPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * m_CurrentRotationSpeed);

        //GOOOOOOO
        transform.Translate(Vector3.forward * Time.deltaTime * m_CurrentSpeed * 2);
        
        if(Vector3.Distance(transform.position, m_playerTransform.position) >= 15.0f)
        {
            m_CurrentState = FSMState.PATROL;
            FindNextPoint();
        }
    }

    void SpawnFollowers()
    {
        //Create an empty follower game object
        GameObject newFollower;

        //loop through the number of followers to create
        for(int i = 0; i < FollowerCount; i++)
        {
            newFollower = new GameObject();
            //spawn each at a certain position
            if(i == 0)
            {
                newFollower.transform.position = new Vector3(transform.position.x + 5.0f, transform.position.y, transform.position.z);
            }
            else if(i == 1)
            {
                newFollower.transform.position = new Vector3(transform.position.x - 5.0f, transform.position.y, transform.position.z);
            }
            else if(i == 2)
            {
                newFollower.transform.position = new Vector3(transform.position.x - 10.0f, transform.position.y, transform.position.z);
            }
            //give them a mesh filter
            newFollower.AddComponent<MeshFilter>().mesh = GetComponent<MeshFilter>().mesh;
            //make sure they can render
            newFollower.AddComponent<MeshRenderer>();
            
            //add them to a list
            FollowerList.Add(newFollower);
        }
    }


    void UpdateFollowers()
    {
        Transform leaderTransform = transform;
        float distanceFromLead;
        //Update each followers offset to be in position behind the leader
        for(int i = 0; i < FollowerList.Count; i++) { 
            distanceFromLead = Vector3.Distance(FollowerList[i].transform.position, transform.position);

            m_destPos += FollowerAvoidObstacles();

            //Rotate to target
            Quaternion targetRotation = Quaternion.LookRotation(m_destPos + FollowerOffsetList[i] - FollowerList[i].transform.position);

            FollowerList[i].transform.rotation = Quaternion.Slerp(FollowerList[i].transform.rotation, targetRotation, Time.deltaTime * m_CurrentRotationSpeed);

            Vector3 forward = FollowerList[i].transform.TransformDirection(Vector3.forward);
            Debug.DrawRay(FollowerList[i].transform.position, forward * 2, Color.green);
            //Move to the position with the leader
            FollowerList[i].transform.Translate(Vector3.forward * Time.deltaTime * m_CurrentSpeed);
        }
    }

    //Update the logic of the patrol state
    protected void UpdatePatrolState()
    {
        m_destPos = tmpDest;

        //Find another random point to patrol if you reach the current one
        dist = Vector3.Distance(transform.position, m_destPos);
        if (dist <= 5.0f)
        {
            Debug.Log("Calculating next point to travel to");

            FindNextPoint();
        }
        m_destPos += AvoidObstacles();

        //Rotate to target
        Quaternion targetRotation = Quaternion.LookRotation(m_destPos - transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * m_CurrentRotationSpeed);

        //GOOOOOOO
        transform.Translate(Vector3.forward * Time.deltaTime * m_CurrentSpeed);


        //Check distance to player tank
        //if close enough, transition to chase!! O:
        //if (Vector3.Distance(transform.position, m_playerTransform.position) <= 15.0f)
        //{
        //    Debug.Log("CHASE INITIATED");

        //    m_CurrentState = FSMState.CHASE;
        //}

    }

    protected void UpdateChaseState()
    {
        fleeTimer = 0;
        //Set the target position as the player position
        m_destPos = m_playerTransform.position;

        dist = Vector3.Distance(transform.position, m_playerTransform.position);
        if(dist >= 20.0f)
        {
            Debug.Log("NOT CHASING, BACK TO PATROLLING");
            m_CurrentState = FSMState.PATROL;
        }
        if (dist >= approachRadius)
        {
            //Rotate to target
            Quaternion targetRotation = Quaternion.LookRotation(transform.position - m_destPos);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * m_CurrentRotationSpeed);

            //GOOOOOO
            transform.Translate(-Vector3.forward * Time.deltaTime * m_CurrentSpeed);
        }
        else 
        {
            m_CurrentState = FSMState.ARRIVE;
        }

    }

    public Vector3 AvoidObstacles()
    {
        RaycastHit hit;

        //only detect the 8th layer
        int layerMask = 1 << 8;

        //check that the hit with obstacles are within the min distance to avoid
        if (Physics.Raycast(transform.position, transform.forward, out hit, minDistanceToAvoid, layerMask))
        {
            //get the normal of the hit to calculate a new direction
            Vector3 hitNormal = hit.normal;
            hitNormal.y = 0.0f;

            return dir = transform.forward + hitNormal * force;
        }
        return new Vector3();
    }

    public Vector3 FollowerAvoidObstacles()
    {
        RaycastHit hit;

        //only detect the 8th layer
        int layerMask = 1 << 8;
        for (int i = 0; i < FollowerCount; i++)
        {
            //check that the hit with obstacles are within the min distance to avoid
            if (Physics.Raycast(FollowerList[i].transform.position, FollowerList[i].transform.forward, out hit, minDistanceToAvoid, layerMask))
            {
                //get the normal of the hit to calculate a new direction
                Vector3 hitNormal = hit.normal;
                hitNormal.y = 0.0f;

                return dir = FollowerList[i].transform.forward + hitNormal * force;
            }
        }
        return new Vector3();
    }

    protected void UpdateArrival()
    {
        fleeTimer += Time.deltaTime;
        dist = Vector3.Distance(transform.position, m_playerTransform.position);

        if (dist >= approachRadius)
            m_CurrentState = FSMState.CHASE;

        if (fleeTimer >= fleeTime)
        {
            fleeTimer = 0.0f;
            m_CurrentState = FSMState.FLEE;
        }

        //Rotate to target
        Quaternion targetRotation = Quaternion.LookRotation(transform.position - m_destPos);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * m_CurrentRotationSpeed);

        //Decrease the speed the closer it is to the player
        transform.Translate(-Vector3.forward * Time.deltaTime * (dist * m_CurrentSpeed) / approachRadius);

    }

    protected void FindNextPoint()
    {
        //Debug.Log("Finding next point!");
        //int rndIndex = Random.Range(0, m_patrolPointList.Length);
        //float rndRadius = 10.0f;
        //Vector3 rndPosition = Vector3.zero;
        m_destPos = (Random.insideUnitSphere * 80 - transform.position) / 5;
        m_destPos.y = 0.0f;
        tmpDest = m_destPos;
        if (IsInCurrentRange(m_destPos))
        {
            //    rndPosition = new Vector3(Random.Range(-rndRadius, rndRadius), 0.0f, Random.Range(-rndRadius, rndRadius));
            //    m_destPos = m_patrolPointList[rndIndex].transform.position + rndPosition;
            m_destPos = (Random.insideUnitSphere * 80 - transform.position) / 5;
            m_destPos.y = 0.0f;
            tmpDest = m_destPos;
        }
    }   

    protected bool IsInCurrentRange(Vector3 pos)
    {
        float xPos = Mathf.Abs(pos.x - transform.position.x);
        float zPos = Mathf.Abs(pos.z - transform.position.z);

        if (xPos <= 50 && zPos <= 50)
            return true;

        return false;
    }

}
