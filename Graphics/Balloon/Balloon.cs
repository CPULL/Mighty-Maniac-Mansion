using UnityEngine;

public class Balloon : MonoBehaviour {
  private static Balloon b;

  public TMPro.TextMeshPro text;
  public SpriteRenderer body;
  public SpriteRenderer tip;
  Transform anchor;
  Vector2 size = Vector2.zero;
  int numWords = 0;
  float delay = 0;
  System.Action speakComplete = null;

  private void Awake() {
    b = this;
    gameObject.SetActive(false);
  }

  public static void Show(string message, Transform speaker, System.Action completeSpeaking) {
    b.speakComplete = completeSpeaking;
    b.anchor = speaker;
    b.delay = 1000;
    b.gameObject.SetActive(true);
    b.size = b.text.GetPreferredValues(message);
    b.text.text = message;

    Vector3 location = speaker.position;
    location.y += 8f + b.size.y / 2;
    location.x -= 2f;
    location.z = -.5f;
    b.transform.position = location;

    b.size.x += .5f;
    b.size.y += .5f;
    b.body.size = b.size;

    b.tip.transform.localPosition = new Vector3(-b.size.x / 4, -b.size.y / 2 + 0.235f, 0);

    b.numWords = 1;
    foreach (char c in message)
      if (char.IsWhiteSpace(c)) b.numWords++;

    b.delay = 1.05f * b.numWords;
  }

  private void Update() {
    if (b.delay <= 0) return;
    b.delay -= Time.deltaTime;
    if (b.delay <= 0) {
      gameObject.SetActive(false);
      b.speakComplete?.Invoke();
      return;
    }

    Vector3 location = b.anchor.position;
    location.y += 8f + size.y / 2;
    location.x -= 2f;
    location.z = -.5f;
    b.transform.position = location;
    b.tip.transform.localPosition = new Vector3(-b.size.x / 4, -b.size.y / 2 + 0.235f, 0);
  }

}


/*
 Show it immediately of the correct size
  Position it in a way it is stuck to the player at about the specified position (but try to respect the height and the tip should be over the head
  Handle left/right for the tip
  Show it for a while (should the mout of the speaker animate?
  Stop it when clicking (move to next dialogue line)
 */