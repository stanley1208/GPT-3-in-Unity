using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{

    public Animator animator;
    int horizontal;
    int vertical;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
    }

    public void PlayTargetAnimation(string targetAnimation, bool isInteracting)
    {
        animator.SetBool("isInteracting", isInteracting);
        animator.CrossFade(targetAnimation, 0.2f);
    }


    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        float snappendHorizontal;
        float snappendVertical;
        #region Snappend Horizontal
        if (horizontalMovement > 0 && horizontalMovement < 0.5f)
        {
            snappendHorizontal = 0.5f;
        }
        else if (horizontalMovement > 0.5f)
        {
            snappendHorizontal = 1;
        }
        else if (horizontalMovement < 0 && horizontalMovement > -0.5f)
        {
            snappendHorizontal = -0.5f;
        }
        else if (verticalMovement < -0.5f)
        {
            snappendHorizontal = -1;
        }
        else
        {
            snappendHorizontal = 0;
        }
        #endregion
        #region Snappend Vertical
        if (verticalMovement > 0 && verticalMovement < 0.5f)
        {
            snappendVertical = 0.5f;
        }
        else if (verticalMovement > 0.5f)
        {
            snappendVertical = 1;
        }
        else if (verticalMovement < 0 && verticalMovement > -0.5f)
        {
            snappendVertical = -0.5f;
        }
        else if (verticalMovement < -0.5f)
        {
            snappendVertical = -1;
        }
        else
        {
            snappendVertical = 0;
        }
        #endregion

        if (isSprinting)
        {
            snappendHorizontal = horizontalMovement;
            snappendVertical = 2;
        }
        animator.SetFloat(horizontal, snappendHorizontal, 0.1f, Time.deltaTime);
        animator.SetFloat(vertical, snappendVertical, 0.1f, Time.deltaTime);
    }
}
