using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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

    private float peopleHeight = 0.8f;
    private float foottHeight = 0.6f;
    private float footReach = 0.4f;
    private float footGap = 0.2f;



  

    // 计算左右脚索引
    private int footIndex = 1;




    private RayCast3D[] rayCast3Ds = new RayCast3D[2];

    public void Initialize(Vector3 pos, RayCast3D[] _rayCast3Ds)
    {

        foot = new Vector3[2];
        hand = new Vector3[2];
        footPrev = new Vector3[2];
        footTarget = new Vector3[2];

        footT = new float[2];


        for (int i = 0; i < 2; i++)
        {
    
            foot[i] = MathUtils.GetPositionInDirection(pos, side[i], footReach);
            footPrev[i] = Vector3.Zero;
            footTarget[i] = foot[i];
            footT[i] = 1;


        }

        rayCast3Ds = _rayCast3Ds;


    }



    public void StepFoot2(Transform3D tranfrom)
    {
        int i = footIndex;
        float sideFactor = i * 2 - 1;



        var len = MathUtils.Distance2D(tranfrom.Origin, footTarget[0] / 2 + footTarget[1] / 2);
        if (len <= footGap || len>=footGap*4)
        {
            return;
        }
      
        footIndex += 1;
        footIndex %= 2;
     

        // clone
        footPrev[i].X = foot[i].X;
        footPrev[i].Y = foot[i].Y;
        footPrev[i].Z = foot[i].Z;


        var shouldPos = tranfrom * (MathUtils.AngleToVector(sideFactor * 20f) * footReach);

        footTarget[i].X = shouldPos.X;
        footTarget[i].Z = shouldPos.Z;


        footT[i] = 0;





        var getPos = (float angle) =>
        {
            var Rad = Mathf.DegToRad(angle - 90);
            var dir = new Vector3(Mathf.Cos(Rad), 0, Mathf.Sin(Rad));
            return dir;
        };
        var newPos2 = getPos(30f * sideFactor) * footReach;

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






            //Debugger.Break();
        }


    }


    public void stepPhysics()
    {
        for(var i  = 0; i < 2; i++)
        {

            rayCast3Ds[i].GlobalPosition = footTarget[i] + Vector3.Up * 0.3f;

            var pos = rayCast3Ds[i].GetCollisionPoint();

            var collisoner = rayCast3Ds[i].GetCollider();

            if (rayCast3Ds[i].GetCollider() != null)
            {
                footTarget[i].Y = pos.Y + 0.3f;
            }
            else
            {
                //footTarget[i].Y -= 0.1f;
            }
            GD.Print(footTarget[i], pos, collisoner);
        }
    }



  
}
