using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCanvas : MonoBehaviour
{
    [SerializeField] private Image _fmodLogo;
    [SerializeField] private Image _studioLogo;
    [SerializeField] private Image _backImage;

    [SerializeField] private float _animationDuration = 1.5f;

    private void Start()
    {
        Color fadeColor = Color.white;
        fadeColor.a = 0f;

        _fmodLogo.color = fadeColor;
        _studioLogo.color = fadeColor;

        StartCoroutine(LoadCor());
    }

    private IEnumerator LoadCor()
    {
        float fadeDuration = _animationDuration;
        float timer = 0;

        while (timer < _animationDuration/2f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        Color currentColor = Color.white;
        currentColor.a = 0;

        timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            currentColor.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            _fmodLogo.color = currentColor;
            yield return null;
        }

        timer = 0;
        while (timer < _animationDuration /2f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;
        while(timer < fadeDuration)
        {
            timer += Time.deltaTime;
            currentColor.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            _fmodLogo.color = currentColor;
            yield return null;
        }

        timer = 0;
        while (timer < _animationDuration / 2f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            currentColor.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            _studioLogo.color = currentColor;
            yield return null;
        }

        timer = 0;
        while (timer < _animationDuration / 2f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            currentColor.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            _studioLogo.color = currentColor;
            yield return null;
        }

        timer = 0;
        while (timer < _animationDuration/2f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        StopBlockRaycasts();

        timer = 0;
        currentColor = Color.black;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            currentColor.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            _backImage.color = currentColor;
            yield return null;
        }

        Destroy(gameObject);
    }

    private void StopBlockRaycasts()
    {
        _fmodLogo.raycastTarget = false;
        _studioLogo.raycastTarget = false;
        _backImage.raycastTarget = false;
    }
}