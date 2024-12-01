using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float bounceBackForce = 5f;
    [SerializeField] public PlayerMovement playerMovement;

    [Header("References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Animator animator;
    private float currentHealth;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public GameObject Death_UI;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();

        ValidateReferences();
    }

    private void Start()
    {
        Death_UI = GameObject.Find("DeathUI");  // Look for the Death UI in the scene
        if (Death_UI == null)
        {
            Debug.LogError("Death UI object not found!");
        }
        else
        {
            Death_UI.SetActive(false);  // Make sure the death UI is hidden initially
        }

        InitializeHealth();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) // Temporary testing key for taking damage
        {
            TakeDamage(10f);
        }
    }

    private void ValidateReferences()
    {
        if (healthSlider == null)
        {
            Debug.LogError("Health Slider is not assigned.");
        }
        if (animator == null)
        {
            Debug.LogWarning("Animator is not assigned. Damage animation will not play.");
        }
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component is missing.");
        }
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component is missing.");
        }
    }

    private void InitializeHealth()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0) return;

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        UpdateHealthBar();

        // Check if health is zero and handle death
        if (currentHealth <= 0)
        {
            HandleDeath();
        }
        else
        {
            // Disable the PlayerMovement script temporarily
            playerMovement.enabled = false;
            StartCoroutine(DisableMovement());
        }
    }

    private IEnumerator DisableMovement()
    {
        ApplyBounceBack();
        PlayDamageAnimation();
        TriggerRedGlow();

        yield return new WaitForSeconds(1); // Disable movement for 1 second

        // Re-enable PlayerMovement after the delay
        playerMovement.enabled = true;
    }

    public void Heal(float amount)
    {
        if (amount <= 0) return;

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    private void ApplyBounceBack()
    {
        if (rb != null)
        {
            Vector2 bounceDirection = transform.localScale.x > 0 ? Vector2.left : Vector2.right;
            rb.AddForce(bounceDirection * bounceBackForce, ForceMode2D.Impulse);
        }
    }

    private void PlayDamageAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("TakeDamageTrigger");
        }
    }

    private void TriggerRedGlow()
    {
        if (spriteRenderer != null)
        {
            StartCoroutine(GlowRedEffect());
        }
    }

    private IEnumerator GlowRedEffect()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        float fadeDuration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            spriteRenderer.color = Color.Lerp(Color.red, originalColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = originalColor;  // Reset to the original color
    }

    private void HandleDeath()
    {
        if (animator != null)
        {
            animator.SetTrigger("Die");  // Trigger death animation
        }

        // Disable player movement and other components
        playerMovement.enabled = false;

        // Display the death UI
        if (Death_UI != null)
        {
            Death_UI.SetActive(true);  // Activate the death screen
        }

        // Optionally, destroy the player after a delay
        // Destroy(gameObject, 2f); // Uncomment to destroy the object after 2 seconds
    }
}
