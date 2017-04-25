using UnityEngine;

/// <summary>
/// The Interactible class flags a Game Object as being "Interactible".
/// Determines what happens when an Interactible is being gazed at.
/// </summary>
public class Interactible : MonoBehaviour
{
    [Tooltip("Audio clip to play when interacting with this hologram.")]
    public AudioClip TargetFeedbackSound;
    public AudioSource audioSource;

   private Material[] defaultMaterials = null;

    public virtual void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null) {
            defaultMaterials = GetComponent<Renderer>().materials;
        }
        // defaultMaterials = GetComponent<Renderer>().materials;

        // Add a BoxCollider if the interactible does not contain one.
        Collider collider = GetComponentInChildren<Collider>();
        if (collider == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }

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
        if(defaultMaterials != null) { 
            for (int i = 0; i < defaultMaterials.Length; i++)
            {
                defaultMaterials[i].SetFloat("_Highlight", .25f);
            }
        }
    }

    virtual public void GazeExited()
    {
        if (defaultMaterials != null)
        {
            for (int i = 0; i < defaultMaterials.Length; i++)
           {
               defaultMaterials[i].SetFloat("_Highlight", 0f);
           }
        }
    }

    virtual public void OnSelect()
    {
        if (defaultMaterials != null)
        {
            for (int i = 0; i < defaultMaterials.Length; i++)
            {
                defaultMaterials[i].SetFloat("_Highlight", .5f);
            }
        }

        // Play the audioSource feedback when we gaze and select a hologram.
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }

       // this.SendMessage("PerformTagAlong");
    }
}