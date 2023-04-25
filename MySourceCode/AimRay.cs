using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class AimRay : MonoBehaviour
{
    //武器オブジェクト
    public GameObject weaponObj;
    //コックピット正面スクリーンのカメラ
    public Camera centerCam;
    //コックピットレバーのオブジェクト
    public GameObject grabbableObj;
    //そのオブジェクトの正面の向き
    public Vector3 weaponObjForward = Vector3.forward;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            grabbableObj= GameObject.Find("CockPit").transform.Find("main").transform.Find("rightarm").transform.Find("Handle").transform.Find("GrabbedPoint").gameObject;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().name == "TitleScene")
        {
            return;
        }
        //コックピットレバーを持っているときの処理
        if (grabbableObj.GetComponent<OVRGrabbable>().isGrabbed == true)
        {
            //レイを生成
            Ray ray = centerCam.ViewportPointToRay(new Vector2(0.5f, 0.5f));
            //レイがヒットしたオブジェクトに武器が向く処理
            if (Physics.Raycast(ray, out RaycastHit hit))//hitはraycastであたった物の情報を取得
            {
                //レイがヒットしたオブジェクトへの向きの計算
                Vector3 dir = hit.point - weaponObj.transform.position;
                //ヒットしたオブジェクトの方向に向く回転の計算
                Quaternion lookAtRotation = Quaternion.LookRotation(dir, Vector3.up);
                //向き回転の補正(向きを変えるオブジェクトのZ軸方向が正面の方向でない場合必要)
                Quaternion offsetRotation = Quaternion.FromToRotation(weaponObjForward, Vector3.forward);
                //補正込みで向きを変えるオブジェクトに回転を反映
                weaponObj.transform.rotation = lookAtRotation * offsetRotation;
            }
        }
    }
}
