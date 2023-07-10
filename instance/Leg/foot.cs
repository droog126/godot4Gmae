using Godot;
using System;

public class FootMovement
{
    private Vector3[] footPrev;
    private Vector3[] footTarget;
    private float[] footT;

    public Vector3[] foot;
    private Vector3[] hand;

    public FootMovement()
    {
        // 初始化数组等操作
    }

    private Vector3[] side = { new Vector3(-1, 0, 0), new Vector3(1, 0, 0) };

    private float peopleHeight = 5.5f;
    private float foottHeight = 4f;
    private float footReach = 2f;


    public void Initialize(Vector3 pos)
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

            foot[i] = MathUtils.GetPositionInDirection(pos, side[i], footReach);
            footPrev[i] = Vector3.Zero;
            footTarget[i] = foot[i];
            footT[i] = 1;

            //hand[i] = position + side * handWidth * sideFactor;

        }
      
    }

    // 计算我的下一步位置
    public void StepFoot(int index, Vector3 pos, Vector3 forward)
    {
        int i = index;
        float sideFactor = i * 2 - 1;


        // clone
        footPrev[i].X = foot[i].X;
        footPrev[i].Z = foot[i].Z;


        var realForward =  forward.Rotated(Vector3.Up, sideFactor * 30f);


        var newPos = MathUtils.GetPositionInDirection(pos, realForward, footReach);
        footTarget[i] = newPos;
     
        footT[i] = 0;

    }


    public void step()
    {
        for (var i = 0; i < 2; i++)
        {
            foot[i].X = Mathf.Lerp(footPrev[i].X, footTarget[i].X, footT[i]);
            foot[i].Y = Mathf.Lerp(footPrev[i].Y, footTarget[i].Y, footT[i]);
            foot[i].Z = Mathf.Lerp(footPrev[i].Z, footTarget[i].Z, footT[i]);


          

            if (footT[i] < 1)
                footT[i] += 0.1f;
        }

    }

    // 完成什么工作
    //public Vector3 GetFootTarget(int index)
    //{

    //}
}
