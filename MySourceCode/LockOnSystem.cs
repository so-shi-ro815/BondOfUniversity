using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


/**
 * 敵をロックオンするための仕組み
 */
public class LockOnSystem : MonoBehaviourPunCallbacks
{
    
	int i = 0;//for文用の添え字
	public GameObject rockonmarker;//ロックオンマーカーを管理するスクリプトがアタッチされているオブジェクト
	public  GameObject player; //自分(プレイヤー)
	public  GameObject Weapon;//武器(ミサイルポッド)
	public  int MAX_LOCK_ON_TIME = 3600;//最大ロックオン時間
	public List<Enemyset> Enemanagement = new List<Enemyset>();//ロックオン中の敵を入れるためのリスト
	public List<GameObject> Enemlist = new List<GameObject>();//ロックオンできる敵を入れるためのリスト
	public List<Enemyset> NonLockEnemy;//ロックオンされていない敵を入れるためのリスト
	public Camera centerCamera;//映っているかを判定するカメラ
	Rect rect = new Rect(0, 0, 1, 1);//画面内か判定するようのRect
	public float lockOnDistX = 0.1f;//水平方向のロックオン可能範囲
	public float lockOnDistY = 0.1f;//垂直方向のロックオン可能範囲
	private const int MAX_LOCK_ON_ENEMY = 3;//最大ロックオン数

	/*ロックオンされているかなどの敵の情報を入れるための構造体*/
	public struct Enemyset
	{
		public GameObject marker;
	   public GameObject Enem;
		public int ElapsedTime;
		public bool isLockOn;
	}
	void Start()
	{
		/*アウェイクで敵味方のタグ付けを行うとよいかも(startメソッドよりもawakeメソッドの方が先に呼ばれるため)早い順に並べると awake > onEnablue > start*/

		SerchEnemy();
		Invoke(nameof(TagSet), 1.5f);
	}
	public void TagSet()
    {
		if (photonView.IsMine)
		{
			//ここからこのメソッドの終わりまでは自分に当てられたチームのタグからロックオンできる敵を探してロックオンされていない敵としてリストに追加する処理
			string enemyTagName = this.transform.root.tag;
			if (this.transform.root.tag == "Blue")
			{
				enemyTagName = "Red";
			}
			else if (this.transform.root.tag == "Red")
			{
				enemyTagName = "Blue";
			}
			Enemlist.AddRange(GameObject.FindGameObjectsWithTag(enemyTagName));
			NonLockEnemy = new List<Enemyset>();
			for (int c = 0; c < Enemlist.Count; c++)
			{
				Enemyset enemyset = new Enemyset();
				enemyset.Enem = Enemlist[c];
				NonLockEnemy.Add(enemyset);
			}
		}
	}
	
	void FixedUpdate()
	{
		//まず初めにロックオン取得処理から
		if (NonLockEnemy.Count > 0)
		{
			lockOnProc();
		}
		//ロックオン継続処理
		if (Enemanagement.Count > 0)
		{
			for (i = 0;i < Enemanagement.Count;i++)
            {
				if (Enemanagement[i].Enem != null)
				{
					if (Enemanagement[i].Enem.GetComponent<Unit>().GetCurrentArmorPoint() > 0f)//そのロックオン中の敵のAPが0より大きいかのチェック
					{
						//敵と自分との距離チェック
						float distance = Vector3.Distance(player.transform.position, Enemanagement[i].Enem.transform.position);
						if (distance <= 100f)
						{
							//敵と自分との間に障害物がないかチェック
							if (Physics.Linecast(player.transform.position, Enemanagement[i].Enem.transform.position, LayerMask.GetMask("Field")) == false)
                            {
								//敵が画面内に映っているかチェック
								Vector3 viewportPos = centerCamera.WorldToViewportPoint(Enemanagement[i].Enem.transform.position);
								if (rect.Contains(viewportPos) == true)
								{
									Vector2 viewPoint = centerCamera.WorldToViewportPoint(Enemanagement[i].Enem.transform.position);//敵のカメラ内座標取得
									viewPoint.x = viewPoint.x - 0.5f;//中心からの水平方向の距離算出
									viewPoint.y = viewPoint.y - 0.5f;//中心からの垂直方向の距離算出
									Vector3 tolocal = player.transform.InverseTransformPoint(Enemanagement[i].Enem.transform.position);//自分の機体から見て敵の座標を取得

									//敵が自分の位置より前にいて、ミサイルのロックオン可能範囲に入っているかチェック
									if (Mathf.Abs(viewPoint.x) <= lockOnDistX && Mathf.Abs(viewPoint.y)<=lockOnDistY && tolocal.z > 0)
									{
										//ロックオン時間を満たしているかチェック
										if (Enemanagement[i].ElapsedTime < MAX_LOCK_ON_TIME)//最大ロックオン時間を過ぎていない場合経過時間加算
										{
											Enemyset enemyset = Enemanagement[i];
											enemyset.ElapsedTime += 1;
											Enemanagement[i] = enemyset;
										}
										if (Weapon.GetComponent<WeaponMisile>().getLockOnTime() <= Enemanagement[i].ElapsedTime)//ロックオンしている時間が設定されている値を満たしていればisLockをtrueに
										{
											Enemyset enemyset = Enemanagement[i];
											enemyset.isLockOn = true;
											Enemanagement[i] = enemyset;
										}
										else
										{
											Enemyset enemyset = Enemanagement[i];
											enemyset.isLockOn = false;
											Enemanagement[i] = enemyset;
										}
									}
									else
									{
										//各条件においてロックオン時間の条件以外で満たさないものがあればロックオン中の敵のリストから外す
										i = RemoveList(i);
									}
								}
								else
								{
									i = RemoveList(i);
								}
							}
                            else
                            {
                                i = RemoveList(i);
                            }
                        }
						else
						{
							i = RemoveList(i);
						}
					}
                    else
                    {
						i = RemoveList(i);
					}
				}
				else
				{
					i = RemoveList(i);
				}
			}
		}
	}

	/*ロックオン取得処理*/
	private void lockOnProc()
	{
		int j;
		for (j = 0;j<NonLockEnemy.Count;j++)
        {
			if (NonLockEnemy[j].Enem != null)
			{
				Debug.Log("残り機体HP"+NonLockEnemy[j].Enem.GetComponent<Unit>().GetCurrentArmorPoint());
				if (NonLockEnemy[j].Enem.GetComponent<Unit>().GetCurrentArmorPoint() > 0)
				{
					float distance = Vector3.Distance(player.transform.position, NonLockEnemy[j].Enem.transform.position);
					if (distance <= 100f)
					{
						if (Physics.Linecast(player.transform.position, NonLockEnemy[j].Enem.transform.position, LayerMask.GetMask("Field")) == false)
						{
							Vector3 viewportPos = centerCamera.WorldToViewportPoint(NonLockEnemy[j].Enem.transform.position);
							if (rect.Contains(viewportPos) == true)
							{
								Debug.Log("画面内");
								Vector2 viewPoint = centerCamera.WorldToViewportPoint(NonLockEnemy[j].Enem.transform.position);
								viewPoint.x = viewPoint.x - 0.5f;
								viewPoint.y = viewPoint.y - 0.5f;
								Vector3 tolocal = player.transform.InverseTransformPoint(NonLockEnemy[j].Enem.transform.position);
								if (Mathf.Abs(viewPoint.x) <= lockOnDistX && Mathf.Abs(viewPoint.y) <= lockOnDistY && tolocal.z >0)
								{
									//ロックオン時間の条件がないだけでロックオン継続中の処理と変わらない
									if (Enemanagement.Count < MAX_LOCK_ON_ENEMY)
									{
										Debug.Log("ロックオンします");
										//敵の構造体としての情報を設定する
										Enemyset enemyset = NonLockEnemy[j];
										enemyset.marker = null;//マーカーの割り振りを初期化
										enemyset.ElapsedTime = 0;//ロックオン経過時間を初期化
										enemyset.isLockOn = false;//ロックオン完了しているかのフラグ初期化
										Enemanagement.Add(enemyset);//ロックオン中の敵としてリストに追加
										NonLockEnemy.Remove(NonLockEnemy[j]);//ロックオンされていない敵のリストから外す
									}
								}
							}
						}
					}
				}
			}
            else
            {
				Debug.Log("消しました");
				NonLockEnemy.Remove(NonLockEnemy[j]);
            }
		}
		return;
	}	

	/*ロックオン除外処理*/
	public int RemoveList(int num)
    {
		Debug.Log("除外します");
		//構造体のパラメータを0やnullなどで初期化
		Enemanagement[num].marker.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
		RockMarker r = rockonmarker.GetComponent<RockMarker>();
		r.marker.Add(Enemanagement[num].marker);
		Enemyset enemyset = Enemanagement[num];
		enemyset.marker = null;
		enemyset.ElapsedTime = 0;
		Enemanagement[num] = enemyset;
		NonLockEnemy.Add(Enemanagement[num]);
		Enemanagement.Remove(Enemanagement[num]);
		Debug.Log("除外回数"+NonLockEnemy.Count);
		Weapon.GetComponent<WeaponMisile>().targetset.Enem = null;
		Weapon.GetComponent<WeaponMisile>().SetTaget(null);
		num--;
		return num;
	}
	public void SerchEnemy()
    {
		if (photonView.IsMine)
		{
			//ここからこのメソッドの終わりまでは自分に当てられたチームのタグからロックオンできる敵を探してロックオンされていない敵としてリストに追加する処理
			string enemyTagName = this.transform.parent.tag;
			if (this.transform.root.tag == "Blue")
			{
				enemyTagName = "Red";
			}
			else if (this.transform.root.tag == "Red")
			{
				enemyTagName = "Blue";
			}
			Enemlist.AddRange(GameObject.FindGameObjectsWithTag(enemyTagName));
			NonLockEnemy = new List<Enemyset>();
			for (int c = 0; c < Enemlist.Count; c++)
			{
				Enemyset enemyset = new Enemyset();
				enemyset.Enem = Enemlist[c];
				NonLockEnemy.Add(enemyset);
			}
		}
	}
}
