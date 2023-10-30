using Godot;
using System;

public class Test1
{
    static public void run()
    {
        var forword = new Vector3(0, 0, -1);
        var newVector = forword.Rotated(Vector3.Up, Mathf.DegToRad(30f));
        // GD.Print($"{newVector}");


        //Vector3 up = Vector3.Up; // 假设使用世界坐标系的上方向作为参考
        //Basis rotationBasis = Basis.LookingAt(Vector3.Zero, forward, up);

        //left = rotationBasis.xform(Vector3.Left);
        //right = rotationBasis.xform(Vector3.Right);

        var dir = MathUtils.AngleToVector(-90f);

        var playerBaiss = MathUtils.VectorToBais(dir);

        var rightFoot = MathUtils.AngleToVector(-150f);

        GD.Print("右脚方向", playerBaiss * rightFoot, playerBaiss, rightFoot);

        // 绝对  =  相对 * 子

    }
}


