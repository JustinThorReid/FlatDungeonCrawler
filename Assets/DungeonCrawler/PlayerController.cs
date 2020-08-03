using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour {
    Entity entity;
    MeleeAttack attack;
    Controls controls;
    Controls Controls
    {
        get {
            if(controls == null)
                return controls = new Controls();
            return controls;
        }
    }

    void Start() {
        attack = GetComponent<MeleeAttack>();
        entity = GetComponent<Entity>();
    }

    [ClientCallback]
    private void Update() {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
    }

    [ClientCallback]
    private void OnEnable() => Controls.Enable();
    [ClientCallback]
    private void OnDisable() => Controls.Disable();

    public override void OnStartAuthority() {
        this.enabled = true;
        name = "AUTHORITY";

        Controls.Player.Attack.performed += ctx => attack.CmdAttack(GetMouseDirection());
        Controls.Player.Interact.performed += ctx => StartInteraction();
    }

    [Client]
    private void StartInteraction() {
        CmdStartInteraction(GetMouseDirection());
    }

    [Command]
    private void CmdStartInteraction(Vector2 dir) {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 100, LayerMask.GetMask("Default"));
        if(hit) {
            IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();
            if(interactable != null) {
                interactable.ServerInteract(gameObject);
            }
        }
    }
    
    [ClientCallback]
    void FixedUpdate() {
        Vector2 targetVelocity = Controls.Player.Move.ReadValue<Vector2>();

        entity.Move(targetVelocity);
    }

    [Client]
    Vector3 GetMouseDirection() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Controls.Player.Mouse.ReadValue<Vector2>());
        Vector3 player = transform.position;
        player.z = 0;
        mousePos.z = 0;

        return (mousePos - player).normalized;
    }
}
