using UnityEngine;

public class Sounds : MonoBehaviour {
  public AudioClip[] sounds;
  public AudioClip[] StepSounds;
  public AudioClip TentacleSteps;
  AudioSource source;
  public Transform ContinousSound;
  public AudioSource ContinousSoundSource;
  Audios continuosSound;

  private void Awake() {
    GD.s = this;
    source = GetComponent<AudioSource>();
  }

  public static void Play(Audios sound) {
    if (GD.s == null) return;

    int num = (int)sound;
    if (num < 0 || num >= GD.s.sounds.Length) return;

    GD.s.source.clip = GD.s.sounds[num];
    GD.s.source.Play();
  }

  public static void Play(Audios sound, Vector3 pos) {
    if (GD.s == null) return;

    GD.s.transform.position = pos;
    int num = (int)sound;
    if (num < 0 || num >= GD.s.sounds.Length) return;

    GD.s.source.clip = GD.s.sounds[num];
    GD.s.source.Play();
  }

  public static void Stop() {
    if (GD.s == null) return;
    GD.s.source.Stop();
  }

  public static AudioClip GetStepSound(FloorType floor) {
    int num = (int)floor;
    if (num < 0 || num >= GD.s.StepSounds.Length) return null;
    return GD.s.StepSounds[num];
  }

  public static AudioClip GetTentacle() {
    return GD.s.TentacleSteps;
  }

  internal static void PlayContinously(Audios sound, Vector2 pos) {
    if (GD.s == null) return;

    Debug.Log("Start sound " + sound);
    GD.s.ContinousSound.transform.position = pos;
    int num = (int)sound;
    if (num < 0 || num >= GD.s.sounds.Length) return;

    GD.s.continuosSound = sound;
    GD.s.ContinousSoundSource.clip = GD.s.sounds[num];
    GD.s.ContinousSoundSource.loop = true;
    GD.s.ContinousSoundSource.Play();
  }

  internal static void StopContinously(Audios sound) {
    Debug.Log("Stop sound " + sound);
    if (GD.s.continuosSound == sound)
      GD.s.ContinousSoundSource.Stop();
  }

  private void Update() {
    if (!ContinousSoundSource.isPlaying || GD.c == null || GD.c.currentActor == null) return;

    float vol = (ContinousSound.position - GD.c.currentActor.transform.position).sqrMagnitude * .00001f;
    if (vol > 1) vol = 1;
    if (vol < .1f) vol = .1f;
    ContinousSoundSource.volume = vol;
  }

}

/// <summary>
/// Used to list all possible sounds and musics
/// </summary>
public enum Audios { 
  Doorbell = 0,
  BrokenBottle = 1,
  Microwave = 2,
  MicrowaveBeep = 3,
  EggExplosion = 4,
  WaterDown = 5,
  WaterUp = 6,
  WheelValve = 7,
  Alarm = 8
};



