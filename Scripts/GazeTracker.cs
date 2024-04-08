using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GazeTracker : MonoBehaviour
{
    private XRBaseInteractor gazeInteractor;
    private Dictionary<XRBaseInteractable, float> gazeTimes = new Dictionary<XRBaseInteractable, float>();
    private XRBaseInteractable currentGazedObject;
    private float gazeStartTime;

    void Start()
    {
        gazeInteractor = GetComponent<XRGazeInteractor>();
        gazeInteractor.hoverEntered.AddListener(StartGaze);
        gazeInteractor.hoverExited.AddListener(EndGaze);
    }

    private void StartGaze(HoverEnterEventArgs args)
    {
        currentGazedObject = args.interactableObject as XRBaseInteractable;
        gazeStartTime = Time.time;
    }

    private void EndGaze(HoverExitEventArgs args)
    {
        XRBaseInteractable interactable = args.interactableObject as XRBaseInteractable;

        // If the object that stopped being gazed at is the same as the object that was originally gazed at
        if (currentGazedObject == interactable)
        {
            // Calculate the time the object was gazed at and add it to the dictionary
            float gazeTime = Time.time - gazeStartTime;
            if (gazeTimes.ContainsKey(interactable))
            {
                gazeTimes[interactable] += gazeTime;
            }
            else
            {
                gazeTimes[interactable] = gazeTime;
            }
            currentGazedObject = null;
        }
    }

    /// <summary>
    ///     Returns the total time the object has been gazed at, or -1 if the object has not been gazed at.
    /// </summary>
    /// <param name="interactable"></param>
    /// <returns>The total time the object has been gazed at.</returns>
    public float GetGazeTime(XRBaseInteractable interactable)
    {
        if (gazeTimes.ContainsKey(interactable))
        {
            return gazeTimes[interactable];
        }
        else
        {
            return -1f;
        }
    }

    /// <summary>
    ///    Returns a read-only dictionary of XRBaseInteractable objects and the total time they have been gazed at.
    ///    The key is the XRBaseInteractable object and the value is the total time the object has been gazed at.
    /// </summary>
    /// <returns>A read-only dictionary of XRBaseInteractable objects and the total time they have been gazed at.</returns>
    public IReadOnlyDictionary<XRBaseInteractable, float> GetGazeTimes()
    {
        // Make the dictionary read-only so that it cannot be modified
        return new ReadOnlyDictionary<XRBaseInteractable, float>(gazeTimes);
    }

    /// <summary>
    ///    Prints the total time each XRBaseInteractable object has been gazed at to the console.
    /// </summary>
    public void PrintGazeTimes()
    {
        foreach (KeyValuePair<XRBaseInteractable, float> gazeTime in gazeTimes)
        {
            Debug.Log(gazeTime.Key.name + " has been gazed at for " + gazeTime.Value + " seconds.");
        }
    }
}
