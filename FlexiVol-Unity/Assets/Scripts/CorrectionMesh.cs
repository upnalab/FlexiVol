using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Voxon;

public class CorrectionMesh : MonoBehaviour
{

    public bool needsDeformation = true;
    Vector3[] originalVertexPosition;
    Vector3[] vertices;

    private VXDynamicComponent vxComponent;
    public Mesh mesh;
    MeshFilter meshFilter;

    Vector3 previousPos;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        vxComponent = GetComponent<VXDynamicComponent>();
        originalVertexPosition = mesh.vertices;
        vertices = mesh.vertices;

        // Obtener el componente MeshFilter del objeto
        meshFilter = GetComponent<MeshFilter>();
        mesh.vertices = originalVertexPosition;

        if (meshFilter != null)
        {
            // Obtener la malla del MeshFilter
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            mesh = meshFilter.mesh;
            Destroy(sphere);

            if (needsDeformation)
            {
                // Obtener y modificar los vértices de la malla
                Vector3[] vertices = mesh.vertices;
                for (int i = 0; i < vertices.Length; i++)
                {
                    // Asignar una altura aleatoria en el eje Y entre minHeight y maxHeight
                    Vector3 globalVertex = transform.TransformPoint(originalVertexPosition[i]);
                    float newHeight;
                    newHeight = provissionalName(globalVertex.x, globalVertex.y);

                    // Now we transform from global to local
                    Vector3 localPoint = transform.InverseTransformPoint(globalVertex.x, newHeight, globalVertex.z);

                    vertices[i].y = localPoint.y;

                }

                // Asignar los vértices modificados a la malla
                mesh.vertices = vertices;
                needsDeformation = true;
            }

            // Recalcular la malla para actualizar la geometría
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            // if (vxComponent == null)
            // {
            //     vxComponent = gameObject.AddComponent<Voxon.VXComponent>();
            //     vxComponent.CanExpire = true;
            //     vxComponent.automaticallyExpire = true;
            //     vxComponent.TimeToExpire = 0.1f;

            //     Debug.Log("EXPIRE");

            // }

        }
        else
        {
            Debug.LogError("No se encontró un MeshFilter en el objeto.");
        }
    }

    void Update()
    {
        // Obtener el componente MeshFilter del objeto
        meshFilter = GetComponent<MeshFilter>();
        mesh.vertices = originalVertexPosition;

        if (meshFilter != null)
        {

            needsDeformation = previousPos != transform.position;

            if (needsDeformation)
            {
                // Obtener y modificar los vértices de la malla
                vertices = mesh.vertices;
                for (int i = 0; i < vertices.Length; i++)
                {
                    // Asignar una altura aleatoria en el eje Y entre minHeight y maxHeight
                    Vector3 globalVertex = transform.TransformPoint(originalVertexPosition[i]);
                    float newHeight;
                    newHeight = provissionalName(globalVertex.x, globalVertex.y);

                    // Now we transform from global to local
                    Vector3 localPoint = transform.InverseTransformPoint(globalVertex.x, newHeight, globalVertex.z);

                    vertices[i].y = localPoint.y;

                }

                // Asignar los vértices modificados a la malla
                mesh.vertices = vertices;
                meshFilter.sharedMesh = mesh;

                
                mesh.name = "" + transform.TransformPoint(mesh.vertices[0]).x +""+ transform.TransformPoint(mesh.vertices[0]).y + "" + transform.TransformPoint(mesh.vertices[0]).z;

                // Recalcular la malla para actualizar la geometría
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();

                needsDeformation = false;
            }
            else
            {
                mesh.vertices = vertices;
                meshFilter.sharedMesh = mesh;
                mesh.name = "" + transform.TransformPoint(mesh.vertices[0]).y;
            }

            // if (vxComponent == null)
            // {
            //     vxComponent = gameObject.AddComponent<Voxon.VXComponent>();
            //     vxComponent.CanExpire = true;
            //     vxComponent.automaticallyExpire = true;
            //     vxComponent.TimeToExpire = 0.1f;
            //     vxComponent.MeshPath = meshFilter.mesh.name;

            // }
            vxComponent.MeshPath = meshFilter.mesh.name;
            previousPos = transform.position;

        }
        else
        {
            Debug.LogError("No se encontró un MeshFilter en el objeto.");
        }

    }

    private float applyDeformation(float height, float x)
    {

        float jump = 0.01f;
        float threshold = 0.01f;
        float currHeight = 2;

        while(currHeight >= -2)
        {
                //Aplicamos la amplitud de la parabola
            float parabolleAmplitude = getAmplitudeOfParabolle(currHeight);
                //Obtenemos los puntos que definen la parabola
            Vector2 leftPoint = new Vector2(-5, currHeight);
            //Vector2 rightPoint = new Vector2(5, currHeight);
            Vector2 centerPoint = new Vector2(0, parabolleAmplitude);

                //Extraemos las coordenadas de los puntos
            float x1 = leftPoint.x, y1 = leftPoint.y;
            float x2 = centerPoint.x, y2 = centerPoint.y;
            //float x3 = rightPoint.x, y3 = rightPoint.y;

            //Calculamos para la parabola la altura
            //float a = ((y3 - y1) / (x3 - x1) - (y2 - y1) / (x2 - x1)) / (x3 - x2);
            //float b = ((y2 - y1) / (x2 - x1)) - a * (x1 + x2);
            //float c = y1 - a * Mathf.Pow(x1, 2) - b * x1;

            float a = (y1 - y2) / ((x1 - x2)*(x1 - x2));
            float b = -2 * a * x2;
            float c = y2 + a * Mathf.Pow(x2, 2);

            //Calculamos y para el valor de x proporcionado
            float y = a * Mathf.Pow(x, 2) + b * x + c;

                //Comparamos con la altura deseada
            if (Mathf.Abs(height-y) <= threshold)
            {
                // Debug.Log("Original Height: "+height);
                // Debug.Log("After height: "+ currHeight);
                return currHeight;
            }
            else
            {
                currHeight -= jump;
            }

        }

        return 2f;

    }

    float getAmplitudeOfParabolle(float x)
    {

        float a3 = 0.0675f;
        float b3 = -0.0471f;
        float c3 = 1.2196f;
        float d3 = 0.2532f;

        float a6 = -0.0014f;
        float b6 = 0.035f;
        float c6 = -0.032f;
        float d6 = -0.104f;
        float e6 = 0.0995f;
        float f6 = 1.3873f;
        float g6 = 0.1769f;

        // Calculamos y según la fórmula y = ax^3 + bx^2 + cx + d Grado 3
        //float y = a3 * Mathf.Pow(x, 3) + b3 * Mathf.Pow(x, 2) + c3 * x + d3;

        //Grado 6
        float y = a6 * Mathf.Pow(x, 6) + b6 * Mathf.Pow(x, 5) + c6 * Mathf.Pow(x, 4) + d6 * Mathf.Pow(x, 3) + e6 * Mathf.Pow(x, 2) + f6 * x + g6;

        return y;
    }

    float provissionalName(float x, float initial)
    {

        float data_a6 = -0.0014f;
        float data_a5 = 0.035f;
        float data_a4 = -0.032f;
        float data_a3 = -0.104f;
        float data_a2 = 0.0995f;
        float data_a1 = 1.3873f;
        float data_a0 = 0.1769f;

        //Suppose x_1 = -5
        float coefficient = (1 - Mathf.Pow(x/5, 2));
        float a0 = coefficient * data_a0;
        float a1 = Mathf.Pow(x/5, 2) + coefficient * data_a1;
        float a2 = coefficient * data_a2;
        float a3 = coefficient * data_a3;
        float a4 = coefficient * data_a4;
        float a5 = coefficient * data_a5;
        float a6 = coefficient * data_a6;

        //New approximation function.
        //Taylor series' parameters:

        float A1 = 1/a1;
        float A2 = - Mathf.Pow(A1, 3) * a2;
        float A3 = 2 * Mathf.Pow(A2, 2) * a1 - a3 * Mathf.Pow(A1, 4);
        float A4 = Mathf.Pow(A1, 7) * (5 * a1 * a2 * a3 - Mathf.Pow(a1, 2) * a4 - 5 * Mathf.Pow(a2, 3));
        float A5 = Mathf.Pow(A1, 9) * (6 * Mathf.Pow(a1, 2) * a2 * a4 + 3 * Mathf.Pow(a1, 2) * Mathf.Pow(a3, 2) + 14 * Mathf.Pow(a2, 4) - Mathf.Pow(a1, 3) * a5 - 21 * a1 * Mathf.Pow(a2, 2) * a3);
        float A6 = Mathf.Pow(A1, 11) * (7 * Mathf.Pow(a1, 3) * a2 * a5 + 7 * Mathf.Pow(a1, 3) * a3 * a4 + 84 * a1 * Mathf.Pow(a2, 3) * a3 - Mathf.Pow(a1, 4) * a6 - 28 * Mathf.Pow(a1, 2) * a2 * Mathf.Pow(a3, 2) - 42 * Mathf.Pow(a2, 5) - 28 * Mathf.Pow(a1, 2) * Mathf.Pow(a2, 2) * a4);

        //Taylor series' variable:
        float aux_initial = initial - a0;

        //Grado 6
        float final = A1 * aux_initial + A2 * Mathf.Pow(aux_initial, 2) + A3 * Mathf.Pow(aux_initial, 3) + A4 * Mathf.Pow(aux_initial, 4) + A5 * Mathf.Pow(aux_initial, 5) + A6 * Mathf.Pow(aux_initial, 6);

        // Debug.Log(final);

        return final;
    }

}
