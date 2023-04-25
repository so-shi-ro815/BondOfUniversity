using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Misile : MonoBehaviour
{
    private Vector3 currentPos;//現在位置
    private Vector3 currentVelocity;//現在速度
    private float time;//経過時間
    private float maxCentripetalAccel;//最大操舵加速度(最大向心力)
    private float propulsion;//推進加速度
    [SerializeField]
    private float damping;//空気抵抗
    [SerializeField]
    private GameObject targetObj;//目標オブジェクト
    [SerializeField]
    private float speed;//ミサイルの速さ
    [SerializeField]
    private float curvatureRadius;//カーブ可能半径
    [SerializeField]
    private Transform firstPosition;//初期位置
    [SerializeField]
    private Vector3 firstVerlocity;//初期速度
    [SerializeField]
    private Vector3 misileObjForward;//向き変更用のミサイルオブジェクトの座標
    [SerializeField]
    private GameObject misileObj;//向き変更用のミサイルオブジェクト

    // Start is called before the first frame update
    void Start()
    {
        /*これはテスト用基本的にはオブジェクト生成の時にEmitメソッドを呼び出す*/
        //エディタで入力した値を反映
        //Emit(firstPosition.position, firstVerlocity, speed, curvatureRadius, damping); 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //現在地取得
        currentPos = this.transform.position;
        //目標ベクトル算出
        Vector3 toTargetVector = targetObj.transform.position - currentPos;
        //現在速度正規化
        Vector3 normalizedVelocity = currentVelocity.normalized;
        //目標ベクトルと正規化した現在速度ベクトルのない席を算出
        float dot = Vector3.Dot(toTargetVector, normalizedVelocity);
        //操舵加速度ベクトル(目標地点に向かせるための速度)算出
        Vector3 centripetalAccel = toTargetVector - (normalizedVelocity * dot);
        //操舵加速度ベクトルの速さ取得
        float centripetalAccelMagnitude = centripetalAccel.magnitude;
        //速さが1より大きい場合は速さを1にする
        if(centripetalAccelMagnitude > 1f)
        {
            centripetalAccel /= centripetalAccelMagnitude;
        }

        //加える力を算出
        Vector3 force = centripetalAccel * maxCentripetalAccel;//制限付きの操舵加速度
        force += normalizedVelocity * propulsion;//前に進む推進力
        force -= currentVelocity * damping;//空気抵抗
        currentVelocity += force * Time.deltaTime;//速度積分
        this.transform.position += currentVelocity * Time.deltaTime;//位置に反映

        //ミサイルの向きを調整
        Quaternion lookAtRotation = Quaternion.LookRotation(currentVelocity);//算出した速度の方向に向く成分
        Quaternion offsetRotation = Quaternion.FromToRotation(misileObjForward, Vector3.forward);//正面がZ以外の場合正面を変える成分
        misileObj.transform.rotation = lookAtRotation * offsetRotation;//算出した成分をかけて現在の向きに反映
        time += Time.deltaTime;//時間経過を計測
    }

    /**
     * 引数をMisileクラスの各パラメータなどに設定するためのメソッド
     */
    public void Emit(Vector3 position,Vector3 velocity,float speedValue,float curvatureRadiusValue, float dampingValue, GameObject Target)
    {
        this.targetObj = Target;
        this.currentPos = position;
        this.currentVelocity = velocity;
        //速さv、半径rで円を描く時、その向心力はv^2/rを計算する。
        this.maxCentripetalAccel = speedValue * speedValue / curvatureRadiusValue;
        this.damping = dampingValue;
        this.propulsion = speedValue * dampingValue;//速さ×減衰率(終端速度がspeedValueとなるように推進加速度を算出)
        time = 0f;
    }
}
