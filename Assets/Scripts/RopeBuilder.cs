using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SpringJoint))]
public class RopeBuilder : MonoBehaviour
{
    [SerializeField]
    private GameObject _ropePrefab;

    [SerializeField]
    private float _numberOfRope;

    private List<GameObject> _ropes = new List<GameObject>();
    private Vector3 _newPosition;

    // Start is called before the first frame update
    void Start()
    {
        _ropes.Add(transform.gameObject);
        if (_numberOfRope > 0)
        {
            for (int i = 1; i < _numberOfRope; i++)
                AddNewRope();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetMouseButtonDown(1))
            return;
        
        AddNewRope();
    }

    private void AddNewRope()
    {
        int lastIndex = _ropes.Count - 1;
        _newPosition = _ropes[lastIndex].transform.position - new Vector3(0f, 1f, 0f);

        GameObject temp = Instantiate(_ropePrefab, _newPosition, Quaternion.identity, transform);
        SpringJoint joint = temp.GetComponent<SpringJoint>();
        temp.GetComponent<Rigidbody>().isKinematic = false;
        joint.connectedBody = _ropes[lastIndex].transform.gameObject.GetComponent<Rigidbody>();
        joint.spring = 1000;
        joint.damper = 100;
        _ropes.Add(temp);
    }
}
