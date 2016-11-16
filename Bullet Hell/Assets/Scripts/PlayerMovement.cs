using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    public float speed;
    public Rigidbody rb;

    void Start()
    {

    }
    void Update()
    {
        // Movement //
        Vector3 newPosition = transform.position;
        if (Input.GetKey(KeyCode.W))
        {
            
            newPosition.z += speed;
            transform.position = newPosition;
        }
        if (Input.GetKey(KeyCode.S))
        {
           
            newPosition.z -= speed;
            transform.position = newPosition;
        }
        if (Input.GetKey(KeyCode.A))
        {
            newPosition.x -= speed;
            transform.position = newPosition;
        }
        if (Input.GetKey(KeyCode.D))
        {
            newPosition.x += speed;
            transform.position = newPosition;
        }

        // Looking at mouse position            //
        // Did we want to use a ray for this?.. //
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit, 100))
        {
            Vector3 lookPoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.LookAt(lookPoint);
        }
    }
}
