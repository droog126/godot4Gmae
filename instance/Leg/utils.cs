using Godot;
using System;

public class WalkingAnimation
{
    private float stepTimer = 0.0f;
    private float stepTime = 60.0f; // 步态切换时间，单位为秒
    public int stepIndex = 0;
    private Action<int> stepCallback; // 步态切换完成后的回调函数

    public WalkingAnimation(Action<int> callback)
    {
        stepCallback = callback;
    }
    public WalkingAnimation(Action<int> callback, float stepTime)
    {
        stepCallback = callback;
        this.stepTime = stepTime;
    }

    public void Process(double delta)
    {
      
        stepTimer += (float)delta;
        if (stepTimer >= stepTime)
        {
            stepIndex += 1;
            stepTimer -= stepTime;
            stepCallback?.Invoke(stepIndex);
        }
    }

}


public static class MathUtils
{
    public static Vector3 GetPositionInDirection(Vector3 startPoint, Vector3 direction, float distance)
    {
        Vector3 normalizedDirection = direction.Normalized();
        Vector3 displacement = normalizedDirection * distance;
        Vector3 newPosition = startPoint + displacement;
        return newPosition;
    }

}
