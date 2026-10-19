using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction;

    private Rigidbody2D rigidbody2d;
    private Animator animator;
    private Vector2 move;

    [Header("Movement Settings")]
    public float speed = 3.0f;

    [Header("Health Settings")]
    public int maxHealth = 5;
    public int health { get { return currentHealth; } }
    private int currentHealth;

    public float timeInvincible = 2.0f;
    private bool isInvincible;
    private float damageCooldown;

    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public float shootForce = 300f;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;
    private Vector2 lookDirection = new Vector2(1, 0);

    void Start()
    {
        MoveAction.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        move = MoveAction.ReadValue<Vector2>();

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        if (animator != null)
        {
            animator.SetFloat("Look X", lookDirection.x);
            animator.SetFloat("Look Y", lookDirection.y);
            animator.SetFloat("Speed", move.magnitude);
        }

        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0)
            {
                isInvincible = false;
            }
        }

        // Універсальна перевірка натискання клавіші C для будь-якої системи вводу
        bool cKeyPressed = false;
        if (Keyboard.current != null)
        {
            cKeyPressed = Keyboard.current.cKey.wasPressedThisFrame;
        }

        if (cKeyPressed && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Launch();
        }
    }

    void FixedUpdate()
    {
        Vector2 position = (Vector2)rigidbody2d.position + move * speed * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }

    void Launch()
    {
        if (projectilePrefab == null) return;

        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Launch(lookDirection, shootForce);
        }

        if (animator != null)
        {
            animator.SetTrigger("Launch");
        }
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
            {
                return;
            }
            isInvincible = true;
            damageCooldown = timeInvincible;

            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);
    }
}