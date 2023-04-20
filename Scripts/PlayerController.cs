using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private Rigidbody2D rb;

    [Header("Dashing")]
    [SerializeField] private float dashForce = 5f;
    [SerializeField] private Vector2 endSpeed = new Vector2(1,1);
    [SerializeField] private GameObject afterImage;
    [SerializeField] private float spawnRate = 0.5f;
    [SerializeField] private int dashes = 3;
    [SerializeField] private float dashReloadTime = 3f;
    [SerializeField] private Image[] dashImages;
    [SerializeField] private AudioClip dashSound;

    [Header("Grappling")]
    [SerializeField] private float grappleDistance = 5f;
    [SerializeField] private GameObject hook;
    [SerializeField] private float grappleSpeed = 7f;
    [SerializeField] private float returnMultiplier = 1.2f;
    [SerializeField] private LineRenderer lr;
    [SerializeField] private LayerMask walls;
    [SerializeField] private GameObject lazer;
    [SerializeField] private AudioClip throwGrapple;



    [Header("Lazers")]
    [SerializeField] private int maxLazers = 8;
    [SerializeField] private GameObject[] lazerArray = new GameObject[0];

    [Header("Death")]
    [SerializeField] private GameObject deathParticles;
    [SerializeField] private AudioClip deathSound;

    public static PlayerController instance;

    private Vector2 moveInput;

    private Vector2 pivotPosition;

    private int tempDashes;
    private bool dashReloading = false;

    //Dashing after image variables

    private float tm;

    //Hidden Variables
    [HideInInspector] public bool fullExtent = false;
    [HideInInspector] public bool dashing = false;
    [HideInInspector] public bool isGrappling = false;

    private void Awake()
    {
        tempDashes = dashes;
        instance = this;
        Time.timeScale = 1;
    }

    void Update()
    {
        
        if(lookDir() != Vector2.zero && !isGrappling)
        {
            pivotPosition = new Vector2(lookDir().x, lookDir().y);

            setHookRotation();

            hook.transform.localPosition = pivotPosition;
        }

        //Dashing Behaviour
        if (Input.GetKeyDown(KeyCode.Space) && !dashing && tempDashes > 0)
        {
            dashing = true;

            rb.velocity = Vector2.zero;

            rb.AddForce(moveInput.normalized * dashForce, ForceMode2D.Impulse);

            tempDashes--;

            AudioManager.instance.playSound(dashSound);
        }
        else if ((Mathf.Abs(rb.velocity.x) <= endSpeed.x && Mathf.Abs(rb.velocity.y) <= endSpeed.y) && dashing)
        {
            dashing = false;
        }

        //Grappling Behaviour
        if (Input.GetKeyDown(KeyCode.F) && !isGrappling)
        {
            isGrappling = true;
            AudioManager.instance.playSound(throwGrapple);
        }

        if (isGrappling && !fullExtent)
        {
            hook.transform.Translate(hook.transform.right * Time.deltaTime * grappleSpeed, Space.World);
        }

        if (getLineDistance() >= grappleDistance)
        {
            fullExtent = true;
            StartCoroutine(CamShake.instance.Shake(.2f,.2f));
        }

        if (fullExtent && isGrappling)
        {
            hook.transform.Translate(-hook.transform.right * Time.deltaTime * grappleSpeed * returnMultiplier, Space.World);
        }

        if (getLineDistance() <= 1 && isGrappling && fullExtent)
        {
            isGrappling = false;
            fullExtent = false;
        }

        if (dashing)
        {
            if(tm <= 0)
            {
                Instantiate(afterImage, transform.position, Quaternion.identity);
                tm = spawnRate;
            }
            else
            {
                tm -= Time.deltaTime;
            }

        }

        if (!dashReloading && !(tempDashes > 0))
        {
            setDashImages();
            StartCoroutine(reloadingDash());
        }
        else if (!dashReloading)
        {
            setDashImages();
        }

        setLinePoints();
        
    }

    public void placeLazer(GameObject par, float rot)
    {
        lazerArray = GameObject.FindGameObjectsWithTag("lazer");
        if(lazerArray.Length >= maxLazers)
        {
            GameObject longestLazer = lazerArray[0];
            float longestTime = lazerArray[0].GetComponent<lazer>().getTimeAlive();

            foreach(GameObject g in lazerArray)
            {
                if(g.GetComponent<lazer>().getTimeAlive() > longestTime)
                {
                    longestLazer = g;
                }
            }
            Destroy(longestLazer);
            GameObject GO = Instantiate(lazer, hook.transform.position, Quaternion.Euler(0, 0, rot), par.transform);
        }
        else
        {
            GameObject g = Instantiate(lazer, hook.transform.position, Quaternion.Euler(0, 0, rot), par.transform);
        }
        
    }
    /*
    private void setLazerPos(GameObject g, float rot)
    {
        if(rot == 90)
        {
            g.transform.position = new Vector2(transform.position.x, 0.5073f);
        }
        else if(rot == -90)
        {
            g.transform.position = new Vector2(transform.position.x, -0.5073f);
        }
        else if (rot == 0)
        {
            g.transform.position = new Vector2(0.5073f, transform.position.y);
        }
        else
        {
            g.transform.position = new Vector2(-0.5073f, transform.position.y);
        }
    }
    */
    private void FixedUpdate()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (!dashing && !isGrappling)
        {
            rb.velocity = moveInput.normalized * moveSpeed * Time.fixedDeltaTime*100;
        }

    }

    private bool isTouchingWall()
    {
        return Physics2D.Raycast(hook.transform.position, hook.transform.right, 0.55f, walls);
    }

    private void setLinePoints()
    {
        lr.SetPosition(0, Vector2.zero);
        lr.SetPosition(1, hook.transform.localPosition);
    }
    private float getLineDistance()
    {
        return Vector2.Distance(lr.GetPosition(0), lr.GetPosition(1));
    }

    private Vector2 lookDir()
    {
        Vector2 dir = new Vector2(Input.GetAxisRaw("Grapple Horizontal"), Input.GetAxisRaw("Grapple Vertical"));

        if(!(dir.x != 0 && dir.y != 0))
        {
            return dir;
        }
        else
        {
            return Vector2.zero;
        }

        
    }
    private void setHookRotation()
    {
        if (lookDir() != Vector2.zero)
        {
            if(lookDir().x == 1)
            {
                hook.transform.rotation = Quaternion.Euler(0,0,0);
            }
            else if (lookDir().x == -1)
            {
                hook.transform.rotation = Quaternion.Euler(0, 0, 180);
            }
            else if(lookDir().y == 1)
            {
                hook.transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            else if(lookDir().y == -1)
            {
                hook.transform.rotation = Quaternion.Euler(0, 0, -90);
            }
        }
    }

    private IEnumerator reloadingDash()
    {
        dashReloading = true;
        yield return new WaitForSeconds(dashReloadTime);
        tempDashes = dashes;
        dashReloading = false;
        revDashImages();
    }

    private void setDashImages()
    {
        for(int i = 1; i <= dashImages.Length; i++)
        {
            if(i > tempDashes)
            {
                dashImages[i - 1].gameObject.SetActive(false);
            }
        }
    }

    private void revDashImages()
    {
        foreach(Image i in dashImages)
        {
            i.gameObject.SetActive(true);
        }
    }

    public void die()
    {
        Instantiate(deathParticles, transform.position, deathParticles.transform.rotation);
        AudioManager.instance.playSound(deathSound);
        Destroy(gameObject);
        Time.timeScale = 0f;
    }

}
