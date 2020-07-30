using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Entity : MonoBehaviour
{
    public float initialHealth;
    public float currentHealth;
    public float speed;
    public float acceleration;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = initialHealth;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHealth < initialHealth) {
            currentHealth += Time.deltaTime;
        }
    }

    public void DoDamage(float amount) {
        currentHealth -= amount;

        if(currentHealth <= 0) {
            Destroy(gameObject);
        }
    }

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
