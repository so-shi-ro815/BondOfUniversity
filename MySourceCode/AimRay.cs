using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class AimRay : MonoBehaviour
{
    //����I�u�W�F�N�g
    public GameObject weaponObj;
    //�R�b�N�s�b�g���ʃX�N���[���̃J����
    public Camera centerCam;
    //�R�b�N�s�b�g���o�[�̃I�u�W�F�N�g
    public GameObject grabbableObj;
    //���̃I�u�W�F�N�g�̐��ʂ̌���
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
        //�R�b�N�s�b�g���o�[�������Ă���Ƃ��̏���
        if (grabbableObj.GetComponent<OVRGrabbable>().isGrabbed == true)
        {
            //���C�𐶐�
            Ray ray = centerCam.ViewportPointToRay(new Vector2(0.5f, 0.5f));
            //���C���q�b�g�����I�u�W�F�N�g�ɕ��킪��������
            if (Physics.Raycast(ray, out RaycastHit hit))//hit��raycast�ł����������̏����擾
            {
                //���C���q�b�g�����I�u�W�F�N�g�ւ̌����̌v�Z
                Vector3 dir = hit.point - weaponObj.transform.position;
                //�q�b�g�����I�u�W�F�N�g�̕����Ɍ�����]�̌v�Z
                Quaternion lookAtRotation = Quaternion.LookRotation(dir, Vector3.up);
                //������]�̕␳(������ς���I�u�W�F�N�g��Z�����������ʂ̕����łȂ��ꍇ�K�v)
                Quaternion offsetRotation = Quaternion.FromToRotation(weaponObjForward, Vector3.forward);
                //�␳���݂Ō�����ς���I�u�W�F�N�g�ɉ�]�𔽉f
                weaponObj.transform.rotation = lookAtRotation * offsetRotation;
            }
        }
    }
}
