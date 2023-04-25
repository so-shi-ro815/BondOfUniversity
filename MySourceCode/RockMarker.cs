using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * マーカーオブジェクトを管理するためのコードです。
 */
public class RockMarker : MonoBehaviour
{
	public int i=0;//for文用の添え字
	public List<GameObject> marker = new List<GameObject>();//マーカーオブジェクトをまとめたもの
	public GameObject lockOnSystem;//ロックオンのシステムを持ったオブジェクト
	public Camera CCamera;//機体のセンターカメラ
	public GameObject markerCanvas;//マーカーオブジェクトのキャンバス
	// Use this for initialization
	void Start()
	{
		//マーカーオブジェクトを透明に初期化する
		markerCanvas = GameObject.Find("Canvas");
		marker.AddRange(GameObject.FindGameObjectsWithTag("maker"));
		foreach(GameObject makerob in marker)
        {
			makerob.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
		}
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		//ロックオンシステム取得
		LockOnSystem l = lockOnSystem.GetComponent<LockOnSystem>();
		//ここからは現在ロックオン中の敵にマーカーを割り振る処理
		if (l.Enemanagement.Count > 0)
		{
			for(i=0;i<l.Enemanagement.Count;i++)
			{
				if (l.Enemanagement[i].Enem != null)
				{
					if (l.Enemanagement[i].marker == null)
					{
						GameObject mark = marker[marker.Count - 1];
						marker.RemoveAt(marker.Count - 1);
						LockOnSystem.Enemyset enemyset = l.Enemanagement[i];
						enemyset.marker = mark;
						l.Enemanagement[i] = enemyset;
					}
					//ロックオンマーカー座標を取得
					Vector2 position = CCamera.WorldToViewportPoint(l.Enemanagement[i].Enem.transform.position);
					Debug.Log(position.x);
					Debug.Log(position.y);
					Vector3 mPos = l.Enemanagement[i].marker.transform.position;
					mPos.x = position.x-0.5f+markerCanvas.transform.position.x;
					mPos.y = position.y-0.5f+markerCanvas.transform.position.y;
					Debug.Log(mPos);
					l.Enemanagement[i].marker.transform.position = mPos;
					//各敵のロックオンしている時間に応じて色を黄色、緑に変更する処理
					if (0 < l.Enemanagement[i].ElapsedTime)
					{
						Debug.Log(l.Enemanagement[i].Enem);
						//その敵のロックオンしている時間がミサイルのロックオン時間のパラメーターを越えた場合、マーカーを緑にする
						if (l.Enemanagement[i].isLockOn)
						{
							//ロックオン完了
							l.Enemanagement[i].marker.GetComponent<Image>().color = new Color(0f, 1f, 0f, 1f);//緑
						}
						//その敵のロックオンしている時間がミサイルのロックオン時間のパラメーターより少ない場合、マーカーを黄色にする
						else
						{
							//ロックオン途中
							l.Enemanagement[i].marker.GetComponent<Image>().color = new Color(1f, 1f, 0f, 1f);//黄色
						}
					}
				}
			}
		}
	}
}
