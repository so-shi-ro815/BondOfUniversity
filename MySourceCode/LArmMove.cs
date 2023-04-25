using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class LArmMove : MonoBehaviourPunCallbacks
{
    //���o�[�ړ��X�s�[�h
    public float leverSpeed = 3f;
    //���o�[�I�u�W�F�N�g�����ʒu
    private Vector3 firstLeverPosition;
    //���o�[�I�u�W�F�N�g
    public GameObject leverObject;
    //���o�[�Œ艻�I�u�W�F�N�g
    public GameObject handleObj;
    //���݉\�I�u�W�F�N�g
    public GameObject grabbableObj;
    //�@�̃I�u�W�F�N�g
    public GameObject moventObj;
    //�@�̃A�j���[�V����
    public Animator animator;
    //�@�̂̊�b����
    public float addPower = 25f;
    //���o�[�ړ��ɂ�鑬�x�̔��f�_
    public Transform addPowerPoint;
    //���������ړ��{��
    public float sidePowerRate = 0.5f;
    //���������ړ��{��
    public float upDownPowerRate = 0.5f;
    //�u�[�X�g�ړ��{��
    public float dashRate = 1.5f;
    //�u�[�X�g�g�p�t���O
    private bool boostFlag = false;
    //�I�[�o�[�q�[�g�t���O
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
        //�S�ẴA�j���[�V�������I�t��
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
        //�R�b�N�s�b�g���o�[������ł���Ƃ��̏���
        if (grabbableObj.GetComponent<OVRGrabbable>().isGrabbed == true)
        {
            //�O�����ړ�����
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
            else if (this.handleObj.transform.position.z <= leftBackLeverPosition)//�������ړ�����
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

            //���������ړ�����
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

            //���������ړ�����
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
            {//�u�[�X�g���g�p�����ۂ̃u�[�X�g�Q�[�W�����
                float bstCapacity = moventObj.GetComponent<Unit>().GetCurrentBoostCapacity();
                bstCapacity -= (moventObj.GetComponent<Unit>().GetBoostConsunmption() / 2) * Time.deltaTime;
                moventObj.GetComponent<Unit>().SetCurrentBoostCapacity(bstCapacity);
                this.boostFlag = false;
            }
        }
        else
        {//�G��Ă��Ȃ��Ƃ��͎��Ă�I�u�W�F�N�g�̈ʒu���Œ��
            Vector3 currentPos = leverObject.transform.position;
            currentPos += (currentPos / currentPos.magnitude) * (firstLeverPosition.magnitude - currentPos.magnitude) * leverSpeed * Time.deltaTime;
            leverObject.transform.position = currentPos;
            grabbableObj.transform.position = handleObj.transform.position;
            grabbableObj.transform.rotation = handleObj.transform.rotation;
        }
        IncreseBoostCapaity();
    }

    /*�u�[�X�g�Q�[�W�񕜏���*/
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
