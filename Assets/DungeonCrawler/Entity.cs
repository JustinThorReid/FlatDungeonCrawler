using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.UI;
using TMPro;

public class Entity : NetworkBehaviour
{
    private Rigidbody2D rb;
    [SerializeField]
    private GameObject effectSprite;

    public int initialHealth;
    [SyncVar(hook = nameof(HandleHealthUpdated))]
    [SerializeField]
    private int currentHealth;
    private float healing = 0;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float acceleration;

    public event EventHandler<int> OnHealthChanged;
    [SerializeField]
    private Slider healthBar;
    [SerializeField]
    private TMP_Text nameDisplay;
    [SyncVar(hook = nameof(HandleNameChange))]
    [SerializeField]
    public string displayName;

    [SyncVar(hook = nameof(HandleStunUpdated))]
    public bool isStunned = false;
    private float remainingStunTime = 0;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();

        if(rb != null) {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        if(healthBar != null) {
            OnHealthChanged += (sender, health) => {
                healthBar.value = currentHealth / (float)initialHealth;
                healthBar.transform.localScale = currentHealth == initialHealth ? Vector3.zero : Vector3.one;
            };

            healthBar.value = currentHealth / (float)initialHealth;
            healthBar.transform.localScale = currentHealth == initialHealth ? Vector3.zero : Vector3.one;
        }
    }

    public void HandleNameChange(string oldName, string newName) {
        if(nameDisplay != null) {
            nameDisplay.text = newName;
        }
    }

    // Start is called before the first frame update
    public override void OnStartServer()
    {
        currentHealth = initialHealth;
    }

    [Client]
    private void HandleHealthUpdated(int oldValue, int newValue) {
        OnHealthChanged?.Invoke(this, currentHealth);
    }

    // Update is called once per frame
    [ServerCallback]
    void Update()
    {
        if(currentHealth < initialHealth) {
            healing += Time.deltaTime;
            if(healing > 1) {
                currentHealth += Mathf.FloorToInt(healing);
                healing -= Mathf.FloorToInt(healing);
            }
        }

        if(remainingStunTime > 0) {
            remainingStunTime -= Time.deltaTime;

            if(remainingStunTime <= 0)
                isStunned = false;
        }
    }

    [Server]
    public void DoDamage(int amount) {
        currentHealth -= amount;

        if(currentHealth <= 0) {
            if(GetComponent<PlayerController>() != null) {
                FindObjectOfType<PlayerManager>().PlayerDeath(gameObject);
            }

            Destroy(gameObject);
        }
    }

    [Server]
    public void KnockBack(Vector2 direction, float strength) {
        if(rb != null) {
            GetComponent<Rigidbody2D>().velocity = direction * strength;
        }

        Stun(2);
        //GetComponent<Rigidbody2D>().MovePosition(new Vector2(transform.position.x, transform.position.y) + (direction * strength));
    }

    [Server]
    private void Stun(float durationSeconds) {
        isStunned = true;
        remainingStunTime = durationSeconds;
    }
    
    public void Move(Vector2 direction) {
        if(rb == null)
            return;
        if(isStunned)
            return;

        Vector2 desiredSpeed = direction.normalized * speed;
        Vector2 change = desiredSpeed - GetComponent<Rigidbody2D>().velocity;
        if(change.magnitude > acceleration) {
            change = change.normalized * acceleration;
        }

        GetComponent<Rigidbody2D>().velocity += change;
    }

    private void HandleStunUpdated(bool oldValue, bool newValue) {
        effectSprite.SetActive(newValue);
    }
}
