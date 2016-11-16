using UnityEngine;
using System.Collections;

public class PlayerTankController : MonoBehaviour {

    public GameObject m_Bullet;



    private float m_curSpeed, m_targetSpeed, m_rotSpeed;


    [SerializeField]
    private float m_maxForwardSpeed = 15.0f;

    [SerializeField]
    private float m_maxBackwardSpeed = -10.0f;

    [SerializeField]
    private float m_acceleration = 1.0f;



	// Use this for initialization
	void Start () {
        m_rotSpeed = 150.0f;
	}
	
	// Update is called once per frame
	void Update () {
        
        UpdateControl();
   }

    /// <summary>
    /// Deals with character controls:
    /// Turret Controls,
    /// Movement Controls,
    /// Shooting Controls
    /// </summary>
    void UpdateControl()
    {
   
        if (Input.GetKey(KeyCode.W))
        {
            m_targetSpeed = m_maxForwardSpeed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            m_targetSpeed = m_maxBackwardSpeed;
        }
        else
        {
            m_targetSpeed = 0;
        }
        if(Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, -m_rotSpeed * Time.deltaTime, 0);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, m_rotSpeed * Time.deltaTime, 0);
        }

        m_curSpeed = Mathf.Lerp(m_curSpeed, m_targetSpeed, 7.0f * Time.deltaTime);

        transform.Translate(Vector3.forward * m_acceleration * Time.deltaTime * m_curSpeed);
    }
}
