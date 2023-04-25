using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Weapon�N���X�̎q�N���X
 * ���b�N�I���ɂ����鎞�Ԃ⃍�b�N�I���V�X�e���Ƃ̘A�g���O��
 */
public class WeaponMisile : Weapon
{
    private GameObject target;//�~�T�C���̖ڕW
    public GameObject locksys;//���b�N�I���V�X�e���̃X�N���v�g���������I�u�W�F�N�g
    public int WeponLockOnTime = 50;//���b�N�I���܂ł̎���
    public LockOnSystem.Enemyset targetset;//���b�N�I���ɕK�v�ȓG�̏�񂪂܂Ƃ܂����\����
    // Start is called before the first frame update
    void Start()
    {
        target = null;
        targetset.Enem = null;
    }
    /*���b�N�I���܂ł̎��Ԃ̃Q�b�^�[*/
    public int getLockOnTime()
    {
        return WeponLockOnTime;
    }

    /*�\���̂̃Q�b�^�[*/
    public LockOnSystem.Enemyset TagetsSet()
    {
        return targetset;
    }
    /*�ڕW�̐ݒ�*/
    public void SetTaget(GameObject g)
    {
        target = g;
    }
    public LockOnSystem.Enemyset enemyset;
    // Update is called once per frame
    void LateUpdate()
    {
        //���b�N�I�����̓G�̂̒��Ő�Ƀ��b�N�I�����ԂɒB�����G��ڕW�ɐݒ�
        LockOnSystem l = locksys.GetComponent<LockOnSystem>();
        if (target == null)
        {
            if (l.Enemanagement.Count > 0)
            {
                for (int i = 0; i < l.Enemanagement.Count; i++)
                {
                    if (l.Enemanagement[i].Enem != null)
                    {
                        if (l.Enemanagement[i].isLockOn)
                        {
                            enemyset = l.Enemanagement[i];
                            targetset = enemyset;
                            target = l.Enemanagement[i].Enem;
                            break;
                        }
                    }
                }
            }
        }
    }
}
