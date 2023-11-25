using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = Unity.Mathematics.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    private PowerupState _powerupState = PowerupState.None;
    [SerializeField] public float p_jumpingPower = 10f;
    [SerializeField] float p_speed = 10f;
    [SerializeField] private float pushStrength = 1000;
    [SerializeField] private float pushAwayRadius=50000;
    bool p_facingRight = true;
    float horizontal;

    public Transform p_groundCheck;
    public Rigidbody2D p_rigidbody;
    public LayerMask p_groundPlayer;

    public bool canMove = true;
    

    [FormerlySerializedAs("stunnmTime")] [SerializeField]
    private float stunTime = 2f;

    public float remainingStun;

    //References
    GameManager gameManager;
    PlayerInformation playerInformation;
    Camera cam;

    private Animator _animator;

    private SoundManager _soundManager;

    private bool _readyForGroundCheck = false;

    void Start()
    {
        _soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        playerInformation = GetComponent<PlayerInformation>();
        cam =  GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {

        if (_readyForGroundCheck && IsGrounded())
        {
            _animator.SetBool("jump", false);
            _readyForGroundCheck = false;
        }

        if (_powerupState == PowerupState.Stunned)
        {
            remainingStun -= Time.deltaTime;
            return;
        }
        if (remainingStun <= 0 && _powerupState == PowerupState.Stunned)
            _powerupState = PowerupState.None;

        p_rigidbody.velocity = new Vector2(horizontal * p_speed, p_rigidbody.velocity.y);

        float ratio = (float)Screen.width / (float)Screen.height;
        if (transform.position.y >= cam.transform.position.y + cam.orthographicSize)
        {
            transform.position = new Vector3(transform.position.x, cam.transform.position.y + cam.orthographicSize,
                transform.position.z);
        }

        if(transform.position.x >= cam.transform.position.x + (cam.orthographicSize * ratio))
        {
            transform.position = new Vector3(cam.transform.position.x + (cam.orthographicSize * ratio), transform.position.y, transform.position.z);
        }
        else if(transform.position.x <= cam.transform.position.x - (cam.orthographicSize * ratio))
        {
            transform.position = new Vector3(cam.transform.position.x - (cam.orthographicSize * ratio), transform.position.y, transform.position.z);
        }
    }

    public void PushFinished()
    {
        _animator.SetBool("push", false);
    }

    public void Push()
    {
        _animator.SetBool("push", true);

        // Adjust these values as needed
        float pushRadius = 2.0f;

        // Find colliders within the circular area
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, pushRadius);

        // Check if the collider belongs to an enemy player and push them away
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Player") && collider.gameObject != gameObject)
            {
                StartCoroutine(ApplyPushOverTime(collider.transform, pushStrength));
            }
        }

        _soundManager.PlaySoundEffect(_soundManager.SoundEffects.PushBack);
    }

    private IEnumerator ApplyPushOverTime(Transform target, float pushStrength)
    {
        float stepDuration = 0.02f;  // Adjust as needed, smaller values for more frequent movements
        float elapsedTime = 0.0f;

        Vector3 initialPosition = target.position;
        Vector3 repelDirection = (target.position - transform.position).normalized;

        while (elapsedTime < 0.1f)  // You can adjust the total duration here if needed
        {
            Vector3 targetPosition = initialPosition + repelDirection * pushStrength * (elapsedTime / 0.1f);

            // Check if the target position is valid
            if (!IsPositionBlocked(targetPosition))
            {
                target.position = targetPosition;
            }
            else
            {
                // If the position is blocked, stop pushing
                break;
            }

            elapsedTime += stepDuration;
            yield return new WaitForSeconds(stepDuration);
        }
    }

    private bool IsPositionBlocked(Vector3 position)
    {
        // Cast a ray from the initial position to the target position
        RaycastHit2D hit = Physics2D.Linecast(transform.position, position, LayerMask.GetMask("Ground"));

        // If the ray hits something, the position is blocked
        return hit.collider != null && hit.collider.CompareTag("Block");
    }


    public void SetStunned()
    {
        _animator.SetBool("stunned", true);
        _soundManager.PlaySoundEffect(_soundManager.SoundEffects.PlayerStun);
        _powerupState = PowerupState.Stunned;
        remainingStun = stunTime;
        canMove = false;
        StartCoroutine(ResetMovingAllowed());
    }


    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(p_groundCheck.position, 0.2f, p_groundPlayer);

        //return Physics2D.OverlapBox(p_groundCheck.position, new Vector2(1f, 0.5f), p_groundPlayer);
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (canMove)
        {
            horizontal = context.ReadValue<Vector2>().x;
            _animator.SetFloat("v", context.ReadValue<Vector2>().x);
            _animator.SetFloat("h", context.ReadValue<Vector2>().y);    
        }
        else
        {
            horizontal = 0;
        }
        
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (_powerupState == PowerupState.Stunned)
            return;
        if (context.performed && IsGrounded())
        {
            _animator.SetBool("jump", true);
            StartCoroutine(JumpCheckDelay());
            _soundManager.PlaySoundEffect(_soundManager.SoundEffects.PlayerJump);
            p_rigidbody.velocity = new Vector2(p_rigidbody.velocity.x, p_jumpingPower);
            if (_powerupState == PowerupState.JumpBoost)
                resetJumpboost();
        }

        if (context.canceled && p_rigidbody.velocity.y > 0f)
        {
            p_rigidbody.velocity = new Vector2(p_rigidbody.velocity.x, p_rigidbody.velocity.y * 0.8f);
        }
    }

    private IEnumerator JumpCheckDelay()
    {
        yield return new WaitForSeconds(.2f);
        _readyForGroundCheck = true;
    }
    
    private IEnumerator ResetMovingAllowed()
    {
        yield return new WaitForSecondsRealtime(stunTime);
        canMove = true;
        _powerupState = PowerupState.None;
        _animator.SetFloat("v", 0);
        _animator.SetFloat("h", 0); 
        _animator.SetBool("stunned", false);
    }

    public void setJumpBoost()
    {
        p_jumpingPower *= 1.3f;
        _powerupState = PowerupState.JumpBoost;
    }

    private void resetJumpboost()
    {
        p_jumpingPower /= 1.3f;
        _powerupState = PowerupState.None;
    }

    public void SwapDisappearFinished()
    {
        _animator.SetBool("swapAppearFinished", false);
        gameObject.transform.position = new Vector2(_animator.GetFloat("targetSwapX"), _animator.GetFloat("targetSwapY"));
        _animator.SetBool("swap", false);
    }

    public void SwapAppearFinished()
    {
        canMove = true;
        _animator.SetFloat("v", 0);
        _animator.SetFloat("h", 0);    
        _animator.SetBool("swapAppearFinished", true);
    }
}