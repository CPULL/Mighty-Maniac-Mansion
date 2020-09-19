using UnityEngine;

public class Balloon : MonoBehaviour {
  private static Balloon b;

  public TMPro.TextMeshPro text;
  public SpriteRenderer body;
  public SpriteRenderer tip;
  public BoxCollider2D boxc;
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
    b.delay = 1;
    b.speakComplete = completeSpeaking;
    b.anchor = speaker;
    b.gameObject.SetActive(true);
    b.size = b.text.GetPreferredValues(message);
    b.text.text = message;

    Vector3 location = b.anchor.position + b.transform.up * b.size.y * .15f - b.transform.right * b.size.x * 0.05f;
    b.transform.position = location;
    b.transform.rotation = b.anchor.rotation;

    b.size.x += .5f;
    b.size.y += .5f;
    b.body.size = b.size;
    b.boxc.size = b.size;

    b.tip.transform.localPosition = new Vector3(-b.size.x / 4, -b.size.y / 2 + 0.235f, 0);

    b.numWords = 1;
    foreach (char c in message)
      if (char.IsWhiteSpace(c)) b.numWords++;

    b.delay = .15f * b.numWords * Controller.textSpeed * Controller.textSpeed;
  }

  private void Update() {
    if (b.delay <= 0) return;
    b.delay -= Time.deltaTime;
    if (b.delay <= 0) {
      gameObject.SetActive(false);
      b.speakComplete?.Invoke();
      return;
    }

    Vector3 location = b.anchor.position + transform.up * b.size.y * .15f + transform.right * b.size.x * 0.05f;
    b.transform.position = location;
    b.transform.rotation = b.anchor.rotation;
    b.tip.transform.localPosition = new Vector3(-b.size.x / 4, -b.size.y / 2 + 0.235f, 0);
  }

  private void OnMouseDown() {
    b.delay = Time.deltaTime;
  }
}


