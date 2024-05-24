using UnityEngine;
using UnityEngine.UI;

namespace UIScripts
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Image _imageHP;

        /// <summary>
        /// Sets hp in percent
        /// </summary>
        /// <param name="percent"> From 0 to 1</param>
        public void SetHealthInPercent(float percent)
        { 
            _imageHP.fillAmount = percent;
        }
    }
}