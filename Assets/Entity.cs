using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class Entity : NetworkBehaviour
{
    public float initialHealth;
    [SyncVar(hook = nameof(HandleHealthUpdated))]
    public float currentHealth;
    public float speed;
    public float acceleration;

    public event EventHandler<float> OnHealthChanged;

    private void Start() {
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
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
        //GetComponent<Rigidbody2D>().AddForce(direction * strength);
        GetComponent<Rigidbody2D>().velocity += (direction * strength);
    }
    
    public void Move(Vector2 direction) {
        Vector2 desiredSpeed = direction.normalized * speed;
        Vector2 change = desiredSpeed - GetComponent<Rigidbody2D>().velocity;
        if(change.magnitude > acceleration) {
            change = change.normalized * acceleration;
        }

        GetComponent<Rigidbody2D>().velocity += change;
    }
}
