using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 500f;

    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] Vector3 groundCheckOffset;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform targetPos;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float rebound;
    [SerializeField] private float shotDelay;
    float curDelay;

    bool isGrounded;

    float ySpeed;
    Quaternion targetRotation;

    CameraController cameraController;
    Animator animator;
    CharacterController characterController;

    private void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));

        var moveInput = new Vector3(h, 0, v).normalized;

        var moveDir = cameraController.PlanarRotation * moveInput;

        GroundCheck();

        curDelay += Time.deltaTime;
        if (Input.GetMouseButton(0))
        {
            Fire();
            animator.SetBool("isFire", true);
        }
        else animator.SetBool("isFire", false);

        if (isGrounded)
        {
            ySpeed = -0.5f;
        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftShift) && moveAmount > 0) {
            moveSpeed = 10;
            animator.SetFloat("moveAmount", 2, 0.2f, Time.deltaTime);
        }
        else {
            moveSpeed = 5;
            animator.SetFloat("moveAmount", moveAmount, 0.2f, Time.deltaTime);
        }
        if (animator.GetBool("isFire")) return;

        var velocity = moveDir * moveSpeed;
        velocity.y = ySpeed;

        characterController.Move(velocity * Time.deltaTime);

        if (moveAmount > 0)
        {
            targetRotation = Quaternion.LookRotation(moveDir);
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
            rotationSpeed * Time.deltaTime);

        
    }

    public void Fire()
    {
        if (curDelay >= shotDelay)
        {
            curDelay = 0;
            
            Bullet laser = ObjectPool.GetObject();
            laser.transform.SetPositionAndRotation(startPos.position, startPos.rotation);
            laser.target = targetPos.position;
        }
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }
}