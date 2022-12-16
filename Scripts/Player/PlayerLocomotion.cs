using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    AnimatorManager animatorManager;
    InputManager inputManager;
    PlayerManager playerManager;


    Vector3 moveDirection;
    Transform cameraObject;
    Rigidbody playerRigidbody;


    [Header("Falling")]
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public LayerMask groundLayer;
    public float maxDistance = 5;
    //不在地以下，在正確位置
    public float rayCastHeightOffSet = -4;


    [Header("Movement Flags")]
    public bool isSprinting;
    public bool isGrounded;
    public bool isJumping;

    [Header("Movement Speeds")]
    public float walkingSpeed = 1.5f;
    public float runningSpeed = 5;
    public float sprintingSpeed = 7;
    public float rotationSpeed = 2;

    //強力變量必負值,否則為負數
    [Header("Jump Speeds")]
    public float jumpHeight = 6;
    public float gravityIntensity = -10;



    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerManager = GetComponent<PlayerManager>();
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;

    }

    public void HandleAllMovement()
    {

        HandleFallingAndLanding();
        if (playerManager.isInteracting)
            return;
        if (isGrounded && !isJumping)
        {
            Vector3 movementVelocity = moveDirection;
            playerRigidbody.velocity = movementVelocity;
        }

        HandleMovement();
        HandleRotation();
    }

    //Movement Input
    private void HandleMovement()
    {

        moveDirection = new Vector3(cameraObject.forward.x, 0f, cameraObject.forward.z) * inputManager.verticalInput;
        moveDirection = moveDirection + cameraObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();
        //不走向天空
        moveDirection.y = 0;
        moveDirection *= runningSpeed;

        //sprinting ,選sprinting speed
        //running ,選running speed
        //walking  ,選walking speed
        if (isSprinting)
        {
            moveDirection *= sprintingSpeed;
        }
        else
        {

            if (inputManager.moveAmount > 0.5f)
            {
                moveDirection *= runningSpeed;
            }
            else
            {
                moveDirection *= walkingSpeed;
            }

        }

        //移動速度 = 移動方向
        Vector3 movementVelocity = moveDirection;
        playerRigidbody.velocity = movementVelocity;

        if (isGrounded && !isJumping)
        {

            Vector3 motionVelocity = moveDirection;
            playerRigidbody.velocity = movementVelocity;

        }
    }

    private void HandleRotation()
    {

        Vector3 targetDirection = Vector3.zero;
        targetDirection = new Vector3(cameraObject.forward.x, 0f, cameraObject.forward.z) * inputManager.verticalInput;
        targetDirection = targetDirection + cameraObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;
        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;
        //看向目標
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (isGrounded && !isJumping)
        {
            transform.rotation = playerRotation;
        }
    }

    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        rayCastOrigin.y = rayCastOrigin.y + rayCastHeightOffSet;
        if (!isGrounded && isJumping)
        {
            if (playerManager.isInteracting == false)
            {
                animatorManager.PlayTargetAnimation("Falling", true);
            }
            inAirTimer = inAirTimer + Time.deltaTime;
            //助力
            playerRigidbody.AddForce(transform.forward * leapingVelocity);
            playerRigidbody.AddForce(-Vector3.up * fallingVelocity * inAirTimer);
        }


        if (Physics.SphereCast(rayCastOrigin, 0.2f, -Vector3.up, out hit, maxDistance, groundLayer))
        {
            if (!isGrounded && playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Landing", true);
            }
            inAirTimer = 0;
            isGrounded = true;
            playerManager.isInteracting = false;

        }
        else
        {
            isGrounded = false;
        }

    }

    public void HandleJumping()
    {
        if (isGrounded)
        {
            animatorManager.animator.SetBool("isJumping", true);
            animatorManager.PlayTargetAnimation("Jump", false);

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpingVelocity;
            playerRigidbody.velocity = playerVelocity;

        }



    }
}
