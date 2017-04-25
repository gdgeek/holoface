using UnityEngine;

/// <summary>
/// The Interactible class flags a Game Object as being "Interactible".
/// Determines what happens when an Interactible is being gazed at.
/// </summary>
public class DragInteractible : Interactible
{
    public override void Start()
    {
        EnableAudioHapticFeedback();
    }


    override public void GazeEntered()
    {
    }

    override public void GazeExited()
    {
       
    }

    override public void OnSelect()
    {
      
        
    }


}