using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour {
    Entity entity;
    MeleeAttack attack;
    Controls controls;

    void Start() {
        attack = GetComponent<MeleeAttack>();
        entity = GetComponent<Entity>();
    }

    public override void OnStartAuthority() {
        this.enabled = true;
        name = "AUTHORITY";

        controls = new Controls();
        controls.Player.Attack.performed += ctx => attack.Attack(GetMouseDirection());
        controls.Player.Enable();
    }
    
    void FixedUpdate() {
        Vector2 targetVelocity = controls.Player.Move.ReadValue<Vector2>();
        entity.Move(targetVelocity);
    }

    Vector3 GetMouseDirection() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(controls.Player.Mouse.ReadValue<Vector2>());
        Vector3 player = transform.position;
        player.z = 0;
        mousePos.z = 0;

        return (mousePos - player).normalized;
    }
}
