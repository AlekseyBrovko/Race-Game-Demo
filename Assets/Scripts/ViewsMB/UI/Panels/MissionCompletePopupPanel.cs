using Client;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Panels
{
    public class MissionCompletePopupPanel : PanelBase, IPopupPanel //или secondPanel
    {
        [SerializeField] private Image _maskImage;
        [SerializeField] private RectMask2D _rectMask;

        private Coroutine _animationCor;
        private float _xSoftness;
        private float _rectMaskWidth;
        private float _duration = 4f;

        public override void Initialize(string id, GameState state, CanvasBehaviour canvasMb, IOpenPanelData openData = null)
        {
            base.Initialize(id, state, canvasMb, openData);

            _xSoftness = _rectMask.softness.x;
            _rectMaskWidth = _maskImage.rectTransform.sizeDelta.x;
            Vector4 padding = new Vector4(_rectMaskWidth / 2f, 0f, _rectMaskWidth / 2f, 0f);
            _rectMask.padding = padding;

            Debug.Log($"_xSoftness = {_xSoftness}; _rectMaskWidth  = {_rectMaskWidth};");
            _animationCor = StartCoroutine(WinAnimationCor());
        }

        private IEnumerator WinAnimationCor()
        {
            float firstTimerDuration = _duration * 0.4f;
            float firstTimer = 0;

            while (firstTimer < firstTimerDuration)
            {
                firstTimer += Time.deltaTime;
                float tempValue = Mathf.Lerp(_rectMaskWidth / 2f, -_xSoftness, firstTimer / firstTimerDuration);
                Vector4 padding = new Vector4(tempValue, 0, tempValue, 0);
                _rectMask.padding = padding;
                yield return null;
            }

            float secondTimerDuration = _duration * 0.2f;
            float secondTimer = 0;
            while (secondTimer < secondTimerDuration)
            {
                secondTimer += Time.deltaTime;
                yield return null;
            }

            float thirdTimerDuration = _duration * 0.4f;
            float thirdTimer = 0;
            while (thirdTimer < thirdTimerDuration)
            {
                thirdTimer += Time.deltaTime;
                float tempValue = Mathf.Lerp(-_xSoftness, _rectMaskWidth / 2f, thirdTimer / thirdTimerDuration);
                Vector4 padding = new Vector4(tempValue, 0, tempValue, 0);
                Debug.Log("padding");
                _rectMask.padding = padding;
                yield return null;
            }

            _canvasMb.DestroyPanelById(this.Id);
        }

        public override void CleanupPanel()
        {
            if (_animationCor != null)
                StopCoroutine(_animationCor);
        }
    }
}