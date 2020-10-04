using UnityEngine;


namespace Dungeon.Environment
{
    [RequireComponent(typeof(Projector))]
    public sealed class ProjectorColorBlender : MonoBehaviour
    {
        #region PrivateData

        [SerializeField] Color[] _colors;
        [SerializeField, Range(0, 1)] float _speed;
        private Projector _projector;
        private Material _projectorMaterial;
        private int _targetColorIndex = 1;

        #endregion


        #region UnityMethods

        private void Start()
        {
            _projectorMaterial = GetComponent<Projector>().material;
            _projectorMaterial.color = _colors[0];
        }

        private void Update()
        {
            if (ColorDifferece(_projectorMaterial.color, _colors[_targetColorIndex]) < 0.01f)
            {
                _targetColorIndex++;
                _targetColorIndex %= _colors.Length;
            }

            _projectorMaterial.color = new Color(
                Mathf.Lerp(_projectorMaterial.color.r, _colors[_targetColorIndex].r, _speed * Time.deltaTime),
                Mathf.Lerp(_projectorMaterial.color.g, _colors[_targetColorIndex].g, _speed * Time.deltaTime),
                Mathf.Lerp(_projectorMaterial.color.b, _colors[_targetColorIndex].b, _speed * Time.deltaTime),
                Mathf.Lerp(_projectorMaterial.color.a, _colors[_targetColorIndex].a, _speed * Time.deltaTime));
        }

        #endregion


        #region Methods

        private float ColorDifferece(Color color1, Color color2)
        {
            return Mathf.Abs(color1.r - color2.r) +
                   Mathf.Abs(color1.g - color2.g) +
                   Mathf.Abs(color1.b - color2.b) +
                   Mathf.Abs(color1.a - color2.a);
        }

        #endregion
    }
}