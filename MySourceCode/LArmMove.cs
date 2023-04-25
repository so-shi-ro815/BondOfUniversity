using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class LArmMove : MonoBehaviourPunCallbacks
{
    //レバー移動スピード
    public float leverSpeed = 3f;
    //レバーオブジェクト初期位置
    private Vector3 firstLeverPosition;
    //レバーオブジェクト
    public GameObject leverObject;
    //レバー固定化オブジェクト
    public GameObject handleObj;
    //つかみ可能オブジェクト
    public GameObject grabbableObj;
    //機体オブジェクト
    public GameObject moventObj;
    //機体アニメーション
    public Animator animator;
    //機体の基礎速さ
    public float addPower = 25f;
    //レバー移動による速度の反映点
    public Transform addPowerPoint;
    //水平方向移動倍率
    public float sidePowerRate = 0.5f;
    //垂直方向移動倍率
    public float upDownPowerRate = 0.5f;
    //ブースト移動倍率
    public float dashRate = 1.5f;
    //ブースト使用フラグ
    private bool boostFlag = false;
    //オーバーヒートフラグ
    private bool boostLimitFlag = false;
    public float leftFrontLeverPosition = 0.3f;
    public float leftFrontBoostLeverPosition = 0.5f;
    public float leftBackLeverPosition = 0.2f;
    public float leftBackBoostLeverPosition = 0.1f;
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
        else if (SceneManager.GetActiveScene().name == "ResultScene")
        {
            return;
        }
        if (PhotonNetwork.IsConnected)
        {
            leverObject = GameObject.Find("CockPit").transform.Find("main").transform.Find("leftarm").gameObject;
            handleObj = GameObject.Find("CockPit").transform.Find("main").transform.Find("leftarm").transform.Find("Handle").gameObject;
            grabbableObj = GameObject.Find("CockPit").transform.Find("main").transform.Find("leftarm").transform.Find("Handle").transform.Find("GrabbedPoint").gameObject;
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
        if (SceneManager.GetActiveScene().name == "TitleScene")
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
            return;
        }
        animator.SetBool("right", false);
        animator.SetBool("left", false);
        animator.SetBool("top", false);
        animator.SetBool("under", false);
        animator.SetBool("dash", false);
        //コックピットレバーをつかんでいるときの処理
        if (grabbableObj.GetComponent<OVRGrabbable>().isGrabbed == true)
        {
            //前方向移動処理
            if (handleObj.transform.position.z >= leftFrontLeverPosition)
            {
                animator.SetBool("back", false);
                animator.SetBool("front", true);
                if (handleObj.transform.position.z >= leftFrontBoostLeverPosition && this.boostLimitFlag == false)
                {
                    animator.SetBool("dash", true);
                    moventObj.GetComponent<Rigidbody>().AddForceAtPosition(moventObj.transform.forward * addPower * dashRate, addPowerPoint.position, ForceMode.VelocityChange);
                    this.boostFlag = true;
                }
                else
                {
                    animator.SetBool("dash", false);
                    moventObj.GetComponent<Rigidbody>().AddForceAtPosition(moventObj.transform.forward * addPower, addPowerPoint.position, ForceMode.Acceleration);
                }
            }
            else if (this.handleObj.transform.position.z <= leftBackLeverPosition)//後ろ方向移動処理
            {
                animator.SetBool("dash", false);
                animator.SetBool("front", false);
                animator.SetBool("back", true);
                if (handleObj.transform.position.z <= leftBackBoostLeverPosition && boostLimitFlag == false)
                {
                    moventObj.GetComponent<Rigidbody>().AddForceAtPosition((-1) * (moventObj.transform.forward * addPower * dashRate), addPowerPoint.position, ForceMode.VelocityChange);
                    this.boostFlag = true;
                }
                else
                {
                    moventObj.GetComponent<Rigidbody>().AddForceAtPosition((-1) * (moventObj.transform.forward * addPower), addPowerPoint.position, ForceMode.Acceleration);
                }
            }

            //水平方向移動処理
            if (OVRInput.Get(OVRInput.RawButton.LThumbstickUp))
            {
                animator.SetBool("left", false);
                animator.SetBool("right", true);
                if (OVRInput.Get(OVRInput.RawButton.LThumbstick) && boostLimitFlag == false)
                {
                    moventObj.GetComponent<Rigidbody>().AddForce(moventObj.transform.right * addPower * sidePowerRate * dashRate, ForceMode.VelocityChange);
                    this.boostFlag = true;
                }
                else
                {
                    moventObj.GetComponent<Rigidbody>().AddForce(moventObj.transform.right * addPower * sidePowerRate, ForceMode.Acceleration);
                }
            }
            else if (OVRInput.Get(OVRInput.RawButton.LThumbstickDown))
            {
                animator.SetBool("right", false);
                animator.SetBool("left", true);
                if (OVRInput.Get(OVRInput.RawButton.LThumbstick) && boostLimitFlag == false)
                {
                    moventObj.GetComponent<Rigidbody>().AddForce((moventObj.transform.right * addPower * sidePowerRate * dashRate) * (-1), ForceMode.VelocityChange);
                    this.boostFlag = true;
                }
                else
                {
                    moventObj.GetComponent<Rigidbody>().AddForce((moventObj.transform.right * addPower * sidePowerRate) * (-1), ForceMode.Acceleration);
                }
            }

            //垂直方向移動処理
            if (OVRInput.Get(OVRInput.RawButton.LThumbstickLeft))
            {
                animator.SetBool("under", false);
                animator.SetBool("top", true);
                if (OVRInput.Get(OVRInput.RawButton.LThumbstick) && boostLimitFlag == false)
                {
                    moventObj.GetComponent<Rigidbody>().AddForce(moventObj.transform.up * addPower * upDownPowerRate * dashRate, ForceMode.VelocityChange);
                    this.boostFlag = true;
                }
                else
                {
                    moventObj.GetComponent<Rigidbody>().AddForce(moventObj.transform.up * addPower * upDownPowerRate, ForceMode.Acceleration);
                }
            }
            else if (OVRInput.Get(OVRInput.RawButton.LThumbstickRight))
            {
                animator.SetBool("top", false);
                animator.SetBool("under", true);
                if (OVRInput.Get(OVRInput.RawButton.LThumbstick) && boostLimitFlag == false)
                {
                    moventObj.GetComponent<Rigidbody>().AddForce((moventObj.transform.up * addPower * upDownPowerRate * dashRate) * (-1), ForceMode.VelocityChange);
                    this.boostFlag = true;
                }
                else
                {
                    moventObj.GetComponent<Rigidbody>().AddForce((moventObj.transform.up * addPower * upDownPowerRate) * (-1), ForceMode.Acceleration);
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
