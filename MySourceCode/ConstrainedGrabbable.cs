using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * このスクリプトはレバーをつかんだ際に、
 * 正しくレバーを動作させるためだけのスクリプトです
 * 特に変更を加える必要はありません
 * **/
public class ConstrainedGrabbable : OVRGrabbable
{
    [SerializeField]
    private Transform _handle;
    Rigidbody _handleRB;
    private bool _grabbed;
    private Vector3 startPos;
    private Quaternion startRot;
    // Start is called before the first frame update
    protected override void Start()
    {
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        base.Start();
        _handleRB = _handle.GetComponent<Rigidbody>();
    }

    public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        base.GrabBegin(hand, grabPoint);
        StartCoroutine(UpdateHandle());
        grabbedBy.GetComponentInChildren<Renderer>().enabled = false;
    }

    IEnumerator UpdateHandle()
    {
        _grabbed = true;
        while (_grabbed)
        {
            _handleRB.MovePosition(transform.position);
            yield return null;
        }
    }

    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        _grabbed = false;
        transform.position = _handle.position;
        transform.rotation = _handle.rotation;
        grabbedBy.GetComponentInChildren<Renderer>().enabled = true;
        base.GrabEnd(linearVelocity, angularVelocity);
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        this.transform.position = _handle.position;
        this.transform.rotation = new Quaternion(0, 0, 0, 0);
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }
}
