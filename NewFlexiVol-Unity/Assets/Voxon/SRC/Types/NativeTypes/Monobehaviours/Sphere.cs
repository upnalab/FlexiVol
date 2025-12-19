using UnityEngine;


namespace Voxon
{
    public class Sphere : MonoBehaviour
    {
        // Editor View (Won't use after initialisation)
        [SerializeField]
        bool parented;
        [SerializeField]
        Vector3[] vPosition = new Vector3[1];
        [SerializeField]
        Color32 colour;
        [SerializeField]
        int fill;
        [SerializeField]
        float radius;
        // Associated Models
        private SphereModel _sphereModel;

        // Associated Views
        private SphereView _sphere;
        // Use this for initialization
        private void Start()
        {
            _sphereModel = new SphereModel {Position = vPosition, Fill = fill, Radius = radius};
            _sphereModel.SetColor(colour);

            if (parented) _sphereModel.Parent = gameObject;

            _sphereModel.Update();

            _sphere = new SphereView(_sphereModel);
        }

        private void Update()
        {
            _sphereModel.Update();
        }

        private void OnValidate()
        {
            _sphere?.Destroy();

            _sphereModel = null;
            _sphereModel = new SphereModel {Position = vPosition, Fill = fill, Radius = radius};
            _sphereModel.SetColor(colour);

            if (parented) _sphereModel.Parent = gameObject;

            _sphereModel.Update();

            _sphere = new SphereView(_sphereModel);
        }

        private void OnDisable()
        {
            _sphere.Destroy();
        }

        private void OnDestroy()
        {
            _sphere.Destroy();
        }

        private void OnDrawGizmos()
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = new Color(colour.r, colour.g, colour.b);
            Gizmos.DrawSphere(vPosition[0]+gameObject.transform.position, radius*5);
        }
    }
}