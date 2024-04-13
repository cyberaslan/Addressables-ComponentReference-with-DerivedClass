using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CyberAslan
{
    /// <summary>
/// Creates an AssetReference that is restricted to having a specific Component.
/// * This is the class that inherits from AssetReference.  It is generic and does not specify which Components it might care about.  A concrete child of this class is required for serialization to work.
/// * At edit-time it validates that the asset set on it is a GameObject with the required Component.
/// * At runtime it can load/instantiate the GameObject, then return the desired component.  API matches base class (LoadAssetAsync &amp; InstantiateAsync).
/// </summary>
/// <typeparam name="TComponent">The component type.</typeparam>

[Serializable]
public class ComponentReference<TComponent> : AssetReference
{
    /// <inheritdoc />
    public ComponentReference(string guid) : base(guid)
    {
    }

    /// <inheritdoc />
    public new AsyncOperationHandle<TComponent> InstantiateAsync(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        return Addressables.ResourceManager.CreateChainOperation<TComponent, GameObject>(base.InstantiateAsync(position, Quaternion.identity, parent), GameObjectReady);
    }

    /// <inheritdoc />
    public new AsyncOperationHandle<TComponent> InstantiateAsync(Transform parent = null, bool instantiateInWorldSpace = false)
    {
        return Addressables.ResourceManager.CreateChainOperation<TComponent, GameObject>(base.InstantiateAsync(parent, instantiateInWorldSpace), GameObjectReady);
    }

    /// <inheritdoc />
    public AsyncOperationHandle<TComponent> LoadAssetAsync()
    {
        return Addressables.ResourceManager.CreateChainOperation<TComponent, GameObject>(base.LoadAssetAsync<GameObject>(), GameObjectReady);
    }

    /// <inheritdoc />
    AsyncOperationHandle<TComponent> GameObjectReady(AsyncOperationHandle<GameObject> arg)
    {
        var comp = arg.Result.GetComponent<TComponent>();
        return Addressables.ResourceManager.CreateCompletedOperation<TComponent>(comp, string.Empty);
    }

    /// <summary>
    /// Validates that the assigned asset has the component type
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool ValidateAsset(Object obj)
    {
        var go = obj as GameObject;
        return go != null && go.GetComponent<TComponent>() != null;
    }

    private IComponentReference _derivedReference;
    public static IComponentReference GetDerivedReference<XComponent>(ComponentReference<XComponent> componentReference) where XComponent : IComponentReference, TComponent
    {
        if (componentReference._derivedReference != null)
        {
            return componentReference._derivedReference;
        }
        
        if (componentReference.CachedAsset == null)
        {
            throw new NullReferenceException("IComponentReference Asset is null");
        }

        if (componentReference.CachedAsset is not GameObject go)
        {
            throw new InvalidCastException("IComponentReference must realise Component on GameObject");
        }

        componentReference._derivedReference = go.GetComponent<IComponentReference>();
        return componentReference._derivedReference;
    }
    
    public static void ValidateInheritorsList<T>(List<ComponentReference<T>> prefabs) where T : IComponentReference
    {
        var overlayBaseType = typeof(T);
        var overlayTypes = Assembly.GetAssembly(overlayBaseType)
            .GetTypes()
            .Where(type => type.IsSubclassOf(overlayBaseType));

        foreach (var overlayType in overlayTypes)
        {
            if (prefabs.All(p => ComponentReference<T>.GetDerivedReference(p).DerivedType != overlayType))
            {
                throw new MissingReferenceException($"Reference to assembly type {overlayType} is missing in list");    
            }
        }
    }

    /// <summary>
    /// Validates that the assigned asset has the component type, but only in the Editor
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public override bool ValidateAsset(string path)
    {
#if UNITY_EDITOR
        //this load can be expensive...
        var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        return go != null && go.GetComponent<TComponent>() != null;
#else
            return false;
#endif
    }

    /// <inheritdoc />
    public void ReleaseInstance(AsyncOperationHandle<TComponent> op)
    {
        // Release the instance
        var component = op.Result as Component;
        if (component != null)
        {
            Addressables.ReleaseInstance(component.gameObject);
        }

        // Release the handle
        Addressables.Release(op);
    }
}

}
