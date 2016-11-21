using UnityEngine;


public class UnityFlock : MonoBehaviour
{
    public Vector3 curVelocity;
    public float healthValue = 1;
    void Start()
    {
        int randColor = Random.Range(0, 10);
        switch(randColor)
        {
            case 1:
                gameObject.GetComponent<Renderer>().material.color = Random.ColorHSV(0.75f, 0.85f, 0.9f, 1, 0.9f, 1); // PURPAL
                break;
            case 2:
                gameObject.GetComponent<Renderer>().material.color = Color.black;
                break;
            case 3:
                gameObject.GetComponent<Renderer>().material.color = Color.blue;
                break;
            case 4:
                gameObject.GetComponent<Renderer>().material.color = Color.gray;
                break;
            case 5:
                gameObject.GetComponent<Renderer>().material.color = Color.green;
                break;
            case 6:
                gameObject.GetComponent<Renderer>().material.color = Color.grey;
                break;
            case 7:
                gameObject.GetComponent<Renderer>().material.color = Color.magenta;
                break;
            case 8:
                gameObject.GetComponent<Renderer>().material.color = Color.red;
                break;
            case 9:
                gameObject.GetComponent<Renderer>().material.color = Color.white;
                break;
            case 10:
                gameObject.GetComponent<Renderer>().material.color = Color.yellow;
                break;

            default:
                break;
        }
    }

    internal UnityFlockController controller;

    void Update()
    {

       if (controller)
        {
            Vector3 relativePos = steer() * Time.deltaTime;
            if (relativePos != Vector3.zero)
                GetComponent<Rigidbody>().AddForce(relativePos);

            // enforce minimum and maximum speeds for the boids
            float speed = GetComponent<Rigidbody>().velocity.magnitude;
            if (speed > controller.maxVelocity)
            {
                GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * controller.maxVelocity;
            }
            else if (speed < controller.minVelocity)
            {
                GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * controller.minVelocity;
            }
        }
        //Slowly fade the color of the agent
        Color c = gameObject.GetComponent<Renderer>().material.color;
        gameObject.GetComponent<Renderer>().material.color = Color.Lerp(c, Random.ColorHSV(Random.Range(0.0f, 1.0f), 
            Random.Range(0.0f, 1.0f), 
            Random.Range(0.0f, 1.0f), 
            Random.Range(0.0f, 1.0f), 0.9f, 1), 1f);

        curVelocity = GetComponent<Rigidbody>().velocity;
    }

    void KillBorb()
    {
        controller.ListSize = controller.flockList.Count - 1;
        controller.flockList.Remove(this);
        Destroy(this);
    }

    public void TakeDamage(float damage)
    {
        healthValue -= damage;
        if(healthValue <= 0)
        {
            KillBorb();
        }
    }

    private Vector3 steer()
    {
        //Cohesion
        Vector3 center = controller.flockCenter -
        transform.localPosition;

        //Alignment
        Vector3 velocity = controller.flockVelocity -
        GetComponent<Rigidbody>().velocity;

        //Follow the leader
        Vector3 follow = controller.target.localPosition -
        transform.localPosition; 

        //Separation
        Vector3 separation = Vector3.zero;

        foreach (UnityFlock flock in controller.flockList)
        {
            
            if (flock != this)
            {
                Vector3 relativePos = transform.localPosition -
                flock.transform.localPosition;
                separation += relativePos / (relativePos.sqrMagnitude);
            }
        }

        //Randomization
        Vector3 randomize = new Vector3((Random.value * 2) - 1,
        (Random.value * 2) - 1, (Random.value * 2) - 1);
        randomize.Normalize();

        return (controller.centerWeight * center +
        controller.velocityWeight * velocity +
        controller.separationWeight * separation +
        controller.followWeight * follow +
        controller.randomizeWeight * randomize);
    }
}