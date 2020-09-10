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
    Vector2 size = b.text.GetPreferredValues(message);
    b.text.text = message;

    Vector3 location = speaker.position;
    location.y += 8f + size.y / 2;
    location.x -= 2f;
    location.z = -.5f;
    b.transform.position = location;

    size.x += .5f;
    size.y += .5f;
    b.body.size = size;

    b.tip.transform.localPosition = new Vector3(-size.x / 4, -size.y / 2 + 0.235f, 0);
  }


}


/*
 Show it immediately of the correct size
  Position it in a way it is stuck to the player at about the specified position (but try to respect the height and the tip should be over the head
  Handle left/right for the tip
  Show it for a while (should the mout of the speaker animate?
  Stop it when clicking (move to next dialogue line)
 */