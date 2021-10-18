using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AudioSystem;

namespace Mechanics.Bolt
{
    /// Summary:
    /// The script to implement the Residue Effect
    /// Searches the object and all child objects for MeshRenderers
    /// Creates a second material for each of those MeshRenderers to apply the Residue Shader
    public class ResidueEffect : MonoBehaviour
    {
        // Required basic material that uses the Custom Residue Effect Shader
        [SerializeField] private Material _residueBaseMaterial = null;
        [SerializeField] SFXLoop residueAudioSource = null;

        private List<MeshRenderer> _meshRenderers;
        private List<Material> _originalMaterials;
        private List<Material> _residueMaterials;
        private AudioSource sfxAudioSource;

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

        // Called on Awake, searches all child objects for MeshRenderers
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

        // Called on Awake, creates a second material for each object that uses the Residue Effect shader (on Base Material)
        private void CreateResidueMaterials()
        {
            if (_residueBaseMaterial == null) return;
            foreach (var material in _originalMaterials) {
                Material residueMat = Instantiate(_residueBaseMaterial);
                // Set the residue Material custom shaders Base Material to originalMat
                // residueMat.shader... = material;
                residueMat.SetTexture("MainTexture", material.mainTexture);
                //residueMat.Lerp(residueMat, material, 0.5f);
                _residueMaterials.Add(residueMat);
            }
        }

        // Called by another script to enable the residue effects
        // TODO: Lerp the shader noise from no residue to full residue?
        private void ActivateResidue()
        {
            if (_residueBaseMaterial == null) return;
            for (var i = 0; i < _meshRenderers.Count; i++) {
                var meshRenderer = _meshRenderers[i];
                if (meshRenderer.materials.Length > 1) {
                    Debug.LogWarning("Mesh Renderers should not have more than one material", gameObject);
                }
                meshRenderer.material = _residueMaterials[i];
            }
            if(residueAudioSource) sfxAudioSource = residueAudioSource.Play(transform.position);
        }

        // Called by another script to disable the residue effects
        public void DisableResidue()
        {
            for (var i = 0; i < _meshRenderers.Count; i++) {
                _meshRenderers[i].material = _originalMaterials[i];
            }
            if(sfxAudioSource) sfxAudioSource.Stop();
        }

        // Called on Destroy, clears all secondary materials
        private void ClearMaterials()
        {
            // Make sure the MeshRenderers are using the original material
            DisableResidue();
            foreach (var mat in _residueMaterials) {
                Destroy(mat);
            }
        }
    }
}