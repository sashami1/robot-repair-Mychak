using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction;
    public InputAction LaunchAction;
    public InputAction TalkAction;

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

    private Vector2 moveDirection = new Vector2(1, 0);
    private NonPlayerCharacter lastNonPlayerCharacter;

    void Start()
    {
        MoveAction.Enable();
        LaunchAction.Enable();
        TalkAction.Enable();

        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        move = MoveAction.ReadValue<Vector2>();

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
        }

        if (animator != null)
        {
            animator.SetFloat("Look X", moveDirection.x);
            animator.SetFloat("Look Y", moveDirection.y);
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

        // Постріл на C або через LaunchAction
        bool cKeyPressed = Keyboard.current != null && Keyboard.current.cKey.wasPressedThisFrame;
        if (LaunchAction.WasPressedThisFrame() || cKeyPressed)
        {
            Launch();
        }

        // Перевірка Raycast для NPC та бульбашки з підказкою (Крок 14)
        RaycastHit2D hit = Physics2D.Raycast(
            rigidbody2d.position + Vector2.up * 0.2f,
            moveDirection,
            1.5f,
            LayerMask.GetMask("NPC")
        );

        if (hit.collider != null)
        {
            NonPlayerCharacter npc = hit.collider.GetComponent<NonPlayerCharacter>();
            if (npc != null)
            {
                if (npc.dialogueBubble != null)
                {
                    npc.dialogueBubble.SetActive(true);
                }
                lastNonPlayerCharacter = npc;
                FindFriend();
            }
        }
        else
        {
            if (lastNonPlayerCharacter != null)
            {
                if (lastNonPlayerCharacter.dialogueBubble != null)
                {
                    lastNonPlayerCharacter.dialogueBubble.SetActive(false);
                }
                lastNonPlayerCharacter = null;
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 position = (Vector2)rigidbody2d.position + move * speed * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible) return;

            isInvincible = true;
            damageCooldown = timeInvincible;

            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        if (UIHandler.instance != null)
        {
            UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
        }
    }

    void Launch()
    {
        if (projectilePrefab == null) return;

        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Launch(moveDirection, shootForce);
        }

        if (animator != null)
        {
            animator.SetTrigger("Launch");
        }
    }

    void FindFriend()
    {
        bool xKeyPressed = Keyboard.current != null && Keyboard.current.xKey.wasPressedThisFrame;
        if (TalkAction.WasPressedThisFrame() || xKeyPressed)
        {
            if (UIHandler.instance != null)
            {
                UIHandler.instance.DisplayDialogue();
            }
        }
    }
}