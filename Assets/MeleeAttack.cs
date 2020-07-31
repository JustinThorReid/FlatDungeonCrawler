using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MeleeAttack : NetworkBehaviour
{
    public GameObject arm;
    public GameObject effect;
    public GameObject shoulder;
    Collider2D trigger;
    Collider2D player;
    float swingAmount = 120;

    Quaternion start;
    Quaternion end;
    bool isSwinging;
    float swingTime = 0;
    float cooldown = 0;

    // Start is called before the first frame update
    void Start()
    {
        effect.SetActive(false);
        arm.SetActive(false);

        trigger = shoulder.GetComponent<Collider2D>();
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
    public void CmdAttack(Vector2 direction) {
        Attack(direction);
    }

    [Server]
    public void Attack(Vector2 direction) {
        if(isSwinging || cooldown > 0)
            return;

        RpcAttackAnimation(direction);
        AttackAnimation(direction);
        
        List<Collider2D> results = new List<Collider2D>();
        Physics2D.OverlapCollider(trigger, new ContactFilter2D().NoFilter(), results);

        results.Remove(player);
        if(results.Count > 0) {
            results.ForEach(item => {
                Entity e = item.GetComponent<Entity>();
                if(e != null) {
                    e.DoDamage(10);
                    e.KnockBack(e.transform.position - transform.position, 5);
                }
            });
        }
    }

    [ClientRpc]
    public void RpcAttackAnimation(Vector2 direction) {
        AttackAnimation(direction);
    }

    private void AttackAnimation(Vector2 direction) {
        start = Quaternion.AngleAxis(0, Vector3.back);
        end = Quaternion.AngleAxis(swingAmount, Vector3.back);

        Quaternion offset = Quaternion.FromToRotation(Vector3.right, direction);
        shoulder.transform.localRotation = offset;

        isSwinging = true;
        swingTime = 0;
        cooldown = 0.25f;
        effect.SetActive(true);
        arm.SetActive(true);

    }
}
