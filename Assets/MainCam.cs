using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCam : MonoBehaviour
{
    public float speed;
    public float slowdownspeed;
    public GameObject player;
    public List<GameObject> dragToCam;

    private Vector3 momentom;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate() {
        foreach (GameObject g in dragToCam) {
            g.transform.position = transform.position;
        }
        
        Vector3 toPlayer = new Vector3((player.transform.position.x - transform.position.x),
            (player.transform.position.y - transform.position.y), 0);
        momentom = toPlayer.normalized * speed * Mathf.Pow(toPlayer.magnitude,2);
        
        if (momentom.magnitude * Time.deltaTime >= toPlayer.magnitude) {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        } else {
            transform.position += momentom * Time.deltaTime;
        }
        
    }
}