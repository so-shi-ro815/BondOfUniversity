using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MisileSpawn : BulletSpawn
{
    private GameObject targetObject;//�~�T�C���̖ڕW
    public float curvartureRadius;//�~�T�C���̋Ȃ���(���a)
    public float damping;//��C��R(������)

    /*���������e�ۂ̃p�����[�^��ݒ肷�郁�\�b�h���㏑��*/
    public override void BulletSetting(ref GameObject bulletObject)
    {
        bulletObject.tag = this.transform.root.tag;
        Vector3 cPos = bulletObject.transform.position;//���ݒn�擾
        bulletObject.GetComponent<MisileLaunch>().StartCreate(damage,speed,effectiveTime,targetObject,cPos,curvartureRadius,damping, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    /*�~�T�C���̔��ł����ڕW��ݒ�*/
    public void setTargetObject(GameObject to)
    {
        this.targetObject = to;
    }

    /*�֐����㏑��*/
    public override bool CheckWeponNum()
    {
        //���b�N�I�����������Ă��Ȃ��ƕ���̎g�p���o���Ȃ�
        if (this.GetComponent<MainLockOn>().GetIslock()==false)
        {
            return false;
        }
        return base.CheckWeponNum();
    }
}
