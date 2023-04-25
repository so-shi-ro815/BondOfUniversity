using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

/**
 * �@�̔ԍ�0
 * �u�~���[�W���v
 * AP3000
 * BC100
 * �ʏ펞�ړ����x�@50
 * �u�[�X�g�g�p���ړ����x�@120
 * ���푕�����@2
 * �J�X�^���p�[�c�����\���@3
 * �Ύ��ʍU���h��́@10
 * �Ό����U���h��� 10
 * �ΔM�U���h��� 10
 * �u�[�X�g�Q�[�W����� 7
 * �u�[�X�g�Q�[�W�ʏ펞�񕜗� 5
 * �u�[�X�g�Q�[�W�I�[�o�[�q�[�g���񕜗� 10
 */
public class Mirazyu : Unit//,IPunObservable
{
    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "BattleScene")
        {
            if (photonView.IsMine)
            {
                gameManager = GameObject.Find("GameManger").GetComponent<PhotonGameManager>();
                GameObject.Find("StatusCanvas").GetComponent<StatusText>().TextActive(this.gameObject);
            }
        }
    }
    Mirazyu()
    {
        this.unitName = "Mirazyu";
        this.unitID = 0;
        this.maxArmorPoint = 3000;
        this.currentArmorPoint = this.maxArmorPoint;
        this.maxBoostCapacity = 100;
        this.currentBoostCapacity = this.maxBoostCapacity;
        this.boostSpeed = 120;
        this.normalSpeed = 50;
        this.equipedWeaponNumber = 2;
        this.maxCustomPartCount = 3;
        this.KEDEF = 10;
        this.TEDEF = 10;
        this.HEDEF = 10;
        this.boostConsumption = 30;
        this.boostNormalIncrease = 5;
        this.boostOverHeatIncrease = 10;
    }
    /*float cpoint;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (photonView.IsMine)
        {
            if (cpoint != currentArmorPoint)//�A�[�}�[�|�C���g���ς�����Ƃ��̂ݏ������ݏ���
            {
                cpoint = currentArmorPoint;
                //�������̏������݂̓}�X�^�[�݂̂��������Ȃ��̂�
                if(PhotonNetwork.IsMasterClient)
                {
                    if (stream.IsWriting)
                    {
                        Debug.Log("�������݊���");
                        stream.SendNext(this.currentArmorPoint);
                    }
                }else if (!PhotonNetwork.IsMasterClient)
                {
                    //�����ŃI�[�i�[����������R�[�h��`��

                    if (stream.IsWriting)
                    {
                        Debug.Log("�������݊���");
                        stream.SendNext(this.currentArmorPoint);
                    }
                }
            }
            else if (cpoint == currentArmorPoint)
            {

            }
        }else if (!photonView.IsMine)
        {
            if ((float)stream.ReceiveNext() != currentArmorPoint)//��
            {
                Debug.Log("�ǂݎ�芮��");
                this.currentArmorPoint = (float)stream.ReceiveNext();
            }
        }
    }*/

}
