using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private Transform model;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform grabCenter;
    [SerializeField] private Transform grabSocket; // Socket Enemies get attached to while grabbed
    public Animator playerAnimator;

    [Header("Stats")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float rotSpeed = 5.0f;
    [SerializeField] private float diveSpeed = 10.0f;
    [SerializeField] private float diveDuration = 0.5f;
    [SerializeField] private float grabRadius = 0.5f;

    private CharacterController characterController;
    private bool isDiving;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Move(Vector2 _move)
    {
        if (isDiving || playerAnimator.GetBool("IsTickling"))
            return;

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
        if (isDiving)
            return;

        StartCoroutine(DiveCoroutine());
    }
    private IEnumerator DiveCoroutine()
    {
        playerAnimator.CrossFade("Dive", 0.2f);

        isDiving = true;

        float startTime = Time.time; 
        while (Time.time < startTime + diveDuration)
        {
            characterController.Move(model.forward * diveSpeed * Time.deltaTime);

            Collider[] hitColliders = Physics.OverlapSphere(grabCenter.position, grabRadius, LayerMask.GetMask("Enemy"));
            foreach (var collider in hitColliders)
            {
                Fruit targetFruit = collider.GetComponentInParent<Fruit>();

                if (targetFruit)
                {
                    Debug.Log("Hit fruit " + targetFruit.name);

                    isDiving = false;

                    StartCoroutine(TickleCoroutine());

                    // Leave coroutine
                    yield break;
                }
            }

            yield return new WaitForEndOfFrame(); 
        }

        isDiving = false;
    }
    IEnumerator TickleCoroutine()
    {
        Debug.Log("Tickling...");

        playerAnimator.SetBool("IsTickling", true);

        //yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(2.0f);

        playerAnimator.SetBool("IsTickling", false);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(grabCenter.position, grabRadius);
    }
}