using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lazer : MonoBehaviour
{

    [SerializeField] private GameObject start;
    [SerializeField] private GameObject end;

    [SerializeField] private LineRenderer lr;

    [SerializeField] private float shootTime = 2f;
    [SerializeField] private float relaodTime = 3f;

    [SerializeField] private LayerMask wallMask;

    [SerializeField] private AudioClip shootSound;

    private bool shooting = false;
    private bool reloading = true;

    private SpriteRenderer startSp;
    private SpriteRenderer endSp;

    private float timeAlive = 0f;

    private void Awake()
    {
        startSp = start.GetComponent<SpriteRenderer>();
        endSp = end.GetComponent<SpriteRenderer>();
        startSp.enabled = false;
        endSp.enabled = false;
        StartCoroutine(startWait());
    }

    private void Update()
    {
        if (!shooting && !reloading)
        {
            StartCoroutine(sht());
        }
        else if (shooting)
        {
            //startSp.enabled = true;
            setPoints();
            
        }
        else if (!shooting)
        {
            startSp.enabled = false;
        }

        if (isTouchingWall())
        {
            Destroy(gameObject);
        }

        timeAlive += Time.deltaTime;
    }


    private IEnumerator startWait()
    {
        reloading = true;
        yield return new WaitForSeconds(relaodTime);
        reloading = false;
    }


    IEnumerator sht()
    {
        shooting = true;
        AudioManager.instance.playSound(shootSound);
        yield return new WaitForSeconds(shootTime);
        clearPoints();
        shooting = false;
        reloading = true;
        yield return new WaitForSeconds(relaodTime);
        reloading = false;
    }

    public float getTimeAlive()
    {
        return timeAlive;
    }

    private void setPoints()
    {
        lr.SetPosition(0, Vector2.zero);
        if (transform.rotation.eulerAngles.z == 0 || transform.rotation.eulerAngles.z == 180)
        {
            if (transform.rotation.eulerAngles.z == 0)
            {
                lr.SetPosition(1, new Vector2(getEndPos().x + Mathf.Abs(lr.transform.position.x), 0));
            }
            else
            {
                lr.SetPosition(1, new Vector2(-(getEndPos().x - Mathf.Abs(lr.transform.position.x)), 0));
            }
        }
        else
        {
            lr.SetPosition(1, new Vector2(getEndPos().y + Mathf.Abs(lr.transform.position.y), 0));

            if(transform.rotation.eulerAngles.z != 90)
            {
                lr.SetPosition(1, new Vector2(-(getEndPos().y - Mathf.Abs(lr.transform.position.y)), 0));
            }
        }

        
        
    }

    private void clearPoints()
    {
        lr.SetPosition(0, Vector2.zero);
        lr.SetPosition(1, Vector2.zero);
    }

    private Vector2 getEndPos()
    {
        Ray2D ry = new Ray2D(lr.gameObject.transform.position, transform.right);
        RaycastHit2D hit2D = Physics2D.Raycast(ry.origin, ry.direction, Mathf.Infinity);
        if(hit2D.collider != null)
        {
            if (hit2D.collider.gameObject.tag.Equals("Player") && !PlayerController.instance.dashing)
            {
                PlayerController.instance.die();
            }
            else if (hit2D.collider.gameObject.tag.Equals("hook") && PlayerController.instance.isGrappling)
            {
                hit2D.collider.gameObject.GetComponentInParent<PlayerController>().fullExtent = true;
                StartCoroutine(CamShake.instance.Shake(.2f, .2f));
            }
            else if (hit2D.collider.gameObject.tag.Equals("hook") && !PlayerController.instance.isGrappling && !PlayerController.instance.dashing)
            {
                PlayerController.instance.die();
            }

            return hit2D.point;
        }
        else
        {
            Debug.Log("Collider not found");
            return Vector2.zero;
        }
     
    }

    private bool isTouchingWall()
    {
        return Physics2D.Raycast(transform.position, transform.up, 0.7f, wallMask) && Physics2D.Raycast(transform.position, -transform.up, 0.7f, wallMask);
    }

}
