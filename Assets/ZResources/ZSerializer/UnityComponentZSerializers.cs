
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
namespace ZSerializer {

[System.Serializable]
public sealed class NavMeshAgentZSerializer : ZSerializer.Internal.ZSerializer {
    public UnityEngine.Vector3 destination;
    public System.Single stoppingDistance;
    public UnityEngine.Vector3 velocity;
    public UnityEngine.Vector3 nextPosition;
    public System.Single baseOffset;
    public System.Boolean autoTraverseOffMeshLink;
    public System.Boolean autoBraking;
    public System.Boolean autoRepath;
    public System.Boolean isStopped;
    public System.Int32 agentTypeID;
    public System.Int32 areaMask;
    public System.Single speed;
    public System.Single angularSpeed;
    public System.Single acceleration;
    public System.Boolean updatePosition;
    public System.Boolean updateRotation;
    public System.Boolean updateUpAxis;
    public System.Single radius;
    public System.Single height;
    public UnityEngine.AI.ObstacleAvoidanceType obstacleAvoidanceType;
    public System.Int32 avoidancePriority;
    public System.Boolean enabled;
    public UnityEngine.HideFlags hideFlags;
    public NavMeshAgentZSerializer (string ZUID, string GOZUID) : base(ZUID, GOZUID) {
        var instance = ZSerializer.ZSerialize.idMap[ZSerializer.ZSerialize.CurrentGroupID][ZUID] as UnityEngine.AI.NavMeshAgent;
        destination = instance.destination;
        stoppingDistance = instance.stoppingDistance;
        velocity = instance.velocity;
        nextPosition = instance.nextPosition;
        baseOffset = instance.baseOffset;
        autoTraverseOffMeshLink = instance.autoTraverseOffMeshLink;
        autoBraking = instance.autoBraking;
        autoRepath = instance.autoRepath;
        isStopped = instance.isStopped;
        agentTypeID = instance.agentTypeID;
        areaMask = instance.areaMask;
        speed = instance.speed;
        angularSpeed = instance.angularSpeed;
        acceleration = instance.acceleration;
        updatePosition = instance.updatePosition;
        updateRotation = instance.updateRotation;
        updateUpAxis = instance.updateUpAxis;
        radius = instance.radius;
        height = instance.height;
        obstacleAvoidanceType = instance.obstacleAvoidanceType;
        avoidancePriority = instance.avoidancePriority;
        enabled = instance.enabled;
        hideFlags = instance.hideFlags;
        ZSerializerSettings.Instance.unityComponentDataList.FirstOrDefault(data => data.Type == typeof(UnityEngine.AI.NavMeshAgent))?.OnSerialize?.Invoke(this, instance);
    }
    public override void RestoreValues(UnityEngine.Component component)
    {
        var instance = (UnityEngine.AI.NavMeshAgent)component;
        instance.destination = destination;
        instance.stoppingDistance = stoppingDistance;
        instance.velocity = velocity;
        instance.nextPosition = nextPosition;
        instance.baseOffset = baseOffset;
        instance.autoTraverseOffMeshLink = autoTraverseOffMeshLink;
        instance.autoBraking = autoBraking;
        instance.autoRepath = autoRepath;
        instance.isStopped = isStopped;
        instance.agentTypeID = agentTypeID;
        instance.areaMask = areaMask;
        instance.speed = speed;
        instance.angularSpeed = angularSpeed;
        instance.acceleration = acceleration;
        instance.updatePosition = updatePosition;
        instance.updateRotation = updateRotation;
        instance.updateUpAxis = updateUpAxis;
        instance.radius = radius;
        instance.height = height;
        instance.obstacleAvoidanceType = obstacleAvoidanceType;
        instance.avoidancePriority = avoidancePriority;
        instance.enabled = enabled;
        instance.hideFlags = hideFlags;
        ZSerializerSettings.Instance.unityComponentDataList.FirstOrDefault(data => data.Type == typeof(UnityEngine.AI.NavMeshAgent))?.OnDeserialize?.Invoke(this, instance);
    }
}
[System.Serializable]
public sealed class TransformZSerializer : ZSerializer.Internal.ZSerializer {
    public UnityEngine.Vector3 position;
    public UnityEngine.Vector3 localPosition;
    public UnityEngine.Vector3 eulerAngles;
    public UnityEngine.Vector3 localEulerAngles;
    public UnityEngine.Vector3 right;
    public UnityEngine.Vector3 up;
    public UnityEngine.Vector3 forward;
    public UnityEngine.Quaternion rotation;
    public UnityEngine.Quaternion localRotation;
    public UnityEngine.Vector3 localScale;
    public UnityEngine.Transform parent;
    public System.Boolean hasChanged;
    public System.Int32 hierarchyCapacity;
    public UnityEngine.HideFlags hideFlags;
    public TransformZSerializer (string ZUID, string GOZUID) : base(ZUID, GOZUID) {
        var instance = ZSerializer.ZSerialize.idMap[ZSerializer.ZSerialize.CurrentGroupID][ZUID] as UnityEngine.Transform;
        position = instance.position;
        localPosition = instance.localPosition;
        eulerAngles = instance.eulerAngles;
        localEulerAngles = instance.localEulerAngles;
        right = instance.right;
        up = instance.up;
        forward = instance.forward;
        rotation = instance.rotation;
        localRotation = instance.localRotation;
        localScale = instance.localScale;
        parent = instance.parent;
        hasChanged = instance.hasChanged;
        hierarchyCapacity = instance.hierarchyCapacity;
        hideFlags = instance.hideFlags;
        ZSerializerSettings.Instance.unityComponentDataList.FirstOrDefault(data => data.Type == typeof(UnityEngine.Transform))?.OnSerialize?.Invoke(this, instance);
    }
    public override void RestoreValues(UnityEngine.Component component)
    {
        var instance = (UnityEngine.Transform)component;
        instance.position = position;
        instance.localPosition = localPosition;
        instance.eulerAngles = eulerAngles;
        instance.localEulerAngles = localEulerAngles;
        instance.right = right;
        instance.up = up;
        instance.forward = forward;
        instance.rotation = rotation;
        instance.localRotation = localRotation;
        instance.localScale = localScale;
        instance.parent = parent;
        instance.hasChanged = hasChanged;
        instance.hierarchyCapacity = hierarchyCapacity;
        instance.hideFlags = hideFlags;
        ZSerializerSettings.Instance.unityComponentDataList.FirstOrDefault(data => data.Type == typeof(UnityEngine.Transform))?.OnDeserialize?.Invoke(this, instance);
    }
}
[System.Serializable]
public sealed class RigidbodyZSerializer : ZSerializer.Internal.ZSerializer {
    public UnityEngine.Vector3 velocity;
    public UnityEngine.Vector3 angularVelocity;
    public System.Single drag;
    public System.Single angularDrag;
    public System.Single mass;
    public System.Boolean useGravity;
    public System.Single maxDepenetrationVelocity;
    public System.Boolean isKinematic;
    public System.Boolean freezeRotation;
    public UnityEngine.RigidbodyConstraints constraints;
    public UnityEngine.CollisionDetectionMode collisionDetectionMode;
    public UnityEngine.Vector3 centerOfMass;
    public UnityEngine.Quaternion inertiaTensorRotation;
    public UnityEngine.Vector3 inertiaTensor;
    public System.Boolean detectCollisions;
    public UnityEngine.Vector3 position;
    public UnityEngine.Quaternion rotation;
    public UnityEngine.RigidbodyInterpolation interpolation;
    public System.Int32 solverIterations;
    public System.Single sleepThreshold;
    public System.Single maxAngularVelocity;
    public System.Int32 solverVelocityIterations;
    public UnityEngine.HideFlags hideFlags;
    public RigidbodyZSerializer (string ZUID, string GOZUID) : base(ZUID, GOZUID) {
        var instance = ZSerializer.ZSerialize.idMap[ZSerializer.ZSerialize.CurrentGroupID][ZUID] as UnityEngine.Rigidbody;
        velocity = instance.velocity;
        angularVelocity = instance.angularVelocity;
        drag = instance.drag;
        angularDrag = instance.angularDrag;
        mass = instance.mass;
        useGravity = instance.useGravity;
        maxDepenetrationVelocity = instance.maxDepenetrationVelocity;
        isKinematic = instance.isKinematic;
        freezeRotation = instance.freezeRotation;
        constraints = instance.constraints;
        collisionDetectionMode = instance.collisionDetectionMode;
        centerOfMass = instance.centerOfMass;
        inertiaTensorRotation = instance.inertiaTensorRotation;
        inertiaTensor = instance.inertiaTensor;
        detectCollisions = instance.detectCollisions;
        position = instance.position;
        rotation = instance.rotation;
        interpolation = instance.interpolation;
        solverIterations = instance.solverIterations;
        sleepThreshold = instance.sleepThreshold;
        maxAngularVelocity = instance.maxAngularVelocity;
        solverVelocityIterations = instance.solverVelocityIterations;
        hideFlags = instance.hideFlags;
        ZSerializerSettings.Instance.unityComponentDataList.FirstOrDefault(data => data.Type == typeof(UnityEngine.Rigidbody))?.OnSerialize?.Invoke(this, instance);
    }
    public override void RestoreValues(UnityEngine.Component component)
    {
        var instance = (UnityEngine.Rigidbody)component;
        instance.velocity = velocity;
        instance.angularVelocity = angularVelocity;
        instance.drag = drag;
        instance.angularDrag = angularDrag;
        instance.mass = mass;
        instance.useGravity = useGravity;
        instance.maxDepenetrationVelocity = maxDepenetrationVelocity;
        instance.isKinematic = isKinematic;
        instance.freezeRotation = freezeRotation;
        instance.constraints = constraints;
        instance.collisionDetectionMode = collisionDetectionMode;
        instance.centerOfMass = centerOfMass;
        instance.inertiaTensorRotation = inertiaTensorRotation;
        instance.inertiaTensor = inertiaTensor;
        instance.detectCollisions = detectCollisions;
        instance.position = position;
        instance.rotation = rotation;
        instance.interpolation = interpolation;
        instance.solverIterations = solverIterations;
        instance.sleepThreshold = sleepThreshold;
        instance.maxAngularVelocity = maxAngularVelocity;
        instance.solverVelocityIterations = solverVelocityIterations;
        instance.hideFlags = hideFlags;
        ZSerializerSettings.Instance.unityComponentDataList.FirstOrDefault(data => data.Type == typeof(UnityEngine.Rigidbody))?.OnDeserialize?.Invoke(this, instance);
    }
}
[System.Serializable]
public sealed class CharacterControllerZSerializer : ZSerializer.Internal.ZSerializer {
    public System.Single radius;
    public System.Single height;
    public UnityEngine.Vector3 center;
    public System.Single slopeLimit;
    public System.Single stepOffset;
    public System.Single skinWidth;
    public System.Single minMoveDistance;
    public System.Boolean detectCollisions;
    public System.Boolean enableOverlapRecovery;
    public System.Boolean enabled;
    public System.Boolean isTrigger;
    public System.Single contactOffset;
    public System.Boolean hasModifiableContacts;
    public UnityEngine.HideFlags hideFlags;
    public CharacterControllerZSerializer (string ZUID, string GOZUID) : base(ZUID, GOZUID) {
        var instance = ZSerializer.ZSerialize.idMap[ZSerializer.ZSerialize.CurrentGroupID][ZUID] as UnityEngine.CharacterController;
        radius = instance.radius;
        height = instance.height;
        center = instance.center;
        slopeLimit = instance.slopeLimit;
        stepOffset = instance.stepOffset;
        skinWidth = instance.skinWidth;
        minMoveDistance = instance.minMoveDistance;
        detectCollisions = instance.detectCollisions;
        enableOverlapRecovery = instance.enableOverlapRecovery;
        enabled = instance.enabled;
        isTrigger = instance.isTrigger;
        contactOffset = instance.contactOffset;
        hasModifiableContacts = instance.hasModifiableContacts;
        hideFlags = instance.hideFlags;
        ZSerializerSettings.Instance.unityComponentDataList.FirstOrDefault(data => data.Type == typeof(UnityEngine.CharacterController))?.OnSerialize?.Invoke(this, instance);
    }
    public override void RestoreValues(UnityEngine.Component component)
    {
        var instance = (UnityEngine.CharacterController)component;
        instance.radius = radius;
        instance.height = height;
        instance.center = center;
        instance.slopeLimit = slopeLimit;
        instance.stepOffset = stepOffset;
        instance.skinWidth = skinWidth;
        instance.minMoveDistance = minMoveDistance;
        instance.detectCollisions = detectCollisions;
        instance.enableOverlapRecovery = enableOverlapRecovery;
        instance.enabled = enabled;
        instance.isTrigger = isTrigger;
        instance.contactOffset = contactOffset;
        instance.hasModifiableContacts = hasModifiableContacts;
        instance.hideFlags = hideFlags;
        ZSerializerSettings.Instance.unityComponentDataList.FirstOrDefault(data => data.Type == typeof(UnityEngine.CharacterController))?.OnDeserialize?.Invoke(this, instance);
    }
}
[System.Serializable]
public sealed class CapsuleColliderZSerializer : ZSerializer.Internal.ZSerializer {
    public UnityEngine.Vector3 center;
    public System.Single radius;
    public System.Single height;
    public System.Int32 direction;
    public System.Boolean enabled;
    public System.Boolean isTrigger;
    public System.Single contactOffset;
    public System.Boolean hasModifiableContacts;
    public UnityEngine.HideFlags hideFlags;
    public CapsuleColliderZSerializer (string ZUID, string GOZUID) : base(ZUID, GOZUID) {
        var instance = ZSerializer.ZSerialize.idMap[ZSerializer.ZSerialize.CurrentGroupID][ZUID] as UnityEngine.CapsuleCollider;
        center = instance.center;
        radius = instance.radius;
        height = instance.height;
        direction = instance.direction;
        enabled = instance.enabled;
        isTrigger = instance.isTrigger;
        contactOffset = instance.contactOffset;
        hasModifiableContacts = instance.hasModifiableContacts;
        hideFlags = instance.hideFlags;
        ZSerializerSettings.Instance.unityComponentDataList.FirstOrDefault(data => data.Type == typeof(UnityEngine.CapsuleCollider))?.OnSerialize?.Invoke(this, instance);
    }
    public override void RestoreValues(UnityEngine.Component component)
    {
        var instance = (UnityEngine.CapsuleCollider)component;
        instance.center = center;
        instance.radius = radius;
        instance.height = height;
        instance.direction = direction;
        instance.enabled = enabled;
        instance.isTrigger = isTrigger;
        instance.contactOffset = contactOffset;
        instance.hasModifiableContacts = hasModifiableContacts;
        instance.hideFlags = hideFlags;
        ZSerializerSettings.Instance.unityComponentDataList.FirstOrDefault(data => data.Type == typeof(UnityEngine.CapsuleCollider))?.OnDeserialize?.Invoke(this, instance);
    }
}
[System.Serializable]
public sealed class HingeJoint2DZSerializer : ZSerializer.Internal.ZSerializer {
    public System.Boolean useMotor;
    public System.Boolean useLimits;
    public UnityEngine.JointMotor2D motor;
    public UnityEngine.JointAngleLimits2D limits;
    public UnityEngine.Vector2 anchor;
    public UnityEngine.Vector2 connectedAnchor;
    public System.Boolean autoConfigureConnectedAnchor;
    public UnityEngine.Rigidbody2D connectedBody;
    public System.Boolean enableCollision;
    public System.Single breakForce;
    public System.Single breakTorque;
    public System.Boolean enabled;
    public UnityEngine.HideFlags hideFlags;
    public Vector2 serializableLimits;
    public Vector2 serializableMotor;
    public HingeJoint2DZSerializer (string ZUID, string GOZUID) : base(ZUID, GOZUID) {
        var instance = ZSerializer.ZSerialize.idMap[ZSerializer.ZSerialize.CurrentGroupID][ZUID] as UnityEngine.HingeJoint2D;
        useMotor = instance.useMotor;
        useLimits = instance.useLimits;
        motor = instance.motor;
        limits = instance.limits;
        anchor = instance.anchor;
        connectedAnchor = instance.connectedAnchor;
        autoConfigureConnectedAnchor = instance.autoConfigureConnectedAnchor;
        connectedBody = instance.connectedBody;
        enableCollision = instance.enableCollision;
        breakForce = instance.breakForce;
        breakTorque = instance.breakTorque;
        enabled = instance.enabled;
        hideFlags = instance.hideFlags;
        ZSerializerSettings.Instance.unityComponentDataList.FirstOrDefault(data => data.Type == typeof(UnityEngine.HingeJoint2D))?.OnSerialize?.Invoke(this, instance);
    }
    public override void RestoreValues(UnityEngine.Component component)
    {
        var instance = (UnityEngine.HingeJoint2D)component;
        instance.useMotor = useMotor;
        instance.useLimits = useLimits;
        instance.motor = motor;
        instance.limits = limits;
        instance.anchor = anchor;
        instance.connectedAnchor = connectedAnchor;
        instance.autoConfigureConnectedAnchor = autoConfigureConnectedAnchor;
        instance.connectedBody = connectedBody;
        instance.enableCollision = enableCollision;
        instance.breakForce = breakForce;
        instance.breakTorque = breakTorque;
        instance.enabled = enabled;
        instance.hideFlags = hideFlags;
        ZSerializerSettings.Instance.unityComponentDataList.FirstOrDefault(data => data.Type == typeof(UnityEngine.HingeJoint2D))?.OnDeserialize?.Invoke(this, instance);
    }
}
}