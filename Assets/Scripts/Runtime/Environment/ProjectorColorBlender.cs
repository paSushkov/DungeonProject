using UnityEngine;


namespace Dungeon.Environment
{
    [RequireComponent(typeof(Projector))]
    public sealed class ProjectorColorBlender : MonoBehaviour
    {
        #region PrivateData

        [SerializeField] Color[] colors = new Color[1];
        [SerializeField, Range(0, 1)] float speed = 1f;
        private Projector _projector;
        private Material _projectorMaterial;
        private int _targetColorIndex = 1;

        #endregion


        #region UnityMethods

        private void Start()
        {
            _projectorMaterial = GetComponent<Projector>().material;
            _projectorMaterial.color = colors[0];
        }

        private void Update()
        {
            if (ColorDifferece(_projectorMaterial.color, colors[_targetColorIndex]) < 0.01f)
            {
                _targetColorIndex++;
                _targetColorIndex %= colors.Length;
            }

            _projectorMaterial.color = new Color(
                Mathf.Lerp(_projectorMaterial.color.r, colors[_targetColorIndex].r, speed * Time.deltaTime),
                Mathf.Lerp(_projectorMaterial.color.g, colors[_targetColorIndex].g, speed * Time.deltaTime),
                Mathf.Lerp(_projectorMaterial.color.b, colors[_targetColorIndex].b, speed * Time.deltaTime),
                Mathf.Lerp(_projectorMaterial.color.a, colors[_targetColorIndex].a, speed * Time.deltaTime));
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