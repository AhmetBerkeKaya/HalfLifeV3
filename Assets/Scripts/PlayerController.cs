using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public CharacterController characterController;
    public float speed = 10f;
    public float gravity = -14f;
    public float PlayerHealth = 100f;
    private Vector3 gravityVector;

    // Ground Check
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.35f;
    public LayerMask groundLayer;

    public bool isGrounded = false;
    public float jumpSpeed = 5f;

    // UI
    public Slider healthSlider;
    public TMP_Text  healthText;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        GroundCheck();
        JumpAndGravity();
    }

    void MovePlayer()
    {
        Vector3 moveVector = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;

        characterController.Move(moveVector * speed * Time.deltaTime);
    }
    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }
    void JumpAndGravity()
    {
        gravityVector.y += gravity * Time.deltaTime;

        characterController.Move(gravityVector * Time.deltaTime);


        if (isGrounded && gravityVector.y < 0)
        {
            gravityVector.y = -3f;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            gravityVector.y = Mathf.Sqrt(jumpSpeed * -2f * gravity);
        }
    }
    public void PlayerTakeDamage(int DamageAmount)
    {
        PlayerHealth -= DamageAmount;
        healthSlider.value -= DamageAmount;
        HealthTextUpdate();

        if(PlayerHealth <= 0)
        {
            PlayerDeath();
            HealthTextUpdate();
            healthSlider.value = 0;
        }
    }
    void PlayerDeath()
    {
        gameManager.RestartGame();
    }

    void HealthTextUpdate()
    {
        healthText.text = PlayerHealth.ToString();
    }
}
