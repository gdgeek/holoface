using Academy.HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoloLensInput : MonoBehaviour {


    [Tooltip("Audio clip to play when interacting with this hologram.")]
    public AudioClip TargetFeedbackSound;
    public AudioSource audioSource;

    DragManager dManager_ = null;
   
    public virtual void Start()
    {

        dManager_ = this.gameObject.GetComponent<DragManager>();
        EnableAudioHapticFeedback();
    }

    public void EnableAudioHapticFeedback()
    {
        // If this hologram has an audio clip, add an AudioSource with this clip.
        if (TargetFeedbackSound != null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.clip = TargetFeedbackSound;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1;
            audioSource.dopplerLevel = 0;
        }
    }

    virtual public void GazeEntered()
    {

        // Play the audioSource feedback when we gaze and select a hologram.
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    virtual public void GazeExited()
    {

        // Play the audioSource feedback when we gaze and select a hologram.
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
    Vector3 manipulationPreviousPosition;
    void PerformNavigationStart(Vector3 position)
    {

        if (GestureManager.Instance.focusedGameObject_ != null)
        {
            DragItem item = GestureManager.Instance.focusedGameObject_.GetComponent<DragItem>();
            if (item != null)
            {
                dManager_.touchStart(item, position.x);
            }
        }
        // audioSource.Play();
    }
    void PerformNavigationUpdate(Vector3 position)
    {
        dManager_.touchDown(position.x);
    }

    void PerformNavigationRelease()
    {
        dManager_.touchUp();
    }


    virtual public void OnSelect()
    {
       
        // Play the audioSource feedback when we gaze and select a hologram.
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }

        // this.SendMessage("PerformTagAlong");
    }
}
