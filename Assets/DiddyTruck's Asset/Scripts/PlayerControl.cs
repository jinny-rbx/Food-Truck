using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerControl : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float turnSpeed = 90f;        // Rotation speed in degrees per second
    public float gravity = -9.81f;

    [Header("Player")]
    public float currentHealth;
    public float currentSatisfy;
    public float currentEnergy;

    [Header("Action")]
    public Action OnHealthChange;
    public Action OnSatisfyChange;
    public Action OnEnergyChange;



    private GameManager game;
    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private float startYRotation;
    private float currentRelativeAngle = 0f;

    void Start()
    {
        // Fix: Connect to the Singleton Instance
        game = GameManager.Instance;

        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (game != null)
        {
            currentHealth = game.maxHealth;
            currentSatisfy = 0;
            currentEnergy = game.maxEnergy;
        }
        else
        {
            Debug.LogError("GameManager Instance is missing in the scene!");
        }

        // Save initial Y rotation
        startYRotation = transform.eulerAngles.y;
    }

    void Update()
    {
        // 1. Get input: W/S for Vertical (Move), A/D for Horizontal (Turn)
        float turnInput = Input.GetAxis("Horizontal"); // A (-1) and D (+1)
        float moveInput = Input.GetAxis("Vertical");   // S (-1) and W (+1)

        // 2. Rotate character with A/D (Clamped relative to start)
        if (Mathf.Abs(turnInput) > 0.01f)
        {
            currentRelativeAngle += turnInput * turnSpeed * Time.deltaTime;

            transform.rotation = Quaternion.Euler(0f, startYRotation + currentRelativeAngle, 0f);
        }

        // 3. Move Forward / Backward with W/S relative to facing direction
        Vector3 moveDirection = transform.forward * moveInput;
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // 4. Apply Gravity
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 5. Update Animator Parameters
        if (animator != null)
        {
            animator.SetFloat("Speed", moveInput);
        }
    }

    public void HealthManager(float damagePoints)
    {
        if (currentHealth > 0 && currentHealth <= game.maxHealth)
        {
            currentHealth += damagePoints;
            OnHealthChange?.Invoke();
        }
    }

    public void SatisfactionManager(float Points)
    {
        if (currentSatisfy >= 0 && currentSatisfy <= game.maxSatisfy)
        {
            currentSatisfy += Points;
            OnSatisfyChange?.Invoke();
        }
    }

    public void EnergyManager(float Points)
    {
        if (currentEnergy >= 0 && currentEnergy <= game.maxEnergy)
        {
            currentEnergy += Points;
            OnEnergyChange?.Invoke();
        }
    }

}