using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class BulletSpawn :MonoBehaviourPunCallbacks
{
	//コックピットレバーのオブジェクト
	public GameObject grabbableObjR;
	//マズルフラッシュ用のオブジェクト
	public GameObject muzzleFlash;
	//攻撃時のSE
	public AudioClip shotSE;
	//撃つ弾丸のオブジェクト
	public GameObject bullet;
	//発射レート
	public int rateOfFire = 10;
	//発射レート計測用変数
	private int delayTime = 0;
	//最大弾数
	public int maxAmmo = 40;
	//現在の弾数
	[SerializeField]
	private int ammo;
	//反映させる弾丸のスピード
	public float speed = 100;
	//反映させる弾丸のダメージ
	public float damage = 100;
	//反映させる弾丸の有効時間
	public int effectiveTime = 30;
	//武器固有番号
	public int weaponNum = 0;
	//現在の武器番号
	public static int currentWeaponNum = 0;
	//弾丸の生成位置オフセット
	public Vector3 bulletOffset = new Vector3(0, 2, 0);
	//生成弾丸向き変更用回転軸
	public Vector3 rotateAxis = new Vector3(0, 0, 0);
	//生成弾丸向き変更用回転角度
	public float angle = 90;
	private void Start()
    {
		if (SceneManager.GetActiveScene().name == "TitleScene")
		{
			return;
		}
		if (photonView.IsMine)
		{
			Debug.Log("持った!");
			grabbableObjR = GameObject.Find("CockPit").transform.Find("main").transform.Find("rightarm").transform.Find("Handle").transform.Find("GrabbedPoint").gameObject;
		}
		ammo = maxAmmo;
    }
    void FixedUpdate()
	{
		if (SceneManager.GetActiveScene().name == "TitleScene")
		{
			return;
		}
		if (CheckWeponNum() != true)
        {
			return;
        }
		if (!photonView.IsMine)
		{
			return;
		}
        if (ammo <= 0)
		{
			ammo = maxAmmo;
        }
		/*コントローラーのトリガーボタンが押されているとき*/
		if (grabbableObjR.GetComponent<OVRGrabbable>().isGrabbed == true)
		{
			//コントローラーのトリガーボタンを推した場合の処理
			bool fire = OVRInput.Get(OVRInput.RawButton.RIndexTrigger);
			if (fire == true)
			{
				if (ammo > 0)
				{
					if (delayTime <= 0)
					{
						delayTime = rateOfFire;
						//弾丸を生成させるための位置を取得
						Vector3 placePosition = this.transform.position;
						//弾丸の生成位置のオフセットを設定
						Vector3 offsetGun = bulletOffset;

						//生成される弾を正しい向きに回転させる処理
						Quaternion q1 = this.transform.rotation;
						Quaternion q2 = Quaternion.AngleAxis(angle, rotateAxis);
						Quaternion q = q1 * q2;

						//オフセット込みで弾の生成される位置を確定
						placePosition = q1 * offsetGun + placePosition;

						//角度と位置を指定して弾を生成
						if (photonView.IsMine)
						{
							Debug.Log("弾生成");
							GameObject tmpBullet = PhotonNetwork.Instantiate(bullet.gameObject.name, placePosition, q);// as GameObject;

							//生成されたオブジェクトのアドレスを別のインタンスに渡す
							BulletSetting(ref tmpBullet);
						}
						else if (!photonView.IsMine)
						{
							GameObject tmpBullet = Instantiate(bullet.gameObject, placePosition, q);// as GameObject;

							//ユーザー識別子
							//PhotonNetwork.LocalPlayer.ActorNumber
							//以下のコメントの分を同期化
							//生成されたオブジェクトのアドレスを別のインタンスに渡す
							BulletSetting(ref tmpBullet);
						}


						//セットしていればマズルフラッシュを発生
						if (muzzleFlash != null)
						{
							Instantiate(muzzleFlash, placePosition, transform.rotation);
						}
						//セットされていればSE再生
						if (shotSE != null)
						{
							this.GetComponent<AudioSource>().PlayOneShot(shotSE);
						}
						ammo -= 1;
					}
				}
			}
			if (0 < delayTime)
			{
				delayTime--;
			}
		}
	}

	/*選択中の武器と比較するメソッド*/
	public virtual bool CheckWeponNum()
	{
		if (weaponNum == currentWeaponNum)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	public void SetWeaponNum()
    {
		currentWeaponNum = weaponNum;
    }
	public virtual void BulletSetting(ref GameObject bulletObject)
	{   //弾の各パラメータを設定
		bulletObject.tag = this.transform.root.tag;
		bulletObject.GetComponent<Bullet>().StartCreate(damage, speed, effectiveTime,PhotonNetwork.LocalPlayer.ActorNumber);
	}

	public int GetAmmo()
    {
		return this.ammo;
    }

	public void SetAmmo(int am)
    {
		this.ammo = am;
    }
	public int GetMaxAmmo()
    {
		return this.maxAmmo;
    }
}
