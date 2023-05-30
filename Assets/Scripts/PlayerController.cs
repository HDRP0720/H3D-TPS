using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
  [SerializeField] private float moveSpeed = 2f;

  private Vector2 moveDirection;

  // For unity event of input system
  public void OnMove(InputAction.CallbackContext context)
  {
    moveDirection = context.ReadValue<Vector2>();
  }

  private void Move(Vector2 direction)
  {
    transform.Translate(direction.x * moveSpeed * Time.deltaTime, 0, direction.y * moveSpeed * Time.deltaTime);
  }

  private void Update()
  {
    Move(moveDirection);
  }
}
