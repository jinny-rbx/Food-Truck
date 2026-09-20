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

        startYRotation = transform.eulerAngles.y;
    }

    void Update()
    {
        // 1. Check Dialogue Lock
        if (DialogueController.instance != null && DialogueController.instance.IsDialogueActive)
        {
            if (!DialogueController.instance.IsWaitingForMovementInput)
            {
                // Reset animator speed to idle when frozen
                if (animator != null)
                {
                    animator.SetFloat("Speed", 0f);
                }
                return; // Lock movement
            }
        }

        // 2. Input detection
        float turnInput = Input.GetAxis("Horizontal"); // A (-1) and D (+1)
        float moveInput = Input.GetAxis("Vertical");   // S (-1) and W (+1)

        // 3. Turn Character
        if (Mathf.Abs(turnInput) > 0.01f)
        {
            currentRelativeAngle += turnInput * turnSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0f, startYRotation + currentRelativeAngle, 0f);
        }

        // 4. Move Forward / Backward
        Vector3 moveDirection = transform.forward * moveInput;
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // 5. Apply Gravity
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 6. Update Animator
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
            currentSatisfy = Mathf.Clamp(currentSatisfy, 0f, game.maxSatisfy);
            OnSatisfyChange?.Invoke();
        }
    }

    public void EnergyManager(float Points)
    {
        if (currentEnergy >= 0 && currentEnergy <= game.maxEnergy)
        {
            currentEnergy += Points;
            currentEnergy = Mathf.Clamp(currentEnergy, 0f, game.maxEnergy);
            OnEnergyChange?.Invoke();
        }
    }
}