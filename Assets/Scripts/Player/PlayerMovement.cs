using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerController playerController { private set; get; }

    [SerializeField] private Transform model;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform grabCenter;
    [SerializeField] private Transform grabSocket; // Socket Enemies get attached to while grabbed
    
    public Animator playerAnimator;

    private Fruit grabbedFruit;
    private Vector3 grabVelocity;
    private float grabSmooth = 0.1f;

    [Header("Stats")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float rotSpeed = 5.0f;
    [SerializeField] private float diveSpeed = 10.0f;
    [SerializeField] private float fallAccel = 9.81f;
    [SerializeField] private float diveDuration = 0.5f;
    [SerializeField] private float grabRadius = 0.5f;
    [SerializeField] private float hitDuration = 1.0f;

    private float hitTimeStamp = -Mathf.Infinity;
    private CharacterController characterController;
    private bool isDiving;

    private float yVelocity = 0.0f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (grabbedFruit)
        {
            grabbedFruit.transform.position = Vector3.SmoothDamp(grabbedFruit.transform.position, grabSocket.position, ref grabVelocity, grabSmooth);
        }

        if (!characterController.isGrounded)
        {
            yVelocity -= fallAccel * Time.deltaTime;
            characterController.Move(Vector3.up * yVelocity * Time.deltaTime);
        }
        else
        {
            yVelocity = 0.0f;
        }
    }
    public void Move(Vector2 _move)
    {
        if (isDiving || playerAnimator.GetBool("IsTickling"))
            return;

        if (Time.time < hitTimeStamp + hitDuration)
        {
            // On hit logic here
            _move *= 0.5f;
        }

        Vector3 cameraForward = playerCamera.forward;
        cameraForward.y = 0.0f;
        cameraForward.Normalize();

        Vector3 cameraRight = playerCamera.right;
        cameraRight.y = 0.0f;
        cameraRight.Normalize();

        Vector3 movement = cameraForward * _move.y;
        movement += cameraRight * _move.x;

        model.transform.forward = Vector3.RotateTowards(model.transform.forward, movement, rotSpeed * Time.deltaTime, 0.0f);

        characterController.Move(movement * moveSpeed * Time.deltaTime);
    }
    public void Dive()
    {
        if (isDiving || juiceTimeStamp + 0.5f > Time.time)
            return;

        StartCoroutine(DiveCoroutine());
    }
    private IEnumerator DiveCoroutine()
    {
        playerAnimator.CrossFade("Dive", 0.05f);

        isDiving = true;

        yield return new WaitForSeconds(0.1f);

        float startTime = Time.time; 
        while (Time.time < startTime + diveDuration)
        {
            characterController.Move(model.forward * diveSpeed * Time.deltaTime);

            Collider[] hitColliders = Physics.OverlapSphere(grabCenter.position, grabRadius, LayerMask.GetMask("Enemy"));
            foreach (var collider in hitColliders)
            {
                Fruit targetFruit = collider.GetComponentInParent<Fruit>();

                if (targetFruit && targetFruit.juiceAmount > 0.0f)
                {
                    Debug.Log("Hit fruit " + targetFruit.name);

                    isDiving = false;

                    grabbedFruit = targetFruit;
                    grabbedFruit.stateLock = true;

                    playerController.juiceMashUI.SetJuice((int)grabbedFruit.my_type);
                    playerController.juiceMashUI.SetValue(grabbedFruit.juiceAmount);

                    StartCoroutine(TickleCoroutine());

                    // Leave coroutine
                    yield break;
                }
            }

            yield return new WaitForEndOfFrame(); 
        }

        //playerController.audioAgent.Play("tackle fail");

        yield return new WaitForSeconds(0.433f);

        isDiving = false;
    }


    // Tickling stats
    [Header("Tickling")]
    [SerializeField] private float juiceRate = 0.1f; 
    [SerializeField] private float stopJuiceTime = 0.5f; 
    private float juiceTimeStamp;

    public void JuiceTarget()
    {
        if (!grabbedFruit)
            return;

        grabbedFruit.juiceAmount -= juiceRate;
        playerController.glassQuantities[1].AddQuantity(grabbedFruit.my_type, juiceRate);

        playerController.audioAgent.Play("getting juiced");

        if (grabbedFruit.juiceAmount <= 0.0f)
        {
            // Release grabbed fruit
            grabbedFruit.EnterState(Fruit.AIStates.Zombie);
            ReleaseGrab();
        }

        juiceTimeStamp = Time.time;
    }
    public Fruit GetGrabbedFruit()
    {
        return grabbedFruit;
    }
    public void ReleaseGrab()
    {
        if (grabbedFruit)
        {
            grabbedFruit.stateLock = false;
            grabbedFruit = null;
        }

        playerAnimator.SetBool("IsTickling", false);
    }
    IEnumerator TickleCoroutine()
    {
        Debug.Log("Tickling...");

        playerAnimator.SetBool("IsTickling", true);

        juiceTimeStamp = Time.time;

        //yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => { return Time.time > juiceTimeStamp + stopJuiceTime || grabbedFruit == null; });

        playerAnimator.SetBool("IsTickling", false);

        ReleaseGrab();
    }
    public void Hit()
    {
        hitTimeStamp = Time.time;

        playerAnimator.Play("GetHit");

        playerAnimator.SetBool("IsTickling", false);
        ReleaseGrab();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(grabCenter.position, grabRadius);
    }
}