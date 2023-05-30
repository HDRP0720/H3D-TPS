using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
  // [SerializeField] private float moveSpeed = 2f;
  [SerializeField] private float maxForwardSpeed = 8f;
  [SerializeField] private float turnSpeed = 100f; 

  private float desiredSpeed;
  private float forwardSpeed;

  private const float groundAccel = 5f;
  private const float groundDecel = 25f;

  private Vector2 moveDirection;
  private float jumpDirection;

  private Animator animator;

  bool IsMoveInput
  {
    get { return !Mathf.Approximately(moveDirection.sqrMagnitude, 0f); }
  }

  private void Start() 
  {
    animator = GetComponent<Animator>();
  }
  private void Update()
  {
    Move(moveDirection);
    Jump(jumpDirection);
  }  

  private void Move(Vector2 direction)
  {
    float turnAmount = direction.x;
    float fDirection = direction.y;

    if(direction.sqrMagnitude > 1f) direction.Normalize();

    desiredSpeed = direction.magnitude * maxForwardSpeed * Mathf.Sign(fDirection);
    float acceleration = IsMoveInput ? groundAccel : groundDecel;

    forwardSpeed = Mathf.MoveTowards(forwardSpeed, desiredSpeed, acceleration * Time.deltaTime);
    animator.SetFloat("ForwardSpeed", forwardSpeed);

    transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);  
  }

  private void Jump(float direction)
  {
    Debug.Log(direction);
  }

  // For unity event of input system
  public void OnMove(InputAction.CallbackContext context)
  {
    moveDirection = context.ReadValue<Vector2>();
  }
  public void OnJump(InputAction.CallbackContext context)
  {
    jumpDirection = context.ReadValue<float>();
  }
}