using System;
using System.Text;
using Dungeon.StatsAndCharacteristics;
using UnityEngine;

namespace Dungeon.UI
{
    public sealed class SimpleStatsDisplay : MonoBehaviour
    {
        #region PrivateData

        [SerializeField] private CharacteristicHolderBase _source;
        private StringBuilder _stringBuilder = new StringBuilder();

        #endregion


        #region UnityMethods

        void LateUpdate()
        {
            _stringBuilder.Clear();
            foreach (var characteristic in _source.Characteristics)
            {
                _stringBuilder.Append(
                    $"Current {Enum.GetName(typeof(CharacteristicType), characteristic.Type)}: " +
                    $"{characteristic.CurrentValue:0} / {characteristic.MaxValue:0}");
            }
        }

        void OnGUI()
        {
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, 0 + h*0.75f, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);

            string text = _stringBuilder.ToString();
            GUI.Label(rect, text, style);
        }

        #endregion
    }
}