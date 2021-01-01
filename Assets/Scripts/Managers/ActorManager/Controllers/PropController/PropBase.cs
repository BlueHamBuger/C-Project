using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;
using RootMotion;

public class PropBase : MonoBehaviour
{

    #region User Interface
    /// <summary>
    /// 决定 muscle 对应的部位的权重值 和 propHandler 的 muscleIndexes 的index照应
    /// </summary>
    public float[] muscleWeights = new float[]{1.0f,1.0f};

    [Tooltip("This has no other purpose but helping you distinguish props by PropHandler.currentProp.propType.")]
    /// <summary>
    /// This has no other purpose but helping you distinguish PropHandler.currentProp by type.
    /// </summary>
    public int propType;

    // [Tooltip("The muscle properties that will be applied to the Muscle on pickup.")]
    // /// <summary>
    // /// The muscle properties that will be applied to the Muscle.
    // /// </summary>
    // public Muscle.Props muscleProps = new Muscle.Props();

    [Tooltip("If true, this prop's layer will be forced to PuppetMaster layer and target's layer forced to PuppetMaster's Target Root's layer when the prop is picked up.")]
    /// <summary>
    /// If true, this prop's layer will be forced to PuppetMaster layer and target's layer forced to PuppetMaster's Target Root's layer when the prop is picked up.
    /// </summary>
    public bool forceLayers = true;


    [Tooltip("The pin weight of the additional pin. Increasing this weight will make the prop follow animation better, but will increase jitter when colliding with objects.")]
    /// <summary>
    /// The pin weight of the additional pin. Increasing this weight will make the prop follow animation better, but will increase jitter when colliding with objects.
    /// </summary>
    [Range(0f, 1f)] public float additionalPinWeight = 1f;

    /// <summary>
    /// Is this prop picked up and connected to a PropHandler?
    /// </summary>
    public bool isPickedUp { get { return propHandler != null; } }

    /// <summary>
    /// Returns the PropHandler that this prop is connected to if it is picked up. If this returns null, the prop is not picked up.
    /// </summary>
    public PropHandler propHandler { get; private set; }
    public Rigidbody r{
        get;
        private set;
    }

    public Transform mesh;
    public Collider handleCollider;
    public Collider edgeCollider;
    public Transform additionalPinTarget;
    public float mass = 0.5f;
    public float addPartMass = 1.0f;
    private Vector3 defaultMeshLocalPos;
    private Vector3 defaultMeshLocalEuler;
    private Vector3 defaultColLocalPos;
    private Vector3 defaultColLocalEuler;
    private Vector3 defaultCol1LocalPos;
    private Vector3 defaultCol1LocalEuler;
    private Vector3 defaultAddLocalEuler;
    private Vector3 defaultAddLocalPos;
    


    // material 相关内容
    public Color emissionColor;
    MeshRenderer meshRenderer;
    float emissionIntensity;
    public float EmissionIntensity{
        get{
            return emissionIntensity;
        }set{
            emissionIntensity = value;
            meshRenderer.material.SetColor("_EmissionColor",emissionColor*value);
        }
    }

    // 代表 物体是否被插入到了场景中
        //默认将所有的物体设置为 无法插入 即false
    public virtual bool IsStabIn{
        get{return false;}
    }		

		




    #endregion User Interface

    // Picking up/dropping props is done by simply changing PropHandler.currentProp
    public void PickUp(PropHandler propHandler)
    {
        // 设置 rigidbody 为不动
        r.isKinematic = true;
        transform.position = propHandler.muscles[0].target.position;
        transform.rotation = propHandler.muscles[0].target.rotation;
        handleCollider.transform.parent = propHandler.muscles[0].transform;
        mesh.transform.parent = propHandler.muscles[0].target;
        if(additionalPinTarget){
            additionalPinTarget.localPosition = defaultAddLocalPos;
            additionalPinTarget.localEulerAngles = defaultAddLocalPos;
            propHandler.muscles[1].transform.position = additionalPinTarget.position;
            propHandler.muscles[1].transform.rotation = additionalPinTarget.rotation;
            propHandler.muscles[1].target.position = additionalPinTarget.position; 
            propHandler.muscles[1].target.rotation = additionalPinTarget.rotation;
            edgeCollider.transform.parent = propHandler.muscles[1].transform;
            propHandler.muscles[1].rigidbody.mass = addPartMass;
        }
        propHandler.muscles[0].rigidbody.mass = mass;
        this.propHandler = propHandler;

        //TODO Layer的确认和修改

        OnPickUp(propHandler);
    }

    // Picking up/dropping props is done by simply changing PropHandler.currentProp
    public void Drop()
    {
        transform.position = propHandler.muscles[0].target.position;
        transform.rotation = propHandler.muscles[0].target.rotation;
        mesh.parent = transform;
        handleCollider.transform.parent = transform;
        edgeCollider.transform.parent = additionalPinTarget;

        handleCollider.transform.localPosition = defaultColLocalPos;
        handleCollider.transform.localEulerAngles = defaultColLocalEuler;
        edgeCollider.transform.localPosition = defaultCol1LocalPos;
        edgeCollider.transform.localEulerAngles = defaultCol1LocalEuler;

        mesh.transform.localPosition = defaultMeshLocalPos;
        mesh.transform.localEulerAngles = defaultMeshLocalEuler;

        
        r.isKinematic = false;
        r.velocity = propHandler.muscles[0].rigidbody.velocity;
        if(additionalPinTarget){
            Rigidbody r1 = additionalPinTarget.GetComponent<Rigidbody>();
            r1.isKinematic = false; //r1.velocity = propHandler.muscles[1].rigidbody.velocity/2;
        }

        this.propHandler = null;

        OnDrop();
    }

    public void StartPickedUp(PropHandler propHandler)
    {
        this.propHandler = propHandler;
    }

    protected virtual void OnPickUp(PropHandler propHandler) { }
    protected virtual void OnDrop() { }
    protected virtual void OnStart() { }

    private ConfigurableJointMotion xMotion, yMotion, zMotion, angularXMotion, angularYMotion, angularZMotion;
    private Collider[] colliders = new Collider[0];

    void Start()
    {
        defaultMeshLocalPos = mesh.localPosition;
        defaultMeshLocalEuler = mesh.localEulerAngles;
        defaultColLocalPos = handleCollider.transform.localPosition;
        defaultColLocalEuler = handleCollider.transform.localEulerAngles;
        defaultCol1LocalPos = edgeCollider.transform.localPosition;
        defaultCol1LocalEuler = edgeCollider.transform.localEulerAngles;
        defaultAddLocalPos = additionalPinTarget.transform.localPosition;
        defaultAddLocalEuler = additionalPinTarget.transform.localEulerAngles;
        r = GetComponent<Rigidbody>();
        // r.mass = mass+ addPartMass;

        meshRenderer = mesh.GetComponent<MeshRenderer>();
        //默认下 强度应该为 -10 即代表 0.001;
        emissionColor = meshRenderer.material.GetColor("_EmissionColor") * 1000;
        emissionIntensity = 0f;
        OnStart();
    }



}
