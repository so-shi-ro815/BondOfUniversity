using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPunCallbacks
{
	//爆破時エフェクトオブジェクト
	public GameObject explosion;
	//弾の向き調整用角度
	protected Quaternion firstRotation;
	//弾のリジッドボディ
	protected Rigidbody rBody;
	//弾速
	protected float speed;
	//弾のダメージ
	protected float damage;
	//弾の有効時間
	protected int effectiveTime;
	//重力有効化までの時間
	protected const int gravityRimit = 3;
	//銃弾が動作中かを表すフラグ
	protected bool activeFlag = false;
	//武器の属性「0」が「KE」,「1」が「TE」,「2」が「HE」,「3」が無属性(爆風など衝撃)
	[SerializeField]
	protected int weaponAttribute = 0;
	//爆風処理のためのパラメータ
	[SerializeField]
	protected float maxExplodDistance = 20;
	[SerializeField]
	protected float minExplodDistance = 5;
	protected float CurrentDistance;


	//Photonの変数
	//発射者の、識別番号
	public int actor = 0;
	/*
	重力を有効化するための処理 
	 */
	IEnumerator Gravity()
	{
		yield return new WaitForSeconds(gravityRimit);
		//重力を有効化
		rBody.useGravity = true;
	}

	/*
	 弾が何かオブジェクトに当たった際の処理
	 */
    private void OnCollisionEnter(Collision collision)
    {
		//爆破エフェクトがセットされている場合のみ爆破エフェクトを発生
		if (explosion != null)
		{
			Instantiate(explosion, transform.position, transform.rotation);
		}
		Debug.Log("衝突");
		EndEffectiveTime();
	}

	public float GetDamage()
	{//ダメージ量取得
		return this.damage;
	}
	public int GetWeaponAttribute()
	{//弾の属性取得
		return this.weaponAttribute;
	}

	/*弾丸が何かに当たったときや有効時間を過ぎた時の処理ときの処理*/
	public virtual void EndEffectiveTime()
	{
		Destroy(this.gameObject);
	}

	/// <summary>
	/// このメソッドは機雷の弾が徐々に速度を下げていく処理を実装する際に使用してください
	/// </summary>
	public virtual void VelocityDown()
	{
		return;
	}
	/*
	 弾が生成された際の処理
	弾の速さ、ダメージ量、有効時間を設定
	 */
	[PunRPC]
	public virtual void Create(float dm, float sp, int ef,int ac)
	{
		this.damage = dm;
		this.speed = sp;
		this.effectiveTime = ef;
		this.actor = ac;
		BulletLaunchSetting();
	}
	public virtual void BulletLaunchSetting()
	{
		//弾の最初の角度取得
		this.firstRotation = transform.rotation;
		this.rBody = GetComponent<Rigidbody>();
		//重力設定をオフに
		this.rBody.useGravity = false;
		//飛ばす速度設定
		Vector3 movementSpeed = new Vector3(0, speed, 0);
		//飛ばす向き決定
		movementSpeed = firstRotation * movementSpeed;
		//弾のオブジェクトを発射
		this.rBody.AddForce(movementSpeed, ForceMode.Impulse);
		this.activeFlag = true;
		//重力有効化までの時間計測開始(非同期処理開始)
		StartCoroutine("Gravity");
	}

	public void StartCreate(float dm,float sp, int ef,int ac)
    {
		photonView.RPC(nameof(Create), RpcTarget.All, dm, sp, ef,ac);
    }
	/*
	 有効時間の計測
	 */
	void FixedUpdate()
	{
        if (this.activeFlag == false)
		{
			return;
		}
		VelocityDown();
		this.effectiveTime--;
		if (this.effectiveTime <= 0)
		{
			Debug.Log("時間消滅");
			EndEffectiveTime();
		}

	}

}
