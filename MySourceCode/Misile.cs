using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Misile : MonoBehaviour
{
    private Vector3 currentPos;//���݈ʒu
    private Vector3 currentVelocity;//���ݑ��x
    private float time;//�o�ߎ���
    private float maxCentripetalAccel;//�ő呀�ǉ����x(�ő���S��)
    private float propulsion;//���i�����x
    [SerializeField]
    private float damping;//��C��R
    [SerializeField]
    private GameObject targetObj;//�ڕW�I�u�W�F�N�g
    [SerializeField]
    private float speed;//�~�T�C���̑���
    [SerializeField]
    private float curvatureRadius;//�J�[�u�\���a
    [SerializeField]
    private Transform firstPosition;//�����ʒu
    [SerializeField]
    private Vector3 firstVerlocity;//�������x
    [SerializeField]
    private Vector3 misileObjForward;//�����ύX�p�̃~�T�C���I�u�W�F�N�g�̍��W
    [SerializeField]
    private GameObject misileObj;//�����ύX�p�̃~�T�C���I�u�W�F�N�g

    // Start is called before the first frame update
    void Start()
    {
        /*����̓e�X�g�p��{�I�ɂ̓I�u�W�F�N�g�����̎���Emit���\�b�h���Ăяo��*/
        //�G�f�B�^�œ��͂����l�𔽉f
        //Emit(firstPosition.position, firstVerlocity, speed, curvatureRadius, damping); 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //���ݒn�擾
        currentPos = this.transform.position;
        //�ڕW�x�N�g���Z�o
        Vector3 toTargetVector = targetObj.transform.position - currentPos;
        //���ݑ��x���K��
        Vector3 normalizedVelocity = currentVelocity.normalized;
        //�ڕW�x�N�g���Ɛ��K���������ݑ��x�x�N�g���̂Ȃ��Ȃ��Z�o
        float dot = Vector3.Dot(toTargetVector, normalizedVelocity);
        //���ǉ����x�x�N�g��(�ڕW�n�_�Ɍ������邽�߂̑��x)�Z�o
        Vector3 centripetalAccel = toTargetVector - (normalizedVelocity * dot);
        //���ǉ����x�x�N�g���̑����擾
        float centripetalAccelMagnitude = centripetalAccel.magnitude;
        //������1���傫���ꍇ�͑�����1�ɂ���
        if(centripetalAccelMagnitude > 1f)
        {
            centripetalAccel /= centripetalAccelMagnitude;
        }

        //������͂��Z�o
        Vector3 force = centripetalAccel * maxCentripetalAccel;//�����t���̑��ǉ����x
        force += normalizedVelocity * propulsion;//�O�ɐi�ސ��i��
        force -= currentVelocity * damping;//��C��R
        currentVelocity += force * Time.deltaTime;//���x�ϕ�
        this.transform.position += currentVelocity * Time.deltaTime;//�ʒu�ɔ��f

        //�~�T�C���̌����𒲐�
        Quaternion lookAtRotation = Quaternion.LookRotation(currentVelocity);//�Z�o�������x�̕����Ɍ�������
        Quaternion offsetRotation = Quaternion.FromToRotation(misileObjForward, Vector3.forward);//���ʂ�Z�ȊO�̏ꍇ���ʂ�ς��鐬��
        misileObj.transform.rotation = lookAtRotation * offsetRotation;//�Z�o���������������Č��݂̌����ɔ��f
        time += Time.deltaTime;//���Ԍo�߂��v��
    }

    /**
     * ������Misile�N���X�̊e�p�����[�^�Ȃǂɐݒ肷�邽�߂̃��\�b�h
     */
    public void Emit(Vector3 position,Vector3 velocity,float speedValue,float curvatureRadiusValue, float dampingValue, GameObject Target)
    {
        this.targetObj = Target;
        this.currentPos = position;
        this.currentVelocity = velocity;
        //����v�A���ar�ŉ~��`�����A���̌��S�͂�v^2/r���v�Z����B
        this.maxCentripetalAccel = speedValue * speedValue / curvatureRadiusValue;
        this.damping = dampingValue;
        this.propulsion = speedValue * dampingValue;//�����~������(�I�[���x��speedValue�ƂȂ�悤�ɐ��i�����x���Z�o)
        time = 0f;
    }
}
