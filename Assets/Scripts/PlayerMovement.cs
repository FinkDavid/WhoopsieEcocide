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
    [SerializeField] private float pushStrength = 3000;
    [SerializeField] private float pushAwayRadius=50000;
    bool p_facingRight = true;
    float horizontal;

    public Transform p_groundCheck;
    public Rigidbody2D p_rigidbody;
    public LayerMask p_groundPlayer;

    private bool canMove = true;
    

    [FormerlySerializedAs("stunnmTime")] [SerializeField]
    private float stunTime = 0.5f;

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

        // if (Input.GetKey(KeyCode.P) && _powerupState == PowerupState.None)
        // {
        //     setJumpBoost();
        // }
        //
        // if (Input.GetKeyDown(KeyCode.I) && _powerupState == PowerupState.None)
        // {
        //     Push();
        // }

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

        //if(!p_facingRight && horizontal > 0f)
        //{
        //    FlipPlayer();
        //}
        //else if(p_facingRight && horizontal < 0f)
        //{
        //    FlipPlayer();
        //}
    }


    public void Push()
    {
        // Perform the raycast
        RaycastHit2D hitMR = Physics2D.Raycast(transform.position + new Vector3(Math.Abs(transform.localScale.x)/2+0.1f,-transform.localScale.y/3, 0),
            Vector2.right);
        RaycastHit2D hitTR =
            Physics2D.Raycast(transform.position + new Vector3(Math.Abs(transform.localScale.x)/2+.01f, transform.localScale.y/3, 0), Vector2.right);
        RaycastHit2D hitML = Physics2D.Raycast(transform.position + new Vector3(-Math.Abs(transform.localScale.x)/2-0.1f, transform.localScale.y/3, 0),
            Vector2.left);
        RaycastHit2D hitTL =
            Physics2D.Raycast(transform.position + new Vector3(-Math.Abs(transform.localScale.x)/2-0.1f, transform.localScale.y/3, 0), Vector2.left);
        
        
        // Check if the ray hit something
        List<GameObject> hitPlayers = new();
        if (hitMR.collider != null && hitMR.collider.CompareTag("Player"))
        {
            if (!hitPlayers.Contains(hitMR.collider.gameObject)&& Vector3.Distance(transform.position, hitMR.collider.gameObject.transform.position)<pushAwayRadius)
                hitPlayers.Add(hitMR.collider.gameObject);
        }

        if (hitTR.collider != null && hitTR.collider.CompareTag("Player"))
        {
            if (!hitPlayers.Contains(hitTR.collider.gameObject)&& Vector3.Distance(transform.position, hitTR.collider.gameObject.transform.position)<pushAwayRadius)
                hitPlayers.Add(hitTR.collider.gameObject);
        }

        if (hitML.collider != null && hitML.collider.CompareTag("Player"))
        {
            if (!hitPlayers.Contains(hitML.collider.gameObject) && Vector3.Distance(transform.position, hitML.collider.gameObject.transform.position)<pushAwayRadius)
                hitPlayers.Add(hitML.collider.gameObject);
        }

        if (hitTL.collider != null && hitTL.collider.CompareTag("Player"))
        {
            if (!hitPlayers.Contains(hitTL.collider.gameObject)&& Vector3.Distance(transform.position, hitTL.collider.gameObject.transform.position)<pushAwayRadius)
                hitPlayers.Add(hitTL.collider.gameObject);
        }

        if (hitPlayers.Count > 0)
        {
            _soundManager.PlaySoundEffect(_soundManager.SoundEffects.PushBack);
        }

        foreach (GameObject player in hitPlayers)
        {
            Vector2 repelDirection = (player.transform.position - transform.position).normalized;
            player.GetComponent<Rigidbody2D>().AddForce(repelDirection * pushStrength, ForceMode2D.Force);
        }
    }

    public void SetStuned()
    {
        // _powerupState = PowerupState.Stunned;
        // remainingStun = stunTime;

        canMove = false;
        StartCoroutine(ResetMovingAllowed());
        _soundManager.PlaySoundEffect(_soundManager.SoundEffects.PlayerStun);
        _powerupState = PowerupState.Stunned;
        remainingStun = stunTime;
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

    void FlipPlayer()
    {
        p_facingRight = !p_facingRight;
        Vector3 p_scale = transform.localScale;
        p_scale.x *= -1;
        transform.localScale = p_scale;
    }
    
    private IEnumerator ResetMovingAllowed()
    {
        // Wait for one second
        yield return new WaitForSecondsRealtime(stunTime);

        // Set the flag to true, allowing the function to be called again
        canMove = true;
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
}