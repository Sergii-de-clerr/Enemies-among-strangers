using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class BigEnemy : Enemie
    {
        [SerializeField] private Enemie Enemy_Prefab;
        protected override void Die()
        {
            Instantiate(Enemy_Prefab, transform.position + transform.right, Quaternion.identity);
            Instantiate(Enemy_Prefab, transform.position - transform.right, Quaternion.identity);
            SceneManager.Instance.RemoveEnemie(this);
            isDead = true;
            AnimatorController.SetTrigger("Die");
            SceneManager.Instance.Player.Hp += RegenPlHp;
        }
    }
}
