using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContextUIParticle : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _text;
    private Camera _camera;
    private Canvas _canvas;
    private RectTransform _parent;
    private UniTaskCompletionSource<bool> _tcs;
    public UniTask Completion => _tcs.Task;

    public void Initialize(Camera camera, Canvas canvas, RectTransform particlesParent)
    {
        _camera = camera;
        _canvas = canvas;
        _parent = particlesParent;
    }

    public void Setup(ContextUIParticleData data)
    {
        _image.enabled = data.Icon != null;
        _image.sprite = data.Icon;
        _text.gameObject.SetActive(!string.IsNullOrEmpty(data.Text));
        _text.text = data.Text;
        _text.color = data.TextColor;
        _tcs = new UniTaskCompletionSource<bool>();
        StartCoroutine(TravelRoutine(data));
    }

    private IEnumerator TravelRoutine(ContextUIParticleData data)
    {
        float elapsed = 0f;
        float duration = data.Design.AnimationSpeed;

        Vector2 dataDelta = data.Design.SpawnDelta;
        Vector2 delta = new Vector2(Random.Range(-dataDelta.x, dataDelta.x), Random.Range(-dataDelta.y, dataDelta.y));
        Vector2 start = data.StartPos.ToScreenPos(_camera, _canvas, _parent) + delta;
        Vector2 end = data.EndPos.ToScreenPos(_camera, _canvas, _parent);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = data.Design.SpeedCurve.Evaluate(elapsed / duration);
            float x = Mathf.Lerp(start.x, end.x, data.Design.ParticleAnimationX.Evaluate(t));
            float y = Mathf.Lerp(start.y, end.y, data.Design.ParticleAnimationY.Evaluate(t));
            float scale = data.Design.SizeAnimation.Evaluate(t);
            transform.localScale = new Vector3(scale, scale, 1);

            transform.localPosition = new Vector3(x, y, transform.position.z);

            yield return null;
        }

        _tcs.TrySetResult(true);

        gameObject.SetActive(false);
    }
}
