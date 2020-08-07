using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AI2Controller : NetworkBehaviour {
    public GameObject shoulder;

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
        Collider2D[] entities = Physics2D.OverlapCircleAll(transform.position, sense, LayerMask.GetMask("Default", "Ignore Raycast"));
        foreach(Collider2D item in entities) {
            if(item.gameObject.GetComponent<PlayerController>()) {
                Vector2 offset = (item.transform.position - transform.position);
                entity.Move(offset.normalized);

                if(offset.magnitude <= 1) {
                    Vector3 rotation = Quaternion.FromToRotation(Vector3.right, offset).eulerAngles;
                    rotation.z = Mathf.RoundToInt(rotation.z / 22.5f) * 22.5f;
                    shoulder.transform.localRotation = Quaternion.Euler(rotation);

                    attack.Attack();
                }
            }
        }
    }
}
