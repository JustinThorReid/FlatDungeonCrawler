using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    Entity entity;
    MeleeAttack attack;

    void Start() {
        attack = GetComponent<MeleeAttack>();
        entity = GetComponent<Entity>();
    }

    private void Update() {
        if(Input.GetMouseButtonDown(0)) {
            attack.Attack(GetMouseDirection());
        }
    }

    void FixedUpdate() {
        Vector2 targetVelocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        entity.Move(targetVelocity);
    }

    Vector3 GetMouseDirection() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 player = transform.position;
        player.z = 0;
        mousePos.z = 0;

        return (mousePos - player).normalized;
    }
}
