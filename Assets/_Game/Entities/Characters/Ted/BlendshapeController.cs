using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendshapeController : MonoBehaviour
{
    [SerializeField] public SkinnedMeshRenderer _mesh;
    [SerializeField] public GameObject[] _trackerJoints;

    private void FixedUpdate()
    {
        BlendShapeUpdate();
    }
    void BlendShapeUpdate()
    {
        for (int i = 0; i < _trackerJoints.Length; i++)
        {
            _mesh.SetBlendShapeWeight(i, _trackerJoints[i].transform.localScale.x * 100);
        }
    }
}
