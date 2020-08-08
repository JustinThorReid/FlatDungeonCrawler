using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Entity))]
public class MeleeAttack : NetworkBehaviour
{
    public GameObject arm;
    public GameObject effect;
    public GameObject shoulder;
    public Collider2D attackArea;
    Collider2D player;
    float swingAmount = 120;

    private Entity entity;

    Quaternion start;
    Quaternion end;
    bool isSwinging;
    float swingTime = 0;
    float cooldown = 0;

    [SyncVar(hook = nameof(BlockingChanged))]
    bool isBlocking = false;
    [SerializeField]
    private GameObject shieldSprite;
    public float shieldAngle = 45;

    // Start is called before the first frame update
    void Start()
    {
        effect.SetActive(false);
        arm.SetActive(false);
        entity = GetComponent<Entity>();

        player = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update() {
        cooldown -= Time.deltaTime;
        if(isSwinging) {
            swingTime += Time.deltaTime * 5;
            arm.transform.localRotation = Quaternion.Slerp(start, end, swingTime);

            if(swingTime >= 1) {
                isSwinging = false;
                arm.transform.localRotation = Quaternion.identity;
                effect.SetActive(false);
                arm.SetActive(false);
            }
        }
    }

    [Command]
    public void CmdBlock(bool state) {
        Block(state);
    }

    [Server]
    public void Block(bool state) {
        isBlocking = state;
    }

    [Client]
    private void BlockingChanged(bool oldValue, bool newValue) {
        shieldSprite.SetActive(newValue);
    }

    [Command]
    public void CmdAttack() {
        Attack();
    }

    [Server]
    public void Attack() {
        if(isSwinging || cooldown > 0)
            return;
        if(entity.isStunned)
            return;

        if(!isBlocking) {
            RpcAttackAnimation();
        }

        List<Collider2D> results = new List<Collider2D>();
        Physics2D.OverlapCollider(attackArea, new ContactFilter2D().NoFilter(), results);

        results.Remove(player);
        if(results.Count > 0) {
            results.ForEach(item => {
                Entity e = item.GetComponent<Entity>();
                if(e != null) {
                    if(isBlocking) {
                        e.KnockBack(e.transform.position - transform.position, 5);
                    } else {
                        MeleeAttack otherMob = item.GetComponent<MeleeAttack>();
                        if(otherMob == null || !otherMob.IsPositionBlocked(transform.position)) {
                            e.DoDamage(25);
                        }
                    }
                }
            });
        }
    }

    [ClientRpc]
    public void RpcAttackAnimation() {
        AttackAnimation();
    }

    private void AttackAnimation() {
        start = Quaternion.AngleAxis(0, Vector3.back);
        end = Quaternion.AngleAxis(swingAmount, Vector3.back);

        isSwinging = true;
        swingTime = 0;
        cooldown = 0.5f;
        effect.SetActive(true);
        arm.SetActive(true);
    }

    /** Is this mob currently shielding/blocking attacks from remotePosition */
    private bool IsPositionBlocked(Vector2 remotePosition) {
        if(!isBlocking)
            return false;

        Vector2 direction = (remotePosition - new Vector2(transform.position.x, transform.position.y)).normalized;
        float angle = Quaternion.Angle(shoulder.transform.localRotation, Quaternion.FromToRotation(Vector3.right, direction));

        Debug.Log(angle);
        return Mathf.Abs(angle) <= shieldAngle;
    }
}
