using Godot;
using System;
using System.Collections.Generic;

public static class Utilities
{
    public static Node3D Instantiate(this PackedScene prefab, Node parent, Vector3 position)
    {
        Node3D instance = prefab.Instantiate() as Node3D;
        instance.GlobalPosition = position;
        parent.AddChild(instance);

        return instance;
    }

    public static Node2D Instantiate(this PackedScene prefab, Node parent, Vector2 position)
    {
        Node2D instance = prefab.Instantiate() as Node2D;
        instance.GlobalPosition = position;
        parent.AddChild(instance);

        return instance;
    }

    public static Node FindChildOfType<T>(this Node node)
    {
        if (node is T)
        {
            return node;
        }

        foreach (Node child in node.GetChildren())
        {
            Node result = child.FindChildOfType<T>();
            if (result != null)
                return result;
        }

        return null;
    }

    public static List<Node> FindChildrenOfType<T>(this Node node)
    {
        List<Node> result = new List<Node>();
        if (node is T)
            result.Add(node);

        foreach (Node child in node.GetChildren())
        {
            result.AddRange(child.FindChildrenOfType<T>());
        }
        return result;
    }

    public static bool HasChildOfType(this Node node, Type type)
    {
        if (node.GetType() == type)
            return true;

        foreach (Node child in node.GetChildren())
        {
            if (child.HasChildOfType(type))
                return true;
        }

        return false;
    }

    public static Node FindParentOfType<T>(this Node node)
    {
        Node parent = node.GetParent();
        if (parent != null && parent is not T)
            return FindParentOfType<T>(parent);
        return parent;
    }

    public static Node FindSiblingOfType<T>(this Node node)
    {
        foreach (Node child in node.GetParent().GetChildren())
        {
            if (child is T)
                return child;
        }
        return null;
    }

    //Source: https://answers.unity.com/questions/24756/formula-behind-smoothdamp.html
    public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float deltaTime, float maxSpeed = float.MaxValue)
    {
        smoothTime = Mathf.Max(0.0001f, smoothTime);
        float num = 2f / smoothTime;
        float num2 = num * deltaTime;
        float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
        float num4 = current - target;
        float num5 = target;
        float num6 = maxSpeed * smoothTime;
        num4 = Mathf.Clamp(num4, -num6, num6);
        target = current - num4;
        float num7 = (currentVelocity + num * num4) * deltaTime;
        currentVelocity = (currentVelocity - num * num7) * num3;
        float num8 = target + (num4 + num7) * num3;
        if (num5 - current > 0f == num8 > num5)
        {
            num8 = num5;
            currentVelocity = (num8 - num5) / deltaTime;
        }
        return num8;
    }

    public static Vector2 SmoothDamp(Vector2 current, Vector2 target, ref Vector2 velocity, float smoothTime, float deltaTime)
    {
        Vector2 result = new Vector2();
        result.X = SmoothDamp(current.X, target.X, ref velocity.X, smoothTime, deltaTime);
        result.Y = SmoothDamp(current.Y, target.Y, ref velocity.Y, smoothTime, deltaTime);
        return result;
    }
    public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 velocity, float smoothTime, float deltaTime)
    {
        Vector3 result = new Vector3();
        result.X = SmoothDamp(current.X, target.X, ref velocity.X, smoothTime, deltaTime);
        result.Y = SmoothDamp(current.Y, target.Y, ref velocity.Y, smoothTime, deltaTime);
        result.Z = SmoothDamp(current.Z, target.Z, ref velocity.Z, smoothTime, deltaTime);
        return result;
    }
}