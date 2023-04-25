using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class RArmMove : MonoBehaviourPunCallbacks
{
    //レバー移動スピード
    public float leverSpeed = 3f;
    //レバーオブジェクト初期位置
    private Vector3 firstLeverPosition;
    //レバーオブジェクト
    public GameObject leverObject;
    //レバー固定化オブジェクト
    public GameObject handleObj;
    //機体オブジェクト
    public GameObject moventObj;
    //つかみ可能オブジェクト
    public GameObject grabbableObj;
    //機体アニメーション
    public Animator animator;
    //機体の基礎速さ
    public float addPower = 25f;
    //レバー移動による速度の反映点
    public Transform addPowerPoint;
    //機体のカメラオブジェクト
    public GameObject camObj;
    //ブースト移動倍率
    public float dashRate = 1.5f;
    //ブースト使用フラグ
    private bool boostFlag = false;
    //オーバーヒートフラグ
    private bool boostLimitFlag = false;
    public float rightFrontLeverPosition=0.3f;
    public float rightFrontBoostLeverPosition = 0.5f;
    public float rightBackLeverPosition = 0.2f;
    public float rightBackBoostLeverPosition = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "TitleScene")
        {
            return;
        }
        else if (SceneManager.GetActiveScene().name == "MenuScene")
        {
            return;
        }
        else if (SceneManager.GetActiveScene().name== "ResultScene")
        {
            return;
        }
        if (PhotonNetwork.IsConnected)
        {
            leverObject = GameObject.Find("CockPit").transform.Find("main").transform.Find("rightarm").gameObject;
            handleObj = GameObject.Find("CockPit").transform.Find("main").transform.Find("rightarm").transform.Find("Handle").gameObject;
            grabbableObj = GameObject.Find("CockPit").transform.Find("main").transform.Find("rightarm").transform.Find("Handle").transform.Find("GrabbedPoint").gameObject;
        }
        if (!photonView.IsMine)
        {
            GameObject Cameras = this.transform.Find("MainCamera").gameObject;
            //自分以外の機体時の処理(AudioListernerは1個のみなので)
            Cameras.transform.Find("CameraC").GetComponent<AudioListener>().gameObject.SetActive(false);
            Cameras.transform.Find("CameraC").GetComponent<Camera>().gameObject.SetActive(false);
            Cameras.transform.Find("CameraL").GetComponent<Camera>().gameObject.SetActive(false);
            Cameras.transform.Find("CameraR").GetComponent<Camera>().gameObject.SetActive(false);
        }
        this.firstLeverPosition = leverObject.transform.position;
        this.boostFlag = false;
        this.boostLimitFlag = false;
        //全てのアニメーションをオフに
        animator.SetBool("front", false);
        animator.SetBool("back", false);
        animator.SetBool("right", false);
        animator.SetBool("left", false);
        animator.SetBool("top", false);
        animator.SetBool("under", false);
        animator.SetBool("dash", false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(SceneManager.GetActiveScene().name == "TitleScene")
        {
            return;
        }
        else if (SceneManager.GetActiveScene().name == "MenuScene")
        {
            return;
        }
        else if (SceneManager.GetActiveScene().name == "ResultScene")
        {
            return;
        }
        if (!photonView.IsMine)
        {
            Debug.Log("動かしている");
            return;
        }
        animator.SetBool("front", false);
        animator.SetBool("back", false);
        animator.SetBool("right", false);
        animator.SetBool("left", false);
        animator.SetBool("top", false);
        animator.SetBool("under", false);
        animator.SetBool("dash", false);
        //コックピットレバーをつかんでいるときの処理
        if (grabbableObj.GetComponent<OVRGrabbable>().isGrabbed == true)
        {
            //前方向移動処理
            if (handleObj.transform.position.z >= rightFrontLeverPosition)
            { 
                animator.SetBool("back", false);
                animator.SetBool("front", true);
                if (handleObj.transform.position.z >= rightFrontBoostLeverPosition && boostLimitFlag == false)
                {
                    animator.SetBool("dash", true);
                    moventObj.GetComponent<Rigidbody>().AddForceAtPosition(moventObj.transform.forward * addPower * dashRate, addPowerPoint.position, ForceMode.VelocityChange);
                    this.boostFlag = true;
                    Debug.Log("Dash!");
                }
                else
                {
                    animator.SetBool("dash", false);
                    moventObj.GetComponent<Rigidbody>().AddForceAtPosition(moventObj.transform.forward * addPower, addPowerPoint.position, ForceMode.Acceleration);
                }
            }
            else if(handleObj.transform.position.z <= rightBackLeverPosition)//後ろ方向移動処理
            {
                animator.SetBool("dash", false);
                animator.SetBool("front", false);
                animator.SetBool("back", true);
                if (handleObj.transform.position.z <= rightBackBoostLeverPosition && boostLimitFlag == false)
                {
                    moventObj.GetComponent<Rigidbody>().AddForceAtPosition((-1) * (moventObj.transform.forward * addPower * dashRate), addPowerPoint.position, ForceMode.VelocityChange);
                    this.boostFlag = true;
                }
                else
                {
                    moventObj.GetComponent<Rigidbody>().AddForceAtPosition((-1) * (moventObj.transform.forward * addPower), addPowerPoint.position, ForceMode.Acceleration);
                }
            }

            //機体縦軸回転処理
            if (OVRInput.Get(OVRInput.RawButton.RThumbstickUp))
            {
                moventObj.GetComponent<Rigidbody>().AddTorque((moventObj.transform.up * 2) * (-1), ForceMode.Acceleration);
            }
            else if (OVRInput.Get(OVRInput.RawButton.RThumbstickDown))
            {
                moventObj.GetComponent<Rigidbody>().AddTorque(moventObj.transform.up * 2, ForceMode.Acceleration);
            }

            //カメラ横軸回転処理(縦軸の回転は機体丸ごと、横軸の回転はカメラのみ)
            //クォータニオンでの角度取得は負の値では取得されないので扱えるよう180～(-180)で正規化
            float rotX = camObj.transform.localEulerAngles.x;
            if (rotX >= 180)
            {
                rotX = 360 - rotX;
                rotX *= -1;
            }
            if (OVRInput.Get(OVRInput.RawButton.RThumbstickLeft))
            {
                if (rotX < 30)
                {
                    Quaternion camRotateD = Quaternion.AngleAxis(2, Vector3.right);
                    Quaternion camRotate = camObj.transform.rotation;
                    camObj.transform.rotation = camRotate * camRotateD;
                }
            }
            else if (OVRInput.Get(OVRInput.RawButton.RThumbstickRight))
            {
                if (rotX > -30)
                {
                    Quaternion camRotateU = Quaternion.AngleAxis(-2, Vector3.right);
                    Quaternion camRotate = camObj.transform.rotation;
                    camObj.transform.rotation = camRotate * camRotateU;
                }
            }
            if (this.boostFlag == true)
            {//ブーストを使用した際のブーストゲージ消費処理
                float bstCapacity = moventObj.GetComponent<Unit>().GetCurrentBoostCapacity();
                bstCapacity -= (moventObj.GetComponent<Unit>().GetBoostConsunmption() / 2) * Time.deltaTime;
                moventObj.GetComponent<Unit>().SetCurrentBoostCapacity(bstCapacity);
                this.boostFlag = false;
            }
        }
        else
        {//触れていないときは持てるオブジェクトの位置を固定に
            Vector3 currentPos = leverObject.transform.position;
            currentPos += (currentPos / currentPos.magnitude) * (firstLeverPosition.magnitude - currentPos.magnitude) * leverSpeed * Time.deltaTime;
            leverObject.transform.position = currentPos;
            grabbableObj.transform.position = handleObj.transform.position;
            grabbableObj.transform.rotation = handleObj.transform.rotation;
        }
        IncreseBoostCapaity();
    }
    /*ブーストゲージ回復処理*/
    public void IncreseBoostCapaity()
    {

        if (moventObj.GetComponent<Unit>().GetCurrentBoostCapacity() <= 0)
        {
            moventObj.GetComponent<Unit>().SetCurrentBoostCapacity(0);
            this.boostLimitFlag = true;
        }

        if (this.boostLimitFlag == true)
        {
            float bstCapacity = moventObj.GetComponent<Unit>().GetCurrentBoostCapacity();
            bstCapacity += (moventObj.GetComponent<Unit>().GetBoostOverHeatIncrease() / 2) * Time.deltaTime;
            moventObj.GetComponent<Unit>().SetCurrentBoostCapacity(bstCapacity);

            if (moventObj.GetComponent<Unit>().GetCurrentBoostCapacity() >= moventObj.GetComponent<Unit>().GetMaxBoostCapacity())
            {
                moventObj.GetComponent<Unit>().SetCurrentBoostCapacity(moventObj.GetComponent<Unit>().GetMaxBoostCapacity());
                this.boostLimitFlag = false;
            }
        }
        else
        {
            float bstCapacity = moventObj.GetComponent<Unit>().GetCurrentBoostCapacity();
            bstCapacity += (moventObj.GetComponent<Unit>().GetBoostNormalIncrease() / 2) * Time.deltaTime;
            moventObj.GetComponent<Unit>().SetCurrentBoostCapacity(bstCapacity);

            if (moventObj.GetComponent<Unit>().GetCurrentBoostCapacity() >= moventObj.GetComponent<Unit>().GetMaxBoostCapacity())
            {
                moventObj.GetComponent<Unit>().SetCurrentBoostCapacity(moventObj.GetComponent<Unit>().GetMaxBoostCapacity());
            }
        }
    }
}
