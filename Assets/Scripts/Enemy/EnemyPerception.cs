using UnityEngine;

//The layer that was missing between the raw sensor and the decision maker.
//
//Sight answers one question, every frame, from scratch: "is there a clear line to the player RIGHT NOW?"
//That is a fine question for a sensor and a terrible one for a brain. A brain that consumes it directly
//flickers whenever a doorframe clips the ray, and forgets everything the instant the ray breaks.
//
//EnemyPerception turns that per-frame boolean into something with continuity: an awareness value that
//BUILDS at a rate proportional to how good the sighting is, DECAYS when the sighting stops, and crosses
//labelled thresholds with hysteresis so the label cannot oscillate. The FSM reads the label, never the ray.
public class EnemyPerception : MonoBehaviour
{
    public enum Awareness
    {
        Unaware,     //nothing going on
        Suspicious,  //something is off — worth walking over and looking
        Alert        //that is the player, go
    }

    [Header("Sensor")]
    public Sight sight;

    [Header("Build-up (awareness per second)")]
    [Tooltip("Rate when the target is dead-centre and close — a clean, unmistakable sighting.")]
    public float gainAtBestQuality = 150.0f;
    [Tooltip("Rate when the target is clipping the far edge of the cone — a glimpse.")]
    public float gainAtWorstQuality = 30.0f;

    [Header("Decay")]
    public float decayPerSecond = 24.0f;
    [Tooltip("Awareness holds flat this long after contact is lost before it starts falling. This is what makes a doorframe or one dropped frame a non-event.")]
    public float decayDelay = 2.0f;

    [Header("Thresholds — enter/exit differ ON PURPOSE (hysteresis)")]
    public float suspiciousEnter = 35.0f;
    public float suspiciousExit = 15.0f;
    public float alertEnter = 100.0f;
    public float alertExit = 55.0f;

    [Header("Memory — the guard stays edgy after an alert")]
    [Tooltip("Once alerted, awareness will not decay below this floor for a while. Breaking line of sight buys you distance, not amnesia.")]
    public float alertedFloor = 30.0f;
    public float alertedFloorDuration = 25.0f;

    [Header("Cover")]
    [Tooltip("How strongly standing in cover slows awareness build-up, on top of the cone already shortening in Sight. 0 disables it.")]
    public float concealmentGainPenalty = 1.0f;

    [Header("Shouting to allies")]
    public float shoutRadius = 18.0f;
    public float shoutAwareness = 55.0f;
    public float shoutCooldown = 6.0f;

    public Awareness Level { get; private set; }
    //Raw sensor passthrough. Use this only when you need the ACTUAL collider (to land a hit); use Level
    //for every decision, or you are back to reading the ray and the flicker comes with it.
    public bool HasVisual => sight != null && sight.detected_object_ != null;
    public Collider VisualTarget => sight != null ? sight.detected_object_ : null;
    public Vector3 LastKnownPosition { get; private set; }
    public bool HasLastKnownPosition { get; private set; }
    public float LastSeenTime { get; private set; }
    public float Awareness01 => Mathf.Clamp01(_awareness / Mathf.Max(1.0f, alertEnter));

    private float _awareness;
    private float _alertedUntil;
    private float _lastStimulusTime = -Mathf.Infinity;
    private float _nextShoutTime;

    public void Initialise(Sight sensor)
    {
        sight = sensor;
        LastSeenTime = -Mathf.Infinity;
    }

    private void OnEnable()
    {
        EnemyAlertNetwork.Register(this);
    }

    private void OnDisable()
    {
        EnemyAlertNetwork.Unregister(this);
    }

    private void Update()
    {
        if (sight == null)
            return;

        float dt = Time.deltaTime;

        if (sight.detected_object_ != null)
        {
            //A clean look builds certainty fast; a glimpse at the edge of the cone builds it slowly.
            //This single line is why walking into the middle of a cone now feels different from being
            //half-visible at the far edge, which is exactly what "detection feels arbitrary" was about.
            float gain = Mathf.Lerp(gainAtWorstQuality, gainAtBestQuality, Mathf.Clamp01(sight.detection_quality_));

            //Cover slows how fast certainty builds. The Scout had this idea and kept it to itself; every
            //enemy gets it now. Ask the collider we actually detected, not the player singleton, so a
            //non-player target never gets the player's concealment applied to it.
            PlayerController seen = sight.detected_object_.GetComponentInParent<PlayerController>();
            if (seen != null)
                gain *= Mathf.Clamp01(1.0f - seen.Concealment * concealmentGainPenalty);

            _awareness += gain * dt;

            LastKnownPosition = sight.detected_object_.transform.position;
            HasLastKnownPosition = true;
            LastSeenTime = Time.time;
        }
        else if (Time.time - LastSeenTime >= decayDelay && Time.time - _lastStimulusTime >= decayDelay)
        {
            _awareness -= decayPerSecond * dt;
        }

        float floor = Time.time < _alertedUntil ? alertedFloor : 0.0f;
        _awareness = Mathf.Clamp(_awareness, floor, alertEnter);

        ResolveLevel();
    }

    //Anything that is not this enemy's own eyes: a noise, or an ally shouting that they found someone.
    public void ReportStimulus(Vector3 position, float awarenessBump)
    {
        _awareness = Mathf.Clamp(_awareness + awarenessBump, 0.0f, alertEnter);
        _lastStimulusTime = Time.time;

        //Only take someone else's word for WHERE if we have nothing fresher of our own.
        if (!HasVisual)
        {
            LastKnownPosition = position;
            HasLastKnownPosition = true;
        }

        ResolveLevel();
    }

    //Called when the guard has finished searching and genuinely found nothing — lets it stand down
    //instead of sitting on the alerted floor forever.
    public void StandDown()
    {
        _alertedUntil = 0.0f;
        _awareness = Mathf.Min(_awareness, suspiciousExit);
        HasLastKnownPosition = false;
        ResolveLevel();
    }

    //Hysteresis lives here: the value you need to ENTER a level is not the value you need to LEAVE it.
    //Without that gap, a target sitting exactly on the threshold flips the label every frame, and the
    //FSM below thrashes with it. This is the fix for the head-snapping, not a tuning problem.
    private void ResolveLevel()
    {
        Awareness previous = Level;

        switch (Level)
        {
            case Awareness.Unaware:
                if (_awareness >= suspiciousEnter)
                    Level = Awareness.Suspicious;
                break;

            case Awareness.Suspicious:
                if (_awareness >= alertEnter)
                    Level = Awareness.Alert;
                else if (_awareness <= suspiciousExit)
                    Level = Awareness.Unaware;
                break;

            case Awareness.Alert:
                if (_awareness <= alertExit)
                    Level = Awareness.Suspicious;
                break;
        }

        if (Level == Awareness.Alert && previous != Awareness.Alert)
        {
            _alertedUntil = Time.time + alertedFloorDuration;

            //Cooldown so two guards cannot bounce the same alert off each other every frame.
            if (Time.time >= _nextShoutTime)
            {
                _nextShoutTime = Time.time + shoutCooldown;
                EnemyAlertNetwork.Broadcast(this, LastKnownPosition, shoutRadius, shoutAwareness);
            }
        }
    }
}
