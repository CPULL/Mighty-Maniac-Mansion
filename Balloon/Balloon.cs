using UnityEngine;

public class Balloon : MonoBehaviour {
  public TMPro.TextMeshPro text;
  public SpriteRenderer body;
  public SpriteRenderer tip;
  public BoxCollider2D boxc;
  private RectTransform txtrt;
  Transform anchor;
  Vector2 size = Vector2.zero;
  int numWords = 0;
  float delay = 0;
  System.Action speakComplete = null;
  SpriteRenderer srAnchor;

  private void Awake() {
    GD.b = this;
    gameObject.SetActive(false);
    cam = Camera.main;
    txtrt = text.GetComponent<RectTransform>();
  }

  public static void Show(string message, Transform speaker, System.Action completeSpeaking) {
    GD.b.delay = 1;
    GD.b.speakComplete = completeSpeaking;
    GD.b.anchor = speaker;
    GD.b.size = GD.b.text.GetPreferredValues(message);

    GD.b.numWords = 1;
    foreach (char c in message)
      if (char.IsWhiteSpace(c)) GD.b.numWords++;

    GD.b.delay = 1 + .25f * GD.b.numWords * Controller.textSpeed * Controller.textSpeed;

    GD.b.text.text = message;
    GD.b.gameObject.SetActive(true);

    GD.b.srAnchor = speaker.GetChild(0).GetComponent<SpriteRenderer>();
    GD.b.boxc.size = GD.b.size;
    GD.b.boxc.offset = new Vector2(-1.5f, .5f * GD.b.size.y + 1.625f + .2f);

    GD.b.size.x += .5f;
    GD.b.size.y += .5f;
    GD.b.body.size = GD.b.size;
    GD.b.txtrt.sizeDelta = new Vector2(GD.b.size.x - .5f, GD.b.size.y);
    GD.b.SetPosition();
  }

  private void Update() {
    if (GD.b.delay <= 0) return;
    GD.b.delay -= Time.deltaTime;
    if (GD.b.delay <= 0) {
      gameObject.SetActive(false);
      GD.b.speakComplete?.Invoke();
      return;
    }

    KeepItVisible();

    if (prevAnchorPos == GD.b.anchor.position) return;
    prevAnchorPos = GD.b.anchor.position;
    SetPosition();
  }

  void SetPosition() {
    // Get the top-left bounding box of the speaker
    Bounds spbnd = srAnchor.bounds;
    Vector3 location = new Vector3(spbnd.min.x, spbnd.max.y, 0) + Vector3.right * .25f - Vector3.up * .2f;

    tip.transform.localPosition = new Vector3(transform.position.x - location.x - 1, 1.86f, 0);
    GD.b.transform.position = location;
  }

  void KeepItVisible() {
    Bounds bounds = body.bounds;
    bounds.Encapsulate(tip.bounds);

    Vector2 tl = new Vector2(bounds.min.x, bounds.min.y);
    Vector2 br = new Vector2(bounds.max.x, bounds.max.y);
    Vector2 tlc = cam.WorldToScreenPoint(tl);
    Vector2 brc = cam.WorldToScreenPoint(br);

    Vector3 pos = transform.position;

    if (tlc.x < 0) {
      pos.x += offramp * Time.deltaTime;
      transform.position = pos;
    }
    else if (brc.x > Screen.width) {
      pos.x -= offramp * Time.deltaTime;
      transform.position = pos;
    }
    tip.flipX = (srAnchor.bounds.center.x < tip.bounds.center.x);

    if (tlc.y < 0) {
      pos.y += offramp * Time.deltaTime;
      transform.position = pos;
    }
    else if (brc.y > Screen.height) {
      pos.y -= offramp * Time.deltaTime;
      transform.position = pos;
    }
  }

  private void OnMouseDown() {
    GD.b.delay = Time.deltaTime;
  }

  Vector3 prevAnchorPos = Vector3.negativeInfinity;

  Camera cam;
  readonly float offramp = 10; // How fast the baloon should go in the visible zone
}


