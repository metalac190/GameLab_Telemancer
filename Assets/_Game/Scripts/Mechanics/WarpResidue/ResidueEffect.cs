using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mechanics.WarpResidue
{
    public class ResidueEffect : MonoBehaviour
    {
        public Material residueBaseMaterial = null;

        private List<MeshRenderer> _meshRenderers;
        private List<Material> _originalMaterials;
        private List<Material> _residueMaterials;

        #region Unity Functions

        private void Awake()
        {
            GetMeshRenderers();
            CreateResidueMaterials();
        }

        private void OnEnable()
        {
            ActivateResidue();
        }

        private void OnDisable()
        {
            DisableResidue();
        }

        private void OnDestroy()
        {
            ClearMaterials();
        }

        #endregion

        private void GetMeshRenderers()
        {
            _meshRenderers = GetComponentsInChildren<MeshRenderer>().ToList();
            _meshRenderers.Add(GetComponent<MeshRenderer>());

            _meshRenderers = _meshRenderers.Where(r => r != null).ToList();

            _originalMaterials = new List<Material>();
            _residueMaterials = new List<Material>();

            foreach (var originalMat in _meshRenderers.Select(meshRenderer => meshRenderer.material)) {
                _originalMaterials.Add(originalMat);
            }
        }

        private void CreateResidueMaterials()
        {
            foreach (var material in _originalMaterials) {
                Material residueMat = Instantiate(residueBaseMaterial);
                // Set the residue Material custom shaders Base Material to originalMat
                // residueMat.shader... = material;
                residueMat.SetTexture("MainTexture", material.mainTexture);
                //residueMat.Lerp(residueMat, material, 0.5f);
                _residueMaterials.Add(residueMat);
            }
        }

        private void ActivateResidue()
        {
            for (var i = 0; i < _meshRenderers.Count; i++) {
                var meshRenderer = _meshRenderers[i];
                if (meshRenderer.materials.Length > 1) {
                    Debug.LogWarning("Mesh Renderers should not have more than one material", gameObject);
                }
                meshRenderer.material = _residueMaterials[i];
            }
        }

        public void DisableResidue()
        {
            for (var i = 0; i < _meshRenderers.Count; i++) {
                _meshRenderers[i].material = _originalMaterials[i];
            }
        }

        private void ClearMaterials()
        {
            foreach (var mat in _residueMaterials) {
                Destroy(mat);
            }
        }
    }
}