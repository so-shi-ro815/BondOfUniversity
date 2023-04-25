using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusText : MonoBehaviour
{
    bool juage = false;
    public Text apText;
    public Text boostText;
    public Text ammoText;
    public float displayAp;
    public float displayBst;
    public int displayAmmo;
    public float ap;
    public float bst;
    public int ammo;
    public GameObject unit;
    public GameObject weapon;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (juage)
        {
            ap = unit.GetComponent<Unit>().GetCurrentArmorPoint();
            bst = unit.GetComponent<Unit>().GetCurrentBoostCapacity();
            ammo = weapon.GetComponent<BulletSpawn>().GetAmmo();
            if (displayAp != ap)
            {
                displayAp = Mathf.Lerp(displayAp, ap, 0.1f);
            }
            if (displayBst != bst)
            {
                displayBst = Mathf.Lerp(displayBst, bst, 0.1f);
            }
            displayAmmo = ammo;
            apText.text = string.Format("{0000:F0}", displayAp);
            boostText.text = string.Format("{0000:F0}", displayBst);
            ammoText.text = string.Format("{0000:F0}", displayAmmo);
        }
    }
    public void TextActive(GameObject go)
    {
        unit = go;
        weapon = go.transform.Find("MainCamera").transform.Find("Gun").gameObject;
        displayBst = unit.GetComponent<Unit>().GetMaxBoostCapacity();
        displayAp = unit.GetComponent<Unit>().GetMaxBoostCapacity();
        displayAmmo = weapon.GetComponent<BulletSpawn>().GetMaxAmmo();
        apText.text = string.Format("{0000:F0}", displayAp);
        boostText.text = string.Format("{0000:F0}", displayBst);
        ammoText.text = string.Format("{0000:F0}", displayAmmo);
        juage = true;
    }
}
