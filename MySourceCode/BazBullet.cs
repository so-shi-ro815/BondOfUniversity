using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BazBullet : Bullet
{
    public GameObject EnemyListObj;

    public override void EndEffectiveTime()
    {
        float dist=float.MaxValue;
        for(int i=0;i<EnemyListObj.GetComponent<LockOnSystem>().Enemlist.Count;i++)
        {
            //敵との距離が設定されている爆破距離以下かチェック
            dist = Vector3.Distance(EnemyListObj.GetComponent<LockOnSystem>().Enemlist[i].transform.position, this.gameObject.transform.position);
            if(dist <= this.maxExplodDistance)
            {
                this.CurrentDistance = dist;
                if (dist > this.minExplodDistance)
                {
                    this.weaponAttribute = 3;
                }
                EnemyListObj.GetComponent<LockOnSystem>().Enemlist[i].GetComponent<Unit>().GetDamage(this.gameObject,PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }
        base.EndEffectiveTime();
    }
    /*ここから下は爆風処理のためのゲッタ*/
    public float GetMaxExplodDistance()
    {
        return this.maxExplodDistance;
    }
    public float GetCurrentDistance()
    {
        return this.CurrentDistance;
    }
    private void Awake()
    {
        EnemyListObj = GameObject.Find("MisilePod").gameObject;
    }
}
