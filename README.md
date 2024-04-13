# Addressables ComponentReference with DerivedClass

## What is AssetReference?
Addressables <b>AssetReference</b> is a special reference to any asset in you project. You can control device memory and build dependencies by using AssetReference instead of native asset type (Sprite, GameObject etc.). See more at [doc](https://docs.unity3d.com/Packages/com.unity.addressables@1.20/manual/AssetReferences.html).
### What is AssetReferenceT?
![asset-types](https://github.com/cyberaslan/Addressables-ComponentReference-with-DerivedClass/assets/87382541/ca64d70b-7d6e-424c-b0be-43408e3da1f2)
There is generic <b>AssetReferenceT\<TComponent\></b> designed to make strictly typed asset variables. Your assets workflow become the same as Unity classic workflow this way.
<br><br><b>How it works with GameObjects?</b><br>
We can store only AssetReferenceT\<GameObject\> not to attached MonoBehaviour component. But classic Unity assets workflow supports reference to MonoBehaviour, so? Use ComponentReference\<T\>.

## What is ComponentReference\<T\>?
ComponentReference is an Addressables extension to store AssetReference via Component attached to GameObject asset. Yes, like a classic Unity prefab. The [ComponentReference\<T\>](https://docs.unity3d.com/Packages/com.unity.addressables@1.20/manual/AssetReferences.html) is a part of Basic Addressables code example doc.

### Component derived class problem
Imagine you have some service creates Overlay from prefab list by generic type parameter. 
![type-check](https://github.com/cyberaslan/Addressables-ComponentReference-with-DerivedClass/assets/87382541/39003127-3c08-4d51-81a1-67153a1e7386)
Concrete overlay class inherits BaseOverlay but <b>ComponentReference\<ConcreteOverlay\></b> is always not <b>ComponentReference\<BaseOverlay\></b> cause generic-types specific.
