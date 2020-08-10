using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : NetworkBehaviour {
    public GameObject shoulder;

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

        if(!hasAuthority) {
            enabled = false;            
        }
    }

    [ClientCallback]
    private void Update() {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);

        Vector3 rotation = Quaternion.FromToRotation(Vector3.right, GetMouseDirection()).eulerAngles;
        rotation.z = Mathf.RoundToInt(rotation.z / 22.5f) * 22.5f;
        shoulder.transform.localRotation = Quaternion.Euler(rotation);
    }

    [ClientCallback]
    private void OnEnable() => Controls.Enable();
    [ClientCallback]
    private void OnDisable() => Controls.Disable();

    public override void OnStartAuthority() {
        name = name + "_authority";

        Controls.Player.Block.started += BlockingStarted;
        Controls.Player.Block.canceled += BlockingStopped;
        //Controls.Player.Block.performed += BlockingStopped;
        Controls.Player.Attack.performed += ctx => attack.CmdAttack();
        Controls.Player.Interact.performed += ctx => StartInteraction();
    }

    [Client]
    private void BlockingStarted(CallbackContext ctx) {
        attack.CmdBlock(true);
    }

    [Client]
    private void BlockingStopped(CallbackContext ctx) {
        attack.CmdBlock(false);
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
