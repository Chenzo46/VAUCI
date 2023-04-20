using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class wallScript : MonoBehaviour
{
    [SerializeField] private float grabForce = 50f;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private Vector2 moveDirection;
    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private float lazerRotation;

    [SerializeField] private AudioClip grappleWallSound;


    private void Update()
    {
        if (rb.velocity == Vector2.zero)
        {
            rb.isKinematic = true;
        }
    }

    private void FixedUpdate()
    {
        if (rb.isKinematic)
        {
            rb.velocity = moveDirection * moveSpeed * moveSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("hook"))
        {
            PlayerController pm = collision.GetComponentInParent<PlayerController>();

            if (pm.isGrappling && rb.isKinematic)
            {
                rb.velocity = Vector2.zero;

                rb.isKinematic = false;

                pm.fullExtent = true;

                rb.AddForce(-collision.transform.right * grabForce, ForceMode2D.Impulse);

                pm.placeLazer(gameObject,lazerRotation);

                StartCoroutine(CamShake.instance.Shake(.2f, .2f));

                AudioManager.instance.playSound(grappleWallSound);

            }
            else
            {
                pm.fullExtent = true;
                
            }
        }
        else if (collision.tag.Equals("Player"))
        {
            PlayerController.instance.die();
            
        }
    }
}
