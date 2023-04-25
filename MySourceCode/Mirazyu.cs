using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

/**
 * 機体番号0
 * 「ミラージュ」
 * AP3000
 * BC100
 * 通常時移動速度　50
 * ブースト使用時移動速度　120
 * 武器装備数　2
 * カスタムパーツ装備可能数　3
 * 対質量攻撃防御力　10
 * 対光線攻撃防御力 10
 * 対熱攻撃防御力 10
 * ブーストゲージ消費量 7
 * ブーストゲージ通常時回復量 5
 * ブーストゲージオーバーヒート時回復量 10
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
            if (cpoint != currentArmorPoint)//アーマーポイントが変わったときのみ書き込み処理
            {
                cpoint = currentArmorPoint;
                //同期分の書き込みはマスターのみしか書けないので
                if(PhotonNetwork.IsMasterClient)
                {
                    if (stream.IsWriting)
                    {
                        Debug.Log("書き込み完了");
                        stream.SendNext(this.currentArmorPoint);
                    }
                }else if (!PhotonNetwork.IsMasterClient)
                {
                    //ここでオーナー権限を譲るコードを描く

                    if (stream.IsWriting)
                    {
                        Debug.Log("書き込み完了");
                        stream.SendNext(this.currentArmorPoint);
                    }
                }
            }
            else if (cpoint == currentArmorPoint)
            {

            }
        }else if (!photonView.IsMine)
        {
            if ((float)stream.ReceiveNext() != currentArmorPoint)//他
            {
                Debug.Log("読み取り完了");
                this.currentArmorPoint = (float)stream.ReceiveNext();
            }
        }
    }*/

}
