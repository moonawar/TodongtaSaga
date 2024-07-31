using UnityEngine;
using System;

[Serializable]
public class TransformData
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public TransformData(Transform transform)
    {
        position = transform.position;
        rotation = transform.rotation;
        scale = transform.localScale;
    }

    public void ApplyTo(Transform transform)
    {
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = scale;
    }
}

[Serializable]
public class RigidbodyData
{
    public Vector3 velocity;
    public Vector3 angularVelocity;
    public float mass;
    public bool useGravity;
    public bool isKinematic;

    public RigidbodyData(Rigidbody rb)
    {
        velocity = rb.velocity;
        angularVelocity = rb.angularVelocity;
        mass = rb.mass;
        useGravity = rb.useGravity;
        isKinematic = rb.isKinematic;
    }

    public void ApplyTo(Rigidbody rb)
    {
        rb.velocity = velocity;
        rb.angularVelocity = angularVelocity;
        rb.mass = mass;
        rb.useGravity = useGravity;
        rb.isKinematic = isKinematic;
    }
}

[Serializable]
public class ColliderData
{
    public bool isTrigger;
    public Vector3 center;

    public ColliderData(Collider collider)
    {
        isTrigger = collider.isTrigger;
        center = collider.bounds.center; // You might need to adapt this to each collider type.
    }

    public void ApplyTo(Collider collider)
    {
        collider.isTrigger = isTrigger;
        // Apply center based on collider type
    }
}

[Serializable]
public class LightData
{
    public Color color;
    public float intensity;
    public float range;
    public LightType type;

    public LightData(Light light)
    {
        color = light.color;
        intensity = light.intensity;
        range = light.range;
        type = light.type;
    }

    public void ApplyTo(Light light)
    {
        light.color = color;
        light.intensity = intensity;
        light.range = range;
        light.type = type;
    }
}

[Serializable]
public class CameraData
{
    public float fieldOfView;
    public Color backgroundColor;
    public bool orthographic;
    public float orthographicSize;

    public CameraData(Camera camera)
    {
        fieldOfView = camera.fieldOfView;
        backgroundColor = camera.backgroundColor;
        orthographic = camera.orthographic;
        orthographicSize = camera.orthographicSize;
    }

    public void ApplyTo(Camera camera)
    {
        camera.fieldOfView = fieldOfView;
        camera.backgroundColor = backgroundColor;
        camera.orthographic = orthographic;
        camera.orthographicSize = orthographicSize;
    }
}

[Serializable]
public class RectTransformData
{
    public Vector3 anchoredPosition;
    public Vector2 sizeDelta;
    public Vector2 pivot;
    public Vector2 anchorMin;
    public Vector2 anchorMax;
    public Vector3 localScale;
    public Quaternion localRotation;

    public RectTransformData(RectTransform rectTransform)
    {
        anchoredPosition = rectTransform.anchoredPosition;
        sizeDelta = rectTransform.sizeDelta;
        pivot = rectTransform.pivot;
        anchorMin = rectTransform.anchorMin;
        anchorMax = rectTransform.anchorMax;
        localScale = rectTransform.localScale;
        localRotation = rectTransform.localRotation;
    }

    public void ApplyTo(RectTransform rectTransform)
    {
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.pivot = pivot;
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.localScale = localScale;
        rectTransform.localRotation = localRotation;
    }
}