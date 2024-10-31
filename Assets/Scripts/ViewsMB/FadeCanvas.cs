using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeCanvas : MonoBehaviour
{
    public static FadeCanvas Instance { get; private set; }

    [SerializeField] private Image _fadeImage;
    [SerializeField] private Text _fadeText;

    private float _timerToWait = 0;
    private float _timerToFade = 0;
    private const float _durationToWait = 0.5f;
    private const float _durationToFade = 1f;

    private Color _startColor;
    private Color _unfadeColor;

    private Color _startTextColor;
    private Color _unfadeTextColor;

    private Coroutine _fadeCoroutine;

    public void Init()
    {
        if (Instance == null)
        {
            Instance = this;
            InitColor();
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Instance.DestroyFadeCanvas();

            Instance = this;
            InitColor();
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void Unfade() =>
        _fadeCoroutine = StartCoroutine(UnfadeCor());

    public void DestroyFadeCanvas()
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);
        Destroy(this.gameObject);
    }

    private void InitColor()
    {
        _startColor = _fadeImage.color;
        _unfadeColor = _fadeImage.color;
        _unfadeColor.a = 0f;

        _startTextColor = _fadeText.color;
        _unfadeTextColor = _fadeText.color;
        _unfadeTextColor.a = 0f;
    }

    private IEnumerator UnfadeCor()
    {
        while (_timerToWait < _durationToWait)
        {
            _timerToWait += Time.unscaledDeltaTime;
            yield return null;
        }

        _fadeText.gameObject.SetActive(false);

        while (_timerToFade < _durationToFade)
        {
            _timerToFade += Time.unscaledDeltaTime;
            float index = _timerToFade / _durationToFade;
            _fadeImage.color = Color.Lerp(_startColor, _unfadeColor, index);
            //_fadeText.color = Color.Lerp(_startTextColor, _unfadeTextColor, index);
            yield return null;
        }

        Instance = null;
        Destroy(this.gameObject);
    }
}