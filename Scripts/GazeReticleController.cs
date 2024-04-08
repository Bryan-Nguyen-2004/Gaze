using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GazeReticleController : MonoBehaviour
{
    [
        SerializeField,
        Tooltip("The gaze interactor to use. If not set, it will be automatically assigned.")
    ]
    private XRGazeInteractor gazeInteractor;
    [
        SerializeField,
        Tooltip("Whether to disable the reticle when not interacting with an object. Note that if false, the reticle will still only be visible if the gaze interactor raycast is hitting something.")
    ]
    private bool disableReticleWhileNotInteracting = true;
    private XRBaseInteractable gazeInteractable;
    private GazeReticle reticle;
    private float gazeTime;
    private float timeToSelect;
    private float timeToSelectDefault;
    private float timeToDeselect;
    private float timeToDeselectDefault;

    private void Start()
    {
        // Get the reticle prefab
        GameObject reticlePrefab = Resources.Load<GameObject>("gaze_reticle");
        if (reticlePrefab == null)
        {
            Debug.LogError("Reticle prefab not found in GazeReticleController.");
            return;
        }

        // Instantiate the reticle prefab
        GameObject reticleGO = Instantiate(reticlePrefab);
        if (disableReticleWhileNotInteracting)
            reticleGO.SetActive(false);

        // Get the GazeReticle component
        reticle = reticleGO.GetComponent<GazeReticle>();
        if (reticle == null)
        {
            Debug.LogError("GazeReticle component not found in GazeReticleController.");
            return;
        }

        // Get the gaze interactor
        gazeInteractor = GetComponent<XRGazeInteractor>();
        if (gazeInteractor == null)
        {
            Debug.LogError("Gaze interactor not found in GazeReticleController.");
            return;
        }
        reticle.SetInteractor(gazeInteractor);

        // Add listeners for when the gaze starts and stops
        gazeInteractor.hoverEntered.AddListener(StartGaze);
        gazeInteractor.hoverExited.AddListener(EndGaze);

        // Get the default time to select/deselect an object
        if (gazeInteractor.allowSelect)
            timeToSelectDefault = gazeInteractor.hoverTimeToSelect;
        if (gazeInteractor.autoDeselect)
            timeToDeselectDefault = gazeInteractor.timeToAutoDeselect;
    }

    private void Update()
    {
        // If the reticle is enabled, update its position
        if (reticle.gameObject.activeSelf || !disableReticleWhileNotInteracting)
        {
            gazeTime += Time.deltaTime;
            gazeInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit);

            // Update the reticle position
            reticle.SetTarget(hit);

            // Update the reticle progress if the object allows gaze selection/deselection else leave it at 0
            if (gazeInteractor.allowSelect && gazeInteractable && gazeInteractable.allowGazeSelect && gazeTime <= timeToSelect)
                reticle.SetProgress(gazeTime / timeToSelect);
            else if (gazeInteractor.autoDeselect && gazeInteractable && gazeInteractable.allowGazeSelect && gazeTime <= timeToDeselect)
                reticle.SetProgress(1 - (gazeTime - timeToSelect) / (timeToDeselect - timeToSelect));
        }
    }

    private void StartGaze(HoverEnterEventArgs args)
    {
        gazeInteractable = args.interactableObject as XRBaseInteractable;

        gazeInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit);

        // Enable the reticle
        if (disableReticleWhileNotInteracting)
            reticle.Enable(true);

        // Set the starting position of the reticle
        reticle.SetTarget(hit);
        reticle.SetProgress(0);

        // Get the time to select the object or use the default time
        if (gazeInteractable.overrideGazeTimeToSelect)
            timeToSelect = gazeInteractable.gazeTimeToSelect;
        else
            timeToSelect = timeToSelectDefault;

        // Get the time to deselect the object or use the default time (time to deselect = time to select + time to auto deselect)
        if (gazeInteractable.overrideTimeToAutoDeselectGaze)
            timeToDeselect = gazeInteractable.timeToAutoDeselectGaze + timeToSelect;
        else
            timeToDeselect = timeToDeselectDefault + timeToSelect;

        gazeTime = 0;
    }

    private void EndGaze(HoverExitEventArgs args)
    {
        gazeTime = 0;
        gazeInteractable = null;
        if (disableReticleWhileNotInteracting)
            reticle.Enable(false);
    }
}
