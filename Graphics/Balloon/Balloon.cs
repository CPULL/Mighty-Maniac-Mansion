using UnityEngine;

public class Balloon : MonoBehaviour {
  private static Balloon b;

  public TMPro.TextMeshPro text;
  public SpriteRenderer body;
  public SpriteRenderer tip;

  private void Awake() {
    b = this;
  }

  public static void Show(string message, Transform speaker) {
    b.text.text = message;

    Vector3 location = speaker.position;
    location.y += .5f;
    location.z = 0;
    b.transform.position = location;
  }

}


/*
 Show it immediately of the correct size
  Position it in a way it is stuck to the player at about the specified position (but try to respect the height and the tip should be over the head
  Handle left/right for the tip
  Show it for a while (should the mout of the speaker animate?
  Stop it when clicking (move to next dialogue line)
 */