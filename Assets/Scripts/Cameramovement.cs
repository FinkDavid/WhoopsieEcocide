using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class Cameramovement : MonoBehaviour
{
    public float startDelay = 2;
    private float remainingFreeze = 0;
    public float freeze = 2;

    public float speed = 1.5f;

    public float speedGainPerSecond = 0.2f;
    public float speedCap = 1000;
    GameManager gameManager;

    [SerializeField] private SoundManager _soundManager;

    private bool _shouldMove = false;

    public bool IsMoving => _shouldMove;

    void Start()
    {
        _soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        StartCoroutine(WaitForMovement(startDelay));
    }

    void LateUpdate()
    {
        if (!gameManager.roundOver)
        {
            MoveCamera();
        }
    }

    private IEnumerator WaitForMovement(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _shouldMove = true;
    }

    private void MoveCamera()
    {
        if (_shouldMove)
        {
            //transform.Translate(Vector3.up * (speed * Time.deltaTime));

            var desiredPosition = transform.position + (Vector3.up * speed) * Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, 0.125f);

            if (speed < speedCap)
                speed += speedGainPerSecond * Time.deltaTime;
        }
    }

    public void SetFreeze()
    {
        _shouldMove = false;
        _soundManager.PlaySoundEffect(_soundManager.SoundEffects.OilFreeze);
        StartCoroutine(WaitForMovement(freeze));
    }
}