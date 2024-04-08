using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

/*
 * This file contains code written by Tomaz Saraiva.
 * Source: https://github.com/tomazsaraiva/unity-gaze-interaction
 */

public class GazeReticle : MonoBehaviour
{
    [Header("Inner")]
    [SerializeField]
    private Canvas _canvas;

    [SerializeField]
    private Image _imageProgress;

    [Header("Configuration")]
    [SerializeField]
    private float _scale = 0.0015f;

    [SerializeField]
    private float _offsetFromHit = 0.1f;

    private XRGazeInteractor _interactor;

    private void Start()
    {
        _canvas.transform.localScale = Vector3.one * _scale;
    }

    private void Update()
    {
        if (_interactor == null)
        {
            return;
        }

        var distance = Vector3.Distance(_interactor.transform.position, transform.position);
        var scale = distance * _scale;
        scale = Mathf.Clamp(scale, _scale, scale);
        _canvas.transform.localScale = Vector3.one * scale;
    }

    /// <summary>
    /// Assigns the GazeInteractor using this reticle.
    /// </summary>
    /// <param name="interactor"></param>
    public void SetInteractor(XRGazeInteractor interactor)
    {
        _interactor = interactor;
        enabled = true;
        Debug.Log(_interactor);
    }

    /// <summary>
    /// Toggles the GameObject.
    /// </summary>
    /// <param name="enable"></param>
    public void Enable(bool enable)
    {
        gameObject.SetActive(enable);
    }

    /// <summary>
    /// Assigns the current hit point to adjust the reticle position and rotation.
    /// </summary>
    /// <param name="hit"></param>
    public void SetTarget(RaycastHit hit)
    {
        Debug.Log(_interactor.transform.position);
        Debug.Log(transform.position);

        var direction = _interactor.transform.position - hit.point;
        var rotation = Quaternion.FromToRotation(Vector3.forward, direction);
        var position = hit.point + transform.forward * _offsetFromHit;

        transform.SetPositionAndRotation(position, rotation);
    }

    /// <summary>
    /// Updates the progress visual indicator.
    /// </summary>
    /// <param name="progress"></param>
    public void SetProgress(float progress)
    {
        _imageProgress.fillAmount = progress;
    }

    [CustomEditor(typeof(GazeReticle))]
    public class GazeReticleInspector : Editor
    {
        private GazeReticle _target;

        void OnEnable()
        {
            _target = (GazeReticle)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Editor");
        }
    }
}
