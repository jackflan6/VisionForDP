using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private float angle;
    private Vector3 movement;
    public float speed;
    private static int amountOfmovementLines = 12;
    private static float movementLineRange=Mathf.PI / 2;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    
    void Update()
    {
        //gets inputs from the input manager
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        movement = new Vector3(x, y, 0).normalized * speed * Time.deltaTime;
        
        
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        angle = Vector2.SignedAngle(Vector2.right, mouse-transform.position);

        
        
        
        CheckForWallCollision(ref movement);

        
        transform.position += movement;
        transform.eulerAngles = new Vector3(0, 0, angle);
        
        
    }
    
    Vector2[] DrawLine1 = new Vector2[amountOfmovementLines+1];
    Vector2[] DrawLine2 = new Vector2[amountOfmovementLines+1];

    public void OnDrawGizmos() {
        for (int i = 0; i <= amountOfmovementLines; i++) {
            Gizmos.DrawLine(DrawLine1[i], DrawLine2[i]);  
        }
    }
    
    private Vector2 RotateV2(Vector2 direction, float amount) {
        return new Vector2((direction.x * Mathf.Cos(amount ) - direction.y * Mathf.Sin(amount )), 
            (direction.x * Mathf.Sin(amount) + direction.y * Mathf.Cos(amount )));
    }

    private float disFromPlane(Vector3 v, Vector3 p) {
        return v.magnitude * (Vector2.Angle(p,v) > 90 ? -1 : 1);
    }
    
    //detects walls in front of player using ray casting without the rigidbody
    private void CheckForWallCollision(ref Vector3 movement) {
        Vector3 origionalMovement = movement;
        List<Vector2> normals = new List<Vector2>();
        foreach (CircleCollider2D c in GetComponents<CircleCollider2D>()) {
            Vector2 posOnCollider = (Vector2)transform.position + c.offset + (Vector2)movement.normalized * c.radius;
            Vector2 perpvectortocollider = RotateV2((Vector2)movement.normalized * c.radius, Mathf.PI / 4);
            Vector2 perpvectortocollider2 = RotateV2((Vector2)movement.normalized * c.radius, -Mathf.PI / 4);
            
            //raycasts around 
            for (int i = 0; i <= amountOfmovementLines; i++) {
                RaycastHit2D hit;
                Vector2 dir = RotateV2((Vector2)origionalMovement.normalized * c.radius,
                    (2*i * movementLineRange / amountOfmovementLines) - movementLineRange);
                Vector2 nextDir =  RotateV2((Vector2)origionalMovement.normalized * c.radius,
                    (2*(i+1) * movementLineRange / amountOfmovementLines) - movementLineRange);
                hit = Physics2D.Raycast((Vector2)transform.position + c.offset + dir, (Vector2)origionalMovement,
                    origionalMovement.magnitude,
                    LayerMask.GetMask("Walls"));
                
                DrawLine1[i] = (Vector2)transform.position + c.offset + dir ;
                DrawLine2[i] = hit ? hit.point :(Vector2)transform.position + c.offset + dir + (Vector2)origionalMovement;
                
                //changes movement to wall
                RaycastHit2D normalHit;
                if (hit) {
                    normalHit = Physics2D.Raycast(transform.position , (hit.point - (Vector2)transform.position), (hit.point - (Vector2)transform.position).magnitude * 2,
                        LayerMask.GetMask("Walls"));
                    normals.Add(normalHit.normal);
                    Debug.DrawLine(hit.point, normalHit.normal+hit.point, Color.red);
                    if ((movement.magnitude) > (hit.distance )) {
                        movement = (hit.distance) * origionalMovement.normalized ;
                    }
                }
                
                
            }

        }
        
        //allows player to slide along a wall
        bool multipleNormals = false;
        Vector2 lastNormal = new Vector2(0,0);
        foreach (Vector2 normal in normals) {
            if (lastNormal != new Vector2(0,0) && lastNormal != normal ) {
                multipleNormals = true;
                break;
            }
            lastNormal = normal;
        }

        if (!multipleNormals && normals.Count != 0) {
            movement += Vector3.ProjectOnPlane(origionalMovement, normals[0]);
        }
    }
}

