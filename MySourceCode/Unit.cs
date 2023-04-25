using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

/**
 * �@�̂̊�{�I�ȃp�����[�^�[
 * 
 * ���O�A
 * �@�̎��ʔԍ�
 * ���푕����
 * �ʏ펞�ړ����x
 * �u�[�X�g���ړ����x
 * �ő�u�[�X�g�e��
 * ���݂̃u�[�X�g�e��
 * �Ύ��ʍU���h���
 * �Ό����U���h���
 * �ΔM�U���h���
 * �ő�AP
 * ���݂�AP
 * �`�[�����ʗp�^�O
 * ���̋@�̂�I�����Ă���ڈ�
 * �v���C���[���ʔԍ�
 * �ő�J�X�^���p�[�c�����\��
 * �u�[�X�g�Q�[�W�����
 * �u�[�X�g�Q�[�W�ʏ펞�񕜗�
 * �u�[�X�g�Q�[�W�I�[�o�[�q�[�g���񕜗�
 */

public abstract class Unit : MonoBehaviourPunCallbacks
{
    protected string unitName;
    protected int unitID;
    protected int equipedWeaponNumber;
    protected float normalSpeed;
    protected float boostSpeed;
    protected float maxBoostCapacity;
    protected float currentBoostCapacity;
    protected float KEDEF;
    protected float TEDEF;
    protected float HEDEF;
    protected float maxArmorPoint;
    protected float currentArmorPoint;
    protected string teamTag;
    protected bool isSelecting;
    protected int playerID;
    protected int maxCustomPartCount;
    protected float boostConsumption;
    protected float boostNormalIncrease;
    protected float boostOverHeatIncrease;
    //Photon�ϐ�
    protected PhotonGameManager gameManager;

    bool state = false;
    float count = 0.0f;


    /**
     * ��������OnCollisionEnter���\�b�h�܂ł́A
     * �e�p�����[�^�̃Q�b�^�[�ƃZ�b�^�[
     */
    public string GetUnitName()
    {
        return this.unitName;
    }
    public float GetNormalSpeed()
    {
        return this.normalSpeed;
    }
    public float GetBoostSpeed()
    {
        return this.boostSpeed;
    }
    public float GetMaxBoostCapacity()
    {
        return this.maxBoostCapacity;
    }
    public float GetKEDEF()
    {
        return this.KEDEF;
    }
    public float GetTEDEF()
    {
        return this.TEDEF;
    }
    public float GetHEDEF()
    {
        return this.HEDEF;
    }
    public float GetMaxArmorPoint()
    {
        return this.maxArmorPoint;
    }
    public string GetTeamTag()
    {
        return this.teamTag;
    }
    public void SetTeamTag(string TM)
    {
        this.teamTag = TM;
        this.tag = this.teamTag;
    }
    public bool GetIsSelecting()
    {
        return this.isSelecting;
    }
    public void SetIsSelecting(bool IS)
    {
        this.isSelecting = IS;
    }
    public int GetPlayerID()
    {
        return this.playerID;
    }
    public void SetPlayerID(int PI)
    {
        this.playerID = PI;
    }
    public int GetUnitID()
    {
        return this.unitID;
    }
    public int GetMaxCustomPartCount()
    {
        return this.maxCustomPartCount;
    }
    public void SetMaxCustomPartCount(int MCPC)
    {
        this.maxCustomPartCount = MCPC;
    }
    public float GetCurrentBoostCapacity()
    {
        return this.currentBoostCapacity;
    }
    public void SetCurrentBoostCapacity(float CBC)
    {
        this.currentBoostCapacity = CBC;
    }
    public float GetCurrentArmorPoint()
    {
        return this.currentArmorPoint;
    }
    public void SetCurrentArmorPoint(float CAP)
    {
        this.currentArmorPoint = CAP;
    }
    public float GetBoostConsunmption()
    {
        return this.boostConsumption;
    }
    public float GetBoostNormalIncrease()
    {
        return this.boostNormalIncrease;
    }
    public float GetBoostOverHeatIncrease()
    {
        return this.boostOverHeatIncrease;
    }

    private void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().name == "BattleScene")
        {
            if (state)
            {
                count += Time.deltaTime;
                if (count >= 10.0f)
                {
                    count = 0.0f;
                    state = false;
                }
            }
        }
    }

    /*���̃��\�b�h�͋@�̂ɏe�e��~�T�C���Ȃǂ����������ۂ̏���*/
    private void OnCollisionEnter(Collision collision)
    {
        if (SceneManager.GetActiveScene().name == "BattleScene")
        {
            //�L�������I�u�W�F�N�g������(�t�B�[���h�̈ꕔ�Ȃ�)�̏ꍇ�����͍s��Ȃ�
            if (collision.gameObject.tag == "Field")
            {
                return;
            }
            //���������I�u�W�F�N�g�������̃`�[���̒e������`�[���̒e���𔻕�
            if (collision.gameObject.tag != teamTag)
            {
                Debug.Log("�ڐG");
                //���������e�̑����ɉ����Ėh��͂������Ȃǂ��đ��_���[�W���Z�o
                GameObject damageObject = collision.gameObject;
                GetDamage(damageObject, damageObject.GetComponent<Bullet>().actor);
                //photonView.RPC(nameof(GetDamage), RpcTarget.All, damageObject, damageObject.GetComponent<Bullet>().actor);
                //GetDamage(damageObject,damageObject.GetComponent<DamegeTestBullet>().actor);//���̂��̒e�Ŏ����Ƃ��ɂ͏���


            }
        }
    }
    //[PunRPC]
    public void GetDamage(GameObject dmgObj,int actor)
    {
        Debug.Log("myActor:" + PhotonNetwork.LocalPlayer.ActorNumber + " ShotActor:" + actor);
        PhotonGameManager PhotonManaeger = GameObject.Find("GameManger").GetComponent<PhotonGameManager>();
        int WA = dmgObj.GetComponent<Bullet>().GetWeaponAttribute();
        float totalDamage = 1;
        switch (WA)
        {
            case 0:
                totalDamage = dmgObj.GetComponent<Bullet>().GetDamage() - this.KEDEF;
                if (totalDamage < 1)
                {
                    totalDamage = 1;
                }
                break;
            case 1:
                totalDamage = dmgObj.GetComponent<Bullet>().GetDamage() - this.TEDEF;
                if (totalDamage < 1)
                {
                    totalDamage = 1;
                }
                break;
            case 2:
                totalDamage = dmgObj.GetComponent<Bullet>().GetDamage() - this.HEDEF;
                if (totalDamage < 1)
                {
                    totalDamage = 1;
                }
                break;
            case 3:
                totalDamage = dmgObj.GetComponent<Bullet>().GetDamage() - dmgObj.GetComponent<BazBullet>().GetCurrentDistance() / dmgObj.GetComponent<BazBullet>().GetMaxExplodDistance();
                if (totalDamage < 1)
                {
                    totalDamage = 1;
                }
                break;
        }
        //���݂̋@�̂�AP�ɔ��f
        if (photonView.IsMine)
        {
            photonView.RPC(nameof(NowDamage), RpcTarget.All,totalDamage);
        }
        //this.currentArmorPoint -= totalDamage;
        Debug.Log(this.gameObject);
        Debug.Log(this.currentArmorPoint);
        if (currentArmorPoint <= 0)
        {
            print("���@�j��m�F");
            RespawnOpe();
            if (photonView.IsMine)
            {
                //�f�X�̃C�x���g
                gameManager.ScoreGet(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1);
                //�L���̃C�x���g(Actor�̕������A�e�̕��o�҂ɕύX����)
                gameManager.ScoreGet(actor, 0, 1);
                //������������A����ĂȂ��l���ړ�����\������A
                gameManager.ScoreAppearGet(PhotonNetwork.LocalPlayer.ActorNumber, actor);
                //photonView.RPC("InstanceScore", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, actor);
                PhotonManaeger.GameOverGet();
            }
            //�����ɁA���j�̏���������
            /*if (photonView.IsMine)
            {
                //������������A����ĂȂ��l���ړ�����\������A
                RespawnOpe();
                photonView.RPC(nameof(InstanceScore), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, actor);
                PhotonManaeger.GameOverGet();
                //�����ɁA���j�̏���������
            }*/
        }else if (currentArmorPoint <= 1000&&!state)
        {
            state = true;
            this.transform.Find("MainCamera").GetComponent<BattleNoise>().SetDamageEffect();
        }
    }
    [PunRPC]
    public void NowDamage(float damage)
    {
        this.currentArmorPoint -= damage;
    }

    private void RespawnOpe()
    {
        GameObject spawn = GameObject.Find("PhotonNetwork").gameObject;
        GameObject gun = this.transform.Find("MainCamera").transform.Find("Gun").gameObject;

        Transform nextposi = spawn.GetComponent<SpawnManger>().spawnposition[Random.Range(0, spawn.GetComponent<SpawnManger>().spawnposition.Length)];
        this.gameObject.transform.position = nextposi.position;

        currentArmorPoint = maxArmorPoint;//���X�|�[���Ɠ����ɏ�Ԃ�������
        gun.GetComponent<BulletSpawn>().SetAmmo(gun.GetComponent<BulletSpawn>().GetMaxAmmo());
    }
    [PunRPC]
    void InstanceScore(int actored,int actor)
    {/*
        string deathname = gameManager.NameSearch(actored);
        string killname = gameManager.NameSearch(actor);

        GameObject parent = GameObject.Find("LeftCanvas").transform.Find("SetScoreList").gameObject;
        GameObject ScoreUI = Instantiate(GameObject.Find("PhotonNetwork").GetComponent<SpawnManger>().score, parent.transform, false);
        ScoreUI.GetComponent<Scoretransfrom>().Settext(killname,deathname);*/
    }
}
