using UnityEngine;

public class Sounds : MonoBehaviour {
  public AudioClip[] sounds;
  public AudioClip[] StepSounds;
  public AudioClip TentacleSteps;
  AudioSource source;

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



