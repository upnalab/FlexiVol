using UnityEngine;


namespace Voxon
{
    public class Box : MonoBehaviour
    {
        // Editor View (Won't use after initialisation)
        [SerializeField] private bool parented;
        [SerializeField] private Vector3[] vPosition = new Vector3[2];
        [SerializeField] private Color32 colour;
        [SerializeField] private int fill;
        // Associated Models
        private BoxModel _boxModel;

        // Associated Views
        private BoxGizmoView _boxGizmoView;

        private BoxView _box;
        // Use this for initialization
        private void Start()
        {
            _boxModel = new BoxModel {Position = vPosition, Fill = fill};
            _boxModel.SetColor(colour);

            if (parented) _boxModel.Parent = gameObject;

            _boxModel.Update();

            _box = new BoxView(_boxModel);
            _boxGizmoView = new BoxGizmoView(_boxModel);
        }

        private void Update()
        {
            _boxModel.Update();
        }

        private void OnDrawGizmos()
        {
            _boxGizmoView?.DrawGizmo();
        }


        private void OnValidate()
        {
            _box?.Destroy();

            _boxModel = null;
            _boxModel = new BoxModel {Position = vPosition, Fill = fill};
            _boxModel.SetColor(colour);

            if (parented) _boxModel.Parent = gameObject;

            _boxModel.Update();

            _box = new BoxView(_boxModel);
            _boxGizmoView = new BoxGizmoView(_boxModel);
        }

        private void OnDisable()
        {
            _box.Destroy();
        }

        private void OnDestroy()
        {
            _box.Destroy();
        }

    }
}