using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
  [Header("Parameter for character movements")]
  [Tooltip("캐릭터가 도달할 수 있는 달리기 최고 속력 파라미터")]
  [SerializeField] private float maxForwardSpeed = 8f;

  [Tooltip("캐릭터 회전 속력 파라미터")]
  [SerializeField] private float turnSpeed = 100f;

  [Tooltip("캐릭터 점프에 따른 높이값 조절 파라미터")]
  [SerializeField] private float jumpSpeed = 30000f;

  [Tooltip("랜딩 애니메이션 시작 기준 지점을 정하기 위한 레이캐스팅 위치 조절 파라미터")]
  [SerializeField] private float groundRayDist = 2f;

  [Header("Parameter for character animations")]
  [SerializeField] private Transform weapon;
  [SerializeField] private Transform rightHand;
  [SerializeField] private Transform rightUpLeg;
  [SerializeField] private Transform spine;

  [Header("Parameter for mouse sensitivity")]
  [SerializeField] private float xSensitivity = 0.5f;
  [SerializeField] private float ySensitivity = 0.5f;

  [SerializeField] private LineRenderer lineRenderer;

  private float desiredSpeed;
  private float forwardSpeed;

  private const float groundAccel = 5f;
  private const float groundDecel = 25f;

  private Vector2 moveDirection;
  private float jumpDirection;

  private Vector2 lookDirection;
  private Vector2 prevLookDirection;

  

  private Animator animator;
  private Rigidbody rb;
  private bool isOnGround = true;

  public bool IsMoveInput
  {
    get { return !Mathf.Approximately(moveDirection.sqrMagnitude, 0f); }
  }

  private void Start() 
  {
    animator = GetComponent<Animator>();
    rb = GetComponent<Rigidbody>();
  }
  private void Update()
  {
    Move(moveDirection);
    Jump(jumpDirection);

    if(animator.GetBool("Armed"))
    {
      lineRenderer.gameObject.SetActive(true);

      RaycastHit laserHit;
      Ray laserRay = new Ray(lineRenderer.transform.position, lineRenderer.transform.forward);
      if (Physics.Raycast(laserRay, out laserHit))
      {
        lineRenderer.SetPosition(1, lineRenderer.transform.InverseTransformPoint(laserHit.point));
      }
    }
    else
    {
      lineRenderer.gameObject.SetActive(false);
    }    

    RaycastHit hit;
    Ray ray = new Ray(transform.position + Vector3.up * groundRayDist * 0.5f, -Vector3.up);
    if(Physics.Raycast(ray, out hit, groundRayDist))
    {
      if(!isOnGround)
      {
        isOnGround = true;
        animator.SetFloat("LandingVelocity", rb.velocity.magnitude);
        animator.SetBool("Land", true);
        animator.SetBool("Falling", false);
      }      
    }
    else
    {
      isOnGround = false;
      animator.SetBool("Falling", true);
      animator.applyRootMotion = false;
    }    
    Debug.DrawRay(transform.position + Vector3.up * groundRayDist * 0.5f, -Vector3.up * groundRayDist, Color.red);
  }  
  private void LateUpdate()
  {
    prevLookDirection += new Vector2(-lookDirection.y * ySensitivity, lookDirection.x * xSensitivity);
    prevLookDirection.x = Mathf.Clamp(prevLookDirection.x, -30, 30);
    prevLookDirection.y = Mathf.Clamp(prevLookDirection.y, -30, 60);

    spine.localEulerAngles = prevLookDirection;
    // spine.Rotate(-prevLookDirection.y, prevLookDirection.x, 0);
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

  bool readyJump = false;
  float jumpEffort = 0;
  private void Jump(float direction)
  {
    if(direction > 0 && isOnGround)
    {
      animator.SetBool("ReadyJump", true);
      readyJump = true;
      jumpEffort += Time.deltaTime;    
    }      
    else if(readyJump)
    {
      animator.SetBool("Launch", true);
      readyJump = false;
      animator.SetBool("ReadyJump", false);
    }
    Debug.Log("Jump Effort: " + jumpEffort);
  }

  // Animation event function
  public void Launch()
  {
    rb.AddForce(0, jumpSpeed * Mathf.Clamp(jumpEffort, 1, 3) , 0);
    animator.SetBool("Launch", false);
    animator.applyRootMotion = false;
  }
  public void Land()
  {
    animator.SetBool("Land", false);
    animator.applyRootMotion = true;
    animator.SetBool("Launch", false);
    jumpEffort = 0;
  }  
  public void PickupGun()
  {
    weapon.SetParent(rightHand);
    weapon.localPosition = new Vector3(-0.0095f, 0.0541f, 0.0261f);
    weapon.localRotation = Quaternion.Euler(-76.42f, -139.677f, -102.302f);
    weapon.localScale = new Vector3(1, 1, 1);
  }
  public void PutDownGun()
  {
    weapon.SetParent(rightUpLeg);
    weapon.localPosition = new Vector3(-0.0095f, 0.0541f, 0.0261f);
    weapon.localRotation = Quaternion.Euler(-76.42f, -139.677f, -102.302f);
    weapon.localScale = new Vector3(1, 1, 1);
  }

  // For unity event of input system
  public void OnMove(InputAction.CallbackContext context)
  {
    moveDirection = context.ReadValue<Vector2>();
  }
  public void OnLook(InputAction.CallbackContext context)
  {
    lookDirection = context.ReadValue<Vector2>();
  }
  public void OnJump(InputAction.CallbackContext context)
  {
    jumpDirection = context.ReadValue<float>();
  }
  public void OnFire(InputAction.CallbackContext context)
  {    
    if ((int)context.ReadValue<float>() == 1 && animator.GetBool("Armed"))
      animator.SetTrigger("Fire");
  }
  public void OnArmed(InputAction.CallbackContext context)
  {
    animator.SetBool("Armed", !animator.GetBool("Armed"));
  }
}