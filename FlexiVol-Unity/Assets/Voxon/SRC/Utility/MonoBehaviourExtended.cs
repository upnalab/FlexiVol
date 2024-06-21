using UnityEngine;
// ReSharper disable CheckNamespace


public static class MethodExtensionForMonoBehaviourTransform
{
    /// <summary>
    /// Gets or add a component. Usage example:
    /// BoxCollider boxCollider = transform.GetOrAddComponent&lt;BoxCollider&gt;();
    /// </summary>
    public static T GetOrAddComponent<T>(this Component child) where T : Component
    {
        var result = child.GetComponent<T>();
        if (result == null)
        {
            result = child.gameObject.AddComponent<T>();
        }
        return result;
    }
}