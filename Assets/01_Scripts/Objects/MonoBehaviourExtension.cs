using NUnit.Framework.Constraints;
using UnityEngine;

public class MonoBehaviourExtension : MonoBehaviour
{
    protected int nMonoObjectID;
    protected string sMonoObjectName;
    protected Transform cachedTransform;
    
    
    public int MonoObjectID => nMonoObjectID;
    public string MonoObjectName => sMonoObjectName;
    public Transform CachedTransform => cachedTransform;

    protected virtual void Awake()
    {
        cachedTransform = transform;
        nMonoObjectID = GetInstanceID();
        sMonoObjectName = gameObject.name;
    }

    protected virtual void Start()
    {
        
    }
    
    protected virtual void OnEnable()
    {
        
    }

    protected virtual void OnDisable()
    {
        
    }

    protected virtual void OnDestroy()
    {

    }
}
