using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerManager : MonoBehaviour
{
    
    private Rigidbody rb;
    Animator animator;
    InputManager inputManager;
    CameraManager cameraManager;
    PlayerLocomotion playerLocomotion;

    public bool isInteracting;
    private void Awake()
    {

        animator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();


    }

    private void Update()
    {
        
        inputManager.HandleAllInputs();
        
    }


    //�T�w��s
    private void FixedUpdate()
    {
        playerLocomotion.HandleAllMovement();
    }
    //�b�V�������ե�
    private void LateUpdate()
    {
        cameraManager.HandleAllCameraMovement();
        isInteracting = animator.GetBool("isInteracting");
        playerLocomotion.isJumping = animator.GetBool("isJumping");
        animator.SetBool("isGrounded", playerLocomotion.isGrounded);
    }
  
}
