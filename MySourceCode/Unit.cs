using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

/**
 * 機体の基本的なパラメーター
 * 
 * 名前、
 * 機体識別番号
 * 武器装備数
 * 通常時移動速度
 * ブースト時移動速度
 * 最大ブースト容量
 * 現在のブースト容量
 * 対質量攻撃防御力
 * 対光線攻撃防御力
 * 対熱攻撃防御力
 * 最大AP
 * 現在のAP
 * チーム判別用タグ
 * その機体を選択している目印
 * プレイヤー識別番号
 * 最大カスタムパーツ装備可能数
 * ブーストゲージ消費量
 * ブーストゲージ通常時回復量
 * ブーストゲージオーバーヒート時回復量
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
    //Photon変数
    protected PhotonGameManager gameManager;

    bool state = false;
    float count = 0.0f;


    /**
     * ここからOnCollisionEnterメソッドまでは、
     * 各パラメータのゲッターとセッター
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

    /*このメソッドは機体に銃弾やミサイルなどがあたった際の処理*/
    private void OnCollisionEnter(Collision collision)
    {
        if (SceneManager.GetActiveScene().name == "BattleScene")
        {
            //有ったたオブジェクトが環境物(フィールドの一部など)の場合処理は行わない
            if (collision.gameObject.tag == "Field")
            {
                return;
            }
            //当たったオブジェクトが自分のチームの弾か相手チームの弾かを判別
            if (collision.gameObject.tag != teamTag)
            {
                Debug.Log("接触");
                //当たった弾の属性に応じて防御力を引くなどして総ダメージを算出
                GameObject damageObject = collision.gameObject;
                GetDamage(damageObject, damageObject.GetComponent<Bullet>().actor);
                //photonView.RPC(nameof(GetDamage), RpcTarget.All, damageObject, damageObject.GetComponent<Bullet>().actor);
                //GetDamage(damageObject,damageObject.GetComponent<DamegeTestBullet>().actor);//仮のもの弾で試すときには消す


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
        //現在の機体のAPに反映
        if (photonView.IsMine)
        {
            photonView.RPC(nameof(NowDamage), RpcTarget.All,totalDamage);
        }
        //this.currentArmorPoint -= totalDamage;
        Debug.Log(this.gameObject);
        Debug.Log(this.currentArmorPoint);
        if (currentArmorPoint <= 0)
        {
            print("実機破壊確認");
            RespawnOpe();
            if (photonView.IsMine)
            {
                //デスのイベント
                gameManager.ScoreGet(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1);
                //キルのイベント(Actorの部分を、銃の放出者に変更する)
                gameManager.ScoreGet(actor, 0, 1);
                //もしかしたら、やられてない人も移動する可能性あり、
                gameManager.ScoreAppearGet(PhotonNetwork.LocalPlayer.ActorNumber, actor);
                //photonView.RPC("InstanceScore", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, actor);
                PhotonManaeger.GameOverGet();
            }
            //ここに、撃破の処理を入れる
            /*if (photonView.IsMine)
            {
                //もしかしたら、やられてない人も移動する可能性あり、
                RespawnOpe();
                photonView.RPC(nameof(InstanceScore), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, actor);
                PhotonManaeger.GameOverGet();
                //ここに、撃破の処理を入れる
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

        currentArmorPoint = maxArmorPoint;//リスポーンと同時に状態を初期化
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
