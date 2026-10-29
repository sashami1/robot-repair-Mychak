using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 3.0f;
    public bool vertical;
    public float changeTime = 3.0f;
    public bool isBroken { get { return broken; } }

    private Rigidbody2D rigidbody2d;
    private Animator animator;
    private float timer;
    private int direction = 1;

    // ‘лаг: чи зламаний робот (за замовчуванн€м - так)
    private bool broken = true;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // якщо робот вже полагоджений Ч припин€Їмо в≥дл≥к таймера
        if (!broken)
        {
            return;
        }

        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }

    void FixedUpdate()
    {
        // якщо робот полагоджений Ч в≥н зупин€Їтьс€ ≥ не рухаЇтьс€
        if (!broken)
        {
            return;
        }

        Vector2 position = rigidbody2d.position;

        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction;
            if (animator != null)
            {
                animator.SetFloat("Move X", 0);
                animator.SetFloat("Move Y", direction);
            }
        }
        else
        {
            position.x = position.x + Time.deltaTime * speed * direction;
            if (animator != null)
            {
                animator.SetFloat("Move X", direction);
                animator.SetFloat("Move Y", 0);
            }
        }

        rigidbody2d.MovePosition(position);
    }

    // 1. Ў ќƒј √–ј¬÷ё: ѕри торканн≥ гравц€ зменшуЇмо його здоров'€
    void OnCollisionEnter2D(Collision2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }

    // 2. ѕќЋј√ќƒ∆≈ЌЌя –ќЅќ“ј: ¬икликаЇтьс€ з скрипта Projectile при влучанн≥
    public void Fix()
    {
        broken = false;
        rigidbody2d.simulated = false; // ¬имикаЇмо ф≥зику ворога, щоб в≥н не штовхавс€

        if (animator != null)
        {
            animator.SetTrigger("Fixed"); // «апускаЇмо перех≥д на ан≥мац≥ю полагодженого робота
        }
    }
}