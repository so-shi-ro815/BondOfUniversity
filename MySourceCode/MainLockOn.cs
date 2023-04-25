using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

/**
 * 現在の攻撃目標のAPと距離などを表示するための処理
 * あとミサイル発射に必要な攻撃目標取得の処理を最後に入れてある
 */
public class MainLockOn : MonoBehaviourPunCallbacks
{
    LockOnSystem.Enemyset mainTaget;//ロックオン中の敵の中で一番最初にロックオンが完了した敵
    public GameObject misilePod;//ミサイルを発射する武器
    float enemHp;//敵のAP
    public Color enehpColor;//敵のHP表記の色
    Text enemyHpText;//敵のAP表記
    GameObject targetMaker;//敵のAP表記のオブジェクト
    bool islock;//ロックオン中の敵の中で一番最初にロックオンが完了した敵がいるというフラグ
    Text targetDistance;//敵との距離の表記
    GameObject distanceMaker;//敵との距離の表記のオブジェクト
    public GameObject playerPosition;//自分のオブジェクト
    float displayEnemyAomor;//敵のAPを保持するための変数(攻撃があったた際に使う)
    //ここからはとりあえずの処置
    public Text subApText;
    public Text subDistText;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<MisileSpawn>().setTargetObject(null);
        if (photonView.IsMine)
        {
            subApText = GameObject.Find("SubEnemyHp").GetComponent<Text>();
            subDistText = GameObject.Find("SubTargetDistance").GetComponent<Text>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //現在の攻撃目標となりえるオブジェクトを取得(nullになる場合もある)
        mainTaget = misilePod.GetComponent<WeaponMisile>().TagetsSet();
        //現在の攻撃目標が設定されていない場合の処理
        if (islock == false)
        {
            //現在の攻撃目標の各パラメータを取得する処理
            if (mainTaget.Enem != null)
            {
                targetMaker = mainTaget.marker.transform.Find("EnemyHp").gameObject;
                enemyHpText = targetMaker.GetComponent<Text>();
                distanceMaker = mainTaget.marker.transform.Find("TargetDistance").gameObject;
                targetDistance = distanceMaker.GetComponent<Text>();
                targetDistance.color = enehpColor;
                enemyHpText.color = enehpColor;
                enemHp = mainTaget.Enem.GetComponent<Unit>().GetCurrentArmorPoint();
                displayEnemyAomor = enemHp;
                islock = true;
            }
        }
        //現在の攻撃目標が設定されている場合の処理
        if (islock == true)
        {
            //現在の攻撃目標がロックオン範囲外や破壊されたなどでnullとなった場合の処理
            if (mainTaget.Enem == null)
            {
                islock = false;
                enemyHpText.color = new Color(0f, 0f, 0f, 0f);
                targetDistance.color = new Color(0f, 0f, 0f, 0f);
                subDistText.color = targetDistance.color;
                subApText.color = enemyHpText.color;
                this.GetComponent<MisileSpawn>().setTargetObject(null);
            }
            if (mainTaget.Enem != null)
            {
                //現在の攻撃目標のAPに変化があった場合の処理
                enemHp = mainTaget.Enem.GetComponent<Unit>().GetCurrentArmorPoint();
                if (displayEnemyAomor != enemHp)
                {
                    displayEnemyAomor = Mathf.Lerp(displayEnemyAomor, enemHp, 0.1f);
                }

                //現在の攻撃目標のAPと距離を表示するオブジェクトを位置調整する処理
                Vector3 tM = targetMaker.transform.position;
                Vector3 dM = distanceMaker.transform.position;
                tM.x = mainTaget.marker.transform.position.x-0.3f;
                tM.y =  mainTaget.marker.transform.position.y;
                dM.x = mainTaget.marker.transform.position.x-0.3f;
                dM.y = mainTaget.marker.transform.position.y - 0.1f;
                targetMaker.transform.position = tM;
                distanceMaker.transform.position = dM;

                //現在の攻撃目標のAPと距離を表示するオブジェクトの色を緑に変え値を表示する処理
                enemyHpText.color = new Color(0f, 0f, 0f, 0f);
                targetDistance.color = new Color(0f, 0f, 0f, 0f);
                enemyHpText.text = "AP " + string.Format("{0000:F0}", displayEnemyAomor);
                targetDistance.text = ((int)Vector3.Distance(mainTaget.Enem.transform.position, playerPosition.transform.position)).ToString()+"m";
                subApText.text = enemyHpText.text;
                subDistText.text = targetDistance.text;
                subDistText.color = new Color(0f, 1f, 0f, 1f); ;
                subApText.color = new Color(0f, 1f, 0f, 1f); ;

                //ミサイルを発射するために攻撃目標を取得
                this.GetComponent<MisileSpawn>().setTargetObject(mainTaget.Enem);
            }
        }
    }

    /*ゲッター*/
    public bool GetIslock()
    {
        return islock;
    }
}
