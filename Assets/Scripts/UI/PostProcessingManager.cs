using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class PostProcessingManager : MonoBehaviour
{
    [SerializeField] private Volume _volume;
    [SerializeField] private AnimationCurve _startRewindEffectIntensity;
    [SerializeField] private AnimationCurve _stopRewindEffectIntensity;
    [SerializeField] private float _startDuration = 3f;
    [SerializeField] private float _stopDuration = 1f;

    private ChromaticAberration _chromatic;
    private LensDistortion _distortion;

    private IEnumerator _startEffectCourutine;

    void Awake()
    {
        _volume.profile.TryGet(out _chromatic);
        _volume.profile.TryGet(out _distortion);
    }

    public void StartRewind()
    {
        _startEffectCourutine = ApplyStartEffect();
        StartCoroutine(_startEffectCourutine);
    }

    public IEnumerator ApplyStartEffect()
    {
        float t = 0;
        while (t < _startDuration)
        {
            t += Time.deltaTime;
            var intensity = _startRewindEffectIntensity.Evaluate(t / _startDuration);

            if (_chromatic != null)
            {
                _chromatic.intensity.value = intensity;
            }
            if (_chromatic != null)
            {
                _distortion.intensity.value = intensity;
            }
            yield return null;
        }
    }

    public IEnumerator ApplyStopEffect()
    {
        float t = 0;
        while (t < _stopDuration)
        {
            t += Time.deltaTime;
            var intensity = _stopRewindEffectIntensity.Evaluate(t / _startDuration);

            if (_chromatic != null)
            {
                _chromatic.intensity.value = intensity;
            }
            if (_chromatic != null)
            {
                _distortion.intensity.value = intensity;
            }
            yield return null;
        }

        if (_chromatic != null)
        {
            _chromatic.intensity.value = 0;
        }
        if (_chromatic != null)
        {
            _distortion.intensity.value = 0;
        }
    }

    public void StopRewind()
    {
        StopCoroutine(_startEffectCourutine);

        StartCoroutine(ApplyStopEffect());
    }
}
