using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class RArmMove : MonoBehaviourPunCallbacks
{
    //���o�[�ړ��X�s�[�h
    public float leverSpeed = 3f;
    //���o�[�I�u�W�F�N�g�����ʒu
    private Vector3 firstLeverPosition;
    //���o�[�I�u�W�F�N�g
    public GameObject leverObject;
    //���o�[�Œ艻�I�u�W�F�N�g
    public GameObject handleObj;
    //�@�̃I�u�W�F�N�g
    public GameObject moventObj;
    //���݉\�I�u�W�F�N�g
    public GameObject grabbableObj;
    //�@�̃A�j���[�V����
    public Animator animator;
    //�@�̂̊�b����
    public float addPower = 25f;
    //���o�[�ړ��ɂ�鑬�x�̔��f�_
    public Transform addPowerPoint;
    //�@�̂̃J�����I�u�W�F�N�g
    public GameObject camObj;
    //�u�[�X�g�ړ��{��
    public float dashRate = 1.5f;
    //�u�[�X�g�g�p�t���O
    private bool boostFlag = false;
    //�I�[�o�[�q�[�g�t���O
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
            //�����ȊO�̋@�̎��̏���(AudioListerner��1�݂̂Ȃ̂�)
            Cameras.transform.Find("CameraC").GetComponent<AudioListener>().gameObject.SetActive(false);
            Cameras.transform.Find("CameraC").GetComponent<Camera>().gameObject.SetActive(false);
            Cameras.transform.Find("CameraL").GetComponent<Camera>().gameObject.SetActive(false);
            Cameras.transform.Find("CameraR").GetComponent<Camera>().gameObject.SetActive(false);
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
            Debug.Log("�������Ă���");
            return;
        }
        animator.SetBool("front", false);
        animator.SetBool("back", false);
        animator.SetBool("right", false);
        animator.SetBool("left", false);
        animator.SetBool("top", false);
        animator.SetBool("under", false);
        animator.SetBool("dash", false);
        //�R�b�N�s�b�g���o�[������ł���Ƃ��̏���
        if (grabbableObj.GetComponent<OVRGrabbable>().isGrabbed == true)
        {
            //�O�����ړ�����
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
            else if(handleObj.transform.position.z <= rightBackLeverPosition)//�������ړ�����
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

            //�@�̏c����]����
            if (OVRInput.Get(OVRInput.RawButton.RThumbstickUp))
            {
                moventObj.GetComponent<Rigidbody>().AddTorque((moventObj.transform.up * 2) * (-1), ForceMode.Acceleration);
            }
            else if (OVRInput.Get(OVRInput.RawButton.RThumbstickDown))
            {
                moventObj.GetComponent<Rigidbody>().AddTorque(moventObj.transform.up * 2, ForceMode.Acceleration);
            }

            //�J����������]����(�c���̉�]�͋@�̊ۂ��ƁA�����̉�]�̓J�����̂�)
            //�N�H�[�^�j�I���ł̊p�x�擾�͕��̒l�ł͎擾����Ȃ��̂ň�����悤180�`(-180)�Ő��K��
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
