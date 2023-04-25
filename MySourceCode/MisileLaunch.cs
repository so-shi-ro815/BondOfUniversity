using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MisileLaunch : Bullet
{
    private GameObject targetObj;//目標オブジェクト
    private Vector3 firstPosition;//飛んでいく最初の位置
    private float curvatureRadius;//曲がり具合(半径)
    private float damping;//空気抵抗(減衰率)
    /*Createメソッドをオーバーロード*/
    [PunRPC]
    public void Create(float dm, float sp, int ef,GameObject t,Vector3 fp,float cv ,float d ,int ac)
    {
        this.damage = dm;
        this.speed = sp;
        this.effectiveTime = ef;
        this.targetObj = t;
        this.firstPosition = fp;
        this.curvatureRadius = cv;
        this.damping = d;
        this.actor = ac;
        this.BulletLaunchSetting();
    }

    public void StartCreate(float dm, float sp, int ef, GameObject t, Vector3 fp, float cv, float d ,int ac)
    {
        photonView.RPC(nameof(Create), RpcTarget.All, dm, sp, ef, t,fp,cv,d,ac);
    }
    /*ミサイルの移動のためのパラメータ設定*/
    public override void BulletLaunchSetting()
    {
        Vector3 firstVelocity = new Vector3(0,0,0);//初速度
        this.GetComponent<Misile>().Emit(firstPosition, firstVelocity, speed, curvatureRadius, damping, targetObj);
    }
}
