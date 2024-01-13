using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float Hp;
    public float MaxHp;
    public float Damage;
    public float AtackSpeed;
    public float HardAttackSpeed = 2;
    private float HardAttackCurTime = 0;
    public float AttackRange = 2;
    private bool Is_HardAttack = false;
    private List<Enemie> closestEnemies;
    [SerializeField] Image HardAttackTime;
    [SerializeField] TextMeshProUGUI AppTime;
    private float curtime = 0;

    private float lastAttackTime = 0;
    private bool isDead = false;
    public Animator AnimatorController;
    [SerializeField] NavMeshAgent Agent;
    private int ClickNum = 0;

    private void Start()
    {
        Hp = MaxHp;
        lastAttackTime = -AtackSpeed;
    }

    private void Update()
    {
        Move_Tick();
        curtime += Time.deltaTime;
        //var texta = TimeSpan.FromSeconds(Time.time);
        AppTime.text = $"{TimeSpan.FromSeconds(Time.time).ToString("mm")}:{TimeSpan.FromSeconds(Time.time).ToString("ss")} " +
            $"{TimeSpan.FromSeconds(curtime).ToString("mm")}:{TimeSpan.FromSeconds(curtime).ToString("ss")}";
        if (isDead)
        {
            return;
        }

        if (Hp <= 0)
        {
            Die();
            return;
        }


        var enemies = SceneManager.Instance.Enemies;
        Enemie closestEnemie = null;
        closestEnemies = new();

        for (int i = 0; i < enemies.Count; i++)
        {
            var enemie = enemies[i];
            if (enemie == null)
            {
                continue;
            }

            if (closestEnemie == null)
            {
                closestEnemie = enemie;
                var Distance = Vector3.Distance(transform.position, enemies[i].transform.position);
                if (Distance < AttackRange)
                {
                    closestEnemies.Add(enemies[i]);
                }
                continue;
            }

            var distance = Vector3.Distance(transform.position, enemies[i].transform.position);
            //Debug.Log(distance);
            var closestDistance = Vector3.Distance(transform.position, closestEnemie.transform.position);

            if (distance < AttackRange)
            {
                closestEnemies.Add(enemies[i]);
            }

            if (distance < closestDistance)
            {
                closestEnemie = enemie;
            }
        }
        //Debug.Log(closestEnemies.Count);
        Check_Hard_Attack_Button(closestEnemies);
        Check_Attack_Button(closestEnemie);
        Edit_Hard_Attack_Button();
        //if (closestEnemie != null)
        //{
        //    var distance = Vector3.Distance(transform.position, closestEnemie.transform.position);
        //    if (distance <= AttackRange)
        //    {
        //        if (Time.time - lastAttackTime > AtackSpeed)
        //        {
        //            //transform.LookAt(closestEnemie.transform);
        //            transform.transform.rotation = Quaternion.LookRotation(closestEnemie.transform.position - transform.position);

        //            lastAttackTime = Time.time;
        //            closestEnemie.Hp -= Damage;
        //            AnimatorController.SetTrigger("Attack");
        //        }
        //    }
        //}
    }

    private void Edit_Hard_Attack_Button()
    {
        if (HardAttackCurTime < HardAttackSpeed)
        {
            HardAttackCurTime += Time.deltaTime;
            HardAttackTime.fillAmount = HardAttackCurTime / HardAttackSpeed;
            Is_HardAttack = false;
        }
        else
        {
            Is_HardAttack = true;
        }
    }

    private void Check_Hard_Attack_Button(List<Enemie> closestEnemies)
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (Is_HardAttack == true)
            {
                Is_HardAttack = false;
                if (closestEnemies.Count != 0)
                {
                    for (int i = 0; i < closestEnemies.Count; i++)
                    {
                        if (Time.time - lastAttackTime > AtackSpeed)
                        {
                            HardAttackCurTime = 0;
                            //transform.transform.rotation = Quaternion.LookRotation(closestEnemies[i].transform.position - transform.position);
                            lastAttackTime = Time.time;
                            closestEnemies[i].Hp -= Damage * 2;
                            AnimatorController.SetTrigger("Hard Attack");
                        }
                    }
                }
            }
        }
    }

    private void Check_Attack_Button(Enemie closestEnemie)
    {
        if (Input.GetMouseButtonDown(0))
        {
            ClickNum = 1;
        }
        if (ClickNum > 0)
        {
            if (closestEnemie != null)
            {
                var distance = Vector3.Distance(transform.position, closestEnemie.transform.position);
                if (distance <= AttackRange)
                {
                    if (Time.time - lastAttackTime > AtackSpeed)
                    {
                        transform.transform.rotation = Quaternion.LookRotation(closestEnemie.transform.position - transform.position);
                        lastAttackTime = Time.time;
                        closestEnemie.Hp -= Damage;
                        AnimatorController.SetTrigger("Attack");
                        ClickNum--;
                    }
                }
                else
                {
                    if (Time.time - lastAttackTime > AtackSpeed)
                    {
                        lastAttackTime = Time.time;
                        AnimatorController.SetTrigger("Attack");
                        ClickNum--;
                    }
                }
            }
        }
    }

    private void Move_Tick()
    {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        if (x != 0 || y != 0)
        {
            var moveposition = transform.position + new Vector3(x, 0, y);
            Agent.SetDestination(moveposition);
            AnimatorController.SetFloat("Speed", Agent.speed);
        }
        else
        {
            Agent.SetDestination(transform.position);
            AnimatorController.SetFloat("Speed", 0);
        }
    }

    private void Die()
    {
        isDead = true;
        AnimatorController.SetTrigger("Die");

        SceneManager.Instance.GameOver();
    }


}
