using System;
using UnityEngine;


namespace Voxon
{
    [RequireComponent(typeof(LineRenderer))]
    public class Line : MonoBehaviour
    {
        // Editor View (Won't use after initialisation)
        [SerializeField]
        private bool parented;
        [SerializeField]
        private Color32 colour;

        // Associated Models
        private LineModel _lineModel;

        // Associated Views
        private LineView _line;

        private LineRenderer _lineRenderer;
        // Use this for initialization
        private void Start()
        {
            try
            {
                _lineRenderer = GetComponent<LineRenderer>();
            }
            catch (Exception e)
            {
                ExceptionHandler.Except($"({name}) Failed to load suitable Line", e);
                Destroy(this);
            }

            var positions = new Vector3[_lineRenderer.positionCount];
            _lineRenderer.GetPositions(positions);

            _lineModel = new LineModel();
            _lineModel.SetColor(colour);
            _lineModel.Points = positions;

            if(parented) _lineModel.Parent = gameObject;

            _lineModel.Update();

            _line = new LineView(_lineModel);
        }

        // Update is called once per frame
        private void Update()
        {
            _lineModel.Update();
        }
    }
}