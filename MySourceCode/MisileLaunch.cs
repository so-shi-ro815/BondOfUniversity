using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MisileLaunch : Bullet
{
    private GameObject targetObj;//�ڕW�I�u�W�F�N�g
    private Vector3 firstPosition;//���ł����ŏ��̈ʒu
    private float curvatureRadius;//�Ȃ���(���a)
    private float damping;//��C��R(������)
    /*Create���\�b�h���I�[�o�[���[�h*/
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
    /*�~�T�C���̈ړ��̂��߂̃p�����[�^�ݒ�*/
    public override void BulletLaunchSetting()
    {
        Vector3 firstVelocity = new Vector3(0,0,0);//�����x
        this.GetComponent<Misile>().Emit(firstPosition, firstVelocity, speed, curvatureRadius, damping, targetObj);
    }
}
