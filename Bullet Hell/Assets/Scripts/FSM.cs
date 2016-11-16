using UnityEngine;
using System.Collections;

public class FSM : MonoBehaviour {

    //Player Transform
    [SerializeField]
    protected Transform m_playerTransform;

    //Next destination of NPC
    [SerializeField]
    protected Vector3 m_destPos;

    //List of points for patrolling
    [SerializeField]
    protected GameObject[] m_patrolPointList;

    protected virtual void Initialize() { }
    protected virtual void FSMUpdate() { }
    protected virtual void FSMFixedUpdate() { }


	// Use this for initialization
	void Start ()
    {
        Initialize();
	}
	
	// Update is called once per frame
	void Update ()
    {
        FSMUpdate();
	}

    void FixedUpdate()
    {
        FSMFixedUpdate();
    }
}
