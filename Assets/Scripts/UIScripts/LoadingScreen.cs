using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UIScripts
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Canvas _loadingCanvas;

        private void Awake()
        {
            DontDestroyOnLoad(_loadingCanvas.gameObject);
        }

        /// <summary>
        /// Sets hp in percent
        /// </summary>
        /// <param name="percent"> From 0 to 1</param>
        public void SetProgressInPercent(float percent)
        {
            _fillImage.fillAmount = percent;
        }

        public void SetText(string text)
        {
            _text.text = text;
        }
    }
}