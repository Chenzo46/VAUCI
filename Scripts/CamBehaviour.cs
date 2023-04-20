using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CamBehaviour : MonoBehaviour
{
    [SerializeField] private Transform p1, p2;
    [SerializeField] private Camera cam;
    private float orgSize;

    [SerializeField] private Sprite chill, off, scared;

    [SerializeField] private float mid, nearEnd, End;

    [SerializeField] private Image face;

    private bool killed  = false;
    private void Awake()
    {
        
        orgSize = cam.orthographicSize;
        transform.position = new Vector3((p1.position.x + p2.position.x) / 2, (p1.position.y + p2.position.y) / 2, -10);
    }


    void Update()
    {
        float dist = Vector2.Distance(p1.position, p2.position);

        transform.position = new Vector3((p1.position.x + p2.position.x) / 2, (p1.position.y + p2.position.y) / 2, -10);

        cam.orthographicSize = dist / 2f;

        

        if(dist < mid)
        {
            face.sprite = chill;
        }
        else if (dist >= mid && dist < nearEnd)
        {
            face.sprite = off;
        }
        else if (dist >= nearEnd)
        {
            face.sprite = scared;
        }

        if(dist >= End && !killed)
        {
            PlayerController.instance.die();
            
            killed = true;
        }

        
    }

    
}
