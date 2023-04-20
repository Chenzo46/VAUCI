using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cornerPoint : MonoBehaviour
{
    [SerializeField] private Transform wallX, wallY;
    [SerializeField] private Vector2 offset;
    
    private void Update()
    {
        transform.position = new Vector2(wallX.position.x+offset.x, wallY.position.y + offset.y);
    }
}
