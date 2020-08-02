using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Entity : NetworkBehaviour
{
    private Rigidbody2D rb;

    public float initialHealth;
    [SyncVar(hook = nameof(HandleHealthUpdated))]
    public float currentHealth;
    public float speed;
    public float acceleration;

    public event EventHandler<float> OnHealthChanged;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();

        if(rb != null) {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    // Start is called before the first frame update
    public override void OnStartServer()
    {
        currentHealth = initialHealth;
    }

    [Client]
    private void HandleHealthUpdated(float oldValue, float newValue) {
        OnHealthChanged?.Invoke(this, currentHealth);
    }

    // Update is called once per frame
    [ServerCallback]
    void Update()
    {
        if(currentHealth < initialHealth) {
            currentHealth += Time.deltaTime;
        }
    }

    [Server]
    public void DoDamage(float amount) {
        currentHealth -= amount;

        if(currentHealth <= 0) {
            Destroy(gameObject);
        }
    }

    [Server]
    public void KnockBack(Vector2 direction, float strength) {
        if(rb != null) {
            GetComponent<Rigidbody2D>().velocity = direction * strength;
        }
        //GetComponent<Rigidbody2D>().MovePosition(new Vector2(transform.position.x, transform.position.y) + (direction * strength));
    }
    
    [Client]
    public void Move(Vector2 direction) {
        if(rb == null)
            return;

        Vector2 desiredSpeed = direction.normalized * speed;
        Vector2 change = desiredSpeed - GetComponent<Rigidbody2D>().velocity;
        if(change.magnitude > acceleration) {
            change = change.normalized * acceleration;
        }

        GetComponent<Rigidbody2D>().velocity += change;
    }
}
