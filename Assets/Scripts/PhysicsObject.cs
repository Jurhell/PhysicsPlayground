using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshRenderer))]
public class PhysicsObject : MonoBehaviour
{
    [SerializeField]
    private Material _awakeMaterial;
    [SerializeField]
    private Material _asleepMaterial;

    private Rigidbody _rigidbody;
    private MeshRenderer _meshRenderer;
    private bool _wasSleeping = true;
    
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //_meshRenderer.material = _rigidbody.IsSleeping() ? _asleepMaterial : _awakeMaterial;

        UpdateMeshRenderer();
    }

    private void UpdateMeshRenderer()
    {
        if (_rigidbody.IsSleeping() && !_wasSleeping && _asleepMaterial != null)
        {
            _wasSleeping = true;
            _meshRenderer.material = _asleepMaterial;
        }
        if (!_rigidbody.IsSleeping() && _wasSleeping && _awakeMaterial != null)
        {
            _wasSleeping = false;
            _meshRenderer.material = _awakeMaterial;
        }
    }
}
