using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator animator;

    private void OnEnable()
    {
        InputHandeler.JumpAction += Jump;
    }

    private void OnDisable()
    {
        InputHandeler.JumpAction -= Jump;
    }

    private void Jump()
    {
        animator.SetTrigger("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("grounded",PlayerMovement.isGrounded);
        animator.SetFloat("Velocity", InputHandeler.InputAxis.normalized.magnitude);
    }
}
