using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AI2Controller : NetworkBehaviour {
    public float speed = 2f;
    public float sense = 5;

    Entity entity;
    MeleeAttack attack;

    // Start is called before the first frame update
    void Start()
    {
        entity = GetComponent<Entity>();
        attack = GetComponent<MeleeAttack>();
    }

    [ServerCallback]
    void FixedUpdate() {
        Collider2D[] entities = Physics2D.OverlapCircleAll(transform.position, sense);
        foreach(Collider2D item in entities) {
            if(item.gameObject.GetComponent<PlayerController>()) {
                Vector2 offset = (item.transform.position - transform.position);
                entity.Move(offset.normalized);

                if(offset.magnitude <= 1) {
                    attack.Attack(offset.normalized);
                }
            }
        }
    }
}
