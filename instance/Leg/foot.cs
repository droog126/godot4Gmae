using Godot;
using System;

public class FootMovement
{
    private Vector3[] footPrev;
    private Vector3[] footTarget;
    private float[] footT;

    private Vector3[] foot;
    private Vector3[] hand;

    public FootMovement()
    {
        // 初始化数组等操作
    }

    private Vector3 side = new Vector3(1,-1,0);
    private float footReach = 5f;
    private float feetWidth = 10f;


    public void Initialize(Vector3 position)
    {
        //float handWidth = radius * 1.5f;

        foot = new Vector3[2];
        hand = new Vector3[2];
        footPrev = new Vector3[2];
        footTarget = new Vector3[2];

        footT = new float[2];

        for (int i = 0; i < 2; i++)
        {
            float sideFactor = i * 2 - 1;

            foot[i] = position + side * feetWidth * sideFactor;
            footPrev[i] = Vector3.Zero;
            footTarget[i] = foot[i];
            footT[i] = 1;

            //hand[i] = position + side * handWidth * sideFactor;

        }
      
    }

    // 计算我的下一步位置
    public void StepFoot(int index, float x, float z, Vector3 forward)
    {
        int i = index;
        float sideFactor = i * 2 - 1;


        // clone
        footPrev[i] = foot[i];

        var xTarget = x + side.X * feetWidth * sideFactor + forward.X * footReach;
        var yTarget = z + side.Z * feetWidth * sideFactor + forward.Z * footReach;

        footTarget[i].X = xTarget;
        footTarget[i].Y = yTarget;

        footT[i] = 0;
    }


    // 完成什么工作
    //public Vector3 GetFootTarget(int index)
    //{

    //}
}
