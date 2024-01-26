using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform model;

    [Header("Stats")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float rotSpeed = 5.0f;
    private CharacterController characterController;

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
        Vector3 movement = new Vector3(_move.x, 0.0f, _move.y);

        model.transform.forward = Vector3.RotateTowards(model.transform.forward, movement, rotSpeed * Time.deltaTime, 0.0f);

        characterController.Move(movement * moveSpeed * Time.deltaTime);
    }
}
