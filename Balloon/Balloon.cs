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
  public float delay = 0;
  System.Action speakComplete = null;
  SpriteRenderer srAnchor;
  Vector3 prevAnchorPos = Vector3.negativeInfinity;
  Camera cam;
  readonly float offramp = 10; // How fast the baloon should go in the visible zone

  private void Awake() {
    GD.b = this;
    gameObject.SetActive(false);
    cam = Camera.main;
    txtrt = text.GetComponent<RectTransform>();
  }

  public static void Show(string message, Transform speaker, System.Action completeSpeaking) {
    GD.b.text.text = "";
    GD.b.gameObject.SetActive(false);

    GD.b.delay = 1;
    GD.b.speakComplete = completeSpeaking;
    GD.b.anchor = speaker;
    GD.b.size = GD.b.text.GetPreferredValues(message);

    GD.b.numWords = 1;
    int maxlen = 0;
    int len = 0;
    foreach (char c in message) {
      if (char.IsWhiteSpace(c)) GD.b.numWords++;
      len++;
      if (c == '\n') {
        if (maxlen < len) maxlen = len;
        len = 0;
      }
    }

    GD.b.delay = 1 + .25f * GD.b.numWords * Controller.textSpeed * Controller.textSpeed;

    GD.b.text.text = message;

    SpriteRenderer sr = speaker.GetChild(0).GetComponent<SpriteRenderer>();

    if (!sr.isVisible) {
      GD.b.gameObject.SetActive(false);
      GD.b.delay = 0;
      GD.b.speakComplete?.Invoke();
    }

    GD.b.srAnchor = sr;
    if (maxlen < 8) GD.b.size.x += 1;
    GD.b.boxc.size = GD.b.size;
    GD.b.gameObject.SetActive(true);

    GD.b.size.x += 1.5f;
    GD.b.size.y += 1f;
    GD.b.body.size = GD.b.size;
    GD.b.txtrt.sizeDelta = new Vector2(GD.b.size.x - 1.5f, GD.b.size.y - .5f);
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
    Bounds speakerBounds = srAnchor.bounds;
    Vector2 tlc = cam.WorldToScreenPoint(new Vector2(speakerBounds.min.x, speakerBounds.min.y));
    bool right = tlc.x > Screen.width * .5;
    float y = GD.b.txtrt.sizeDelta.y;
    bool bottom = tlc.y < Screen.height * (y < 3.2f ? .75f : .5f);

    if (right && bottom) {
      Vector3 location = new Vector3(speakerBounds.center.x, speakerBounds.max.y, 0) + Vector3.left * (size.x * .2f + .75f) + Vector3.up * size.y * .25f;
      tip.transform.localPosition = new Vector3(size.x * .5f - .9f, size.y * -.5f + 0.24f, 0);
      GD.b.transform.position = location;
      tip.flipX = false;
      tip.flipY = false;
    }
    else if (!right && bottom) {
      Vector3 location = new Vector3(speakerBounds.center.x, speakerBounds.max.y, 0) + Vector3.right * (size.x * .2f + .75f) + Vector3.up * size.y * .25f;
      tip.transform.localPosition = new Vector3(-size.x * .5f + .9f, size.y * -.5f + 0.24f, 0);
      GD.b.transform.position = location;
      tip.flipX = true;
      tip.flipY = false;
    }
    else if (right && !bottom) {
      Vector3 location = new Vector3(speakerBounds.center.x, speakerBounds.max.y, 0) + Vector3.left * (size.x * .2f + .75f) + Vector3.up * (size.y * .2f - 2);
      tip.transform.localPosition = new Vector3(size.x * .5f - .9f, -size.y * -.5f - 0.24f, 0);
      GD.b.transform.position = location;
      tip.flipX = false;
      tip.flipY = true;
    }
    else if (!right && !bottom) {
      Vector3 location = new Vector3(speakerBounds.center.x, speakerBounds.max.y, 0) + Vector3.right * (size.x * .2f + .75f) + Vector3.up * (size.y * .2f - 2);
      tip.transform.localPosition = new Vector3(-size.x * .5f + .9f, -size.y * -.5f - 0.24f, 0);
      GD.b.transform.position = location;
      tip.flipX = true;
      tip.flipY = true;
    }
  }


  void KeepItVisible() {
    Bounds bounds = body.bounds;
    bounds.Encapsulate(tip.bounds);

    Vector2 tl = new Vector2(bounds.min.x, bounds.min.y);
    Vector2 br = new Vector2(bounds.max.x, bounds.max.y);
    Vector2 tlc = cam.WorldToScreenPoint(tl);
    Vector2 brc = cam.WorldToScreenPoint(br);

    Vector3 pos = transform.position;

    if (tlc.x < -Screen.width || tlc.x > 2 * Screen.width || tlc.y < -Screen.height || tlc.y > 2 * Screen.height) {
      SetPosition();
    }

    if (tlc.x < 0) {
      pos.x += offramp * Time.deltaTime;
      transform.position = pos;
    }
    else if (brc.x > Screen.width) {
      pos.x -= offramp * Time.deltaTime;
      transform.position = pos;
    }

    if (tlc.y < 0) {
      pos.y += offramp * Time.deltaTime;
      transform.position = pos;
    }
    else if (brc.y > Screen.height) {
      pos.y -= offramp * Time.deltaTime;
      transform.position = pos;
    }
  }

  internal static void Stop() {
    GD.b.delay = -1;
    GD.b.gameObject.SetActive(false);
  }
}


