using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MisileSpawn : BulletSpawn
{
    private GameObject targetObject;//ミサイルの目標
    public float curvartureRadius;//ミサイルの曲がり具合(半径)
    public float damping;//空気抵抗(減衰率)

    /*生成される弾丸のパラメータを設定するメソッドを上書き*/
    public override void BulletSetting(ref GameObject bulletObject)
    {
        bulletObject.tag = this.transform.root.tag;
        Vector3 cPos = bulletObject.transform.position;//現在地取得
        bulletObject.GetComponent<MisileLaunch>().StartCreate(damage,speed,effectiveTime,targetObject,cPos,curvartureRadius,damping, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    /*ミサイルの飛んでいく目標を設定*/
    public void setTargetObject(GameObject to)
    {
        this.targetObject = to;
    }

    /*関数を上書き*/
    public override bool CheckWeponNum()
    {
        //ロックオンが完了していないと武器の使用が出いない
        if (this.GetComponent<MainLockOn>().GetIslock()==false)
        {
            return false;
        }
        return base.CheckWeponNum();
    }
}
