using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Weaponクラスの子クラス
 * ロックオンにかかる時間やロックオンシステムとの連携が前提
 */
public class WeaponMisile : Weapon
{
    private GameObject target;//ミサイルの目標
    public GameObject locksys;//ロックオンシステムのスクリプトを持ったオブジェクト
    public int WeponLockOnTime = 50;//ロックオンまでの時間
    public LockOnSystem.Enemyset targetset;//ロックオンに必要な敵の情報がまとまった構造体
    // Start is called before the first frame update
    void Start()
    {
        target = null;
        targetset.Enem = null;
    }
    /*ロックオンまでの時間のゲッター*/
    public int getLockOnTime()
    {
        return WeponLockOnTime;
    }

    /*構造体のゲッター*/
    public LockOnSystem.Enemyset TagetsSet()
    {
        return targetset;
    }
    /*目標の設定*/
    public void SetTaget(GameObject g)
    {
        target = g;
    }
    public LockOnSystem.Enemyset enemyset;
    // Update is called once per frame
    void LateUpdate()
    {
        //ロックオン中の敵のの中で先にロックオン時間に達した敵を目標に設定
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
