using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace Voxon
{
    [Serializable]
    public class MeshPack : ISerializable
    {

        [SerializeField] private MeshData[] data;

        public MeshPack()
        {
        }
	
        public void SetData(MeshData[] inData)
        {
            data = inData;
        }

        public MeshData[] GetData()
        {
            return data;
        }

        // Implement this method to serialize data. The method is called 
        // on serialization.
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Use the AddValue method to specify serialized values.
            info.AddValue("props", data, typeof(RegisteredMesh[]));

        }

        // The special constructor is used to deserialize values.
        public MeshPack (SerializationInfo info, StreamingContext context)
        {
            // Reset the property value using the GetValue method.
            data = (MeshData[])info.GetValue("props", typeof(MeshData[]));
        }
    }
}
