using Godot;
using System;

///<summary>
/// A helper class that has several helper functions for 2D use.
///</summary>
public static class Twisted_2DFunctions {

    /// <summary>
    /// Clamps an angle to the given min and max bounds. The bounds and angle passed are expected to be in radians.
    /// Setting <c>invert</c> to true will inversely clamp the angle, clamping it to the range outside of the given bounds.
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="min_bound"></param>
    /// <param name="max_bound"></param>
    /// <param name="invert"></param>
    /// <returns></returns>
    public static float clamp_angle(float angle, float min_bound, float max_bound, bool invert=false) {
        // Map to the 0 to 360 range (in radians though) instead of the -180 to 180 range.
        if (angle < 0) {
            angle = (Mathf.Pi * 2) + angle;
        }

        // Make min and max in the range of 0 to 360 (in radians), and make sure they are in the right order
        if (min_bound < 0) {
            min_bound = (Mathf.Pi * 2) + min_bound;
        }
        if (max_bound < 0) {
            max_bound = (Mathf.Pi * 2) + max_bound;
        }
        if (min_bound > max_bound) {
            float tmp = min_bound;
            min_bound = max_bound;
            max_bound = tmp;
        }

        // Note: May not be the most optimal way to clamp, but it always constraints to the nearest angle.
        if (invert == false) {
            if (angle < min_bound || angle > max_bound) {
                Vector2 min_bound_vec = new Vector2(Mathf.Cos(min_bound), Mathf.Sin(min_bound));
                Vector2 max_bound_vec = new Vector2(Mathf.Cos(max_bound), Mathf.Sin(max_bound));
                Vector2 angle_vec = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                if (angle_vec.DistanceSquaredTo(min_bound_vec) <= angle_vec.DistanceSquaredTo(max_bound_vec)) {
                    angle = min_bound;
                } else {
                    angle = max_bound;
                }
            }
        } else {
            if (angle > min_bound && angle < max_bound) {
                Vector2 min_bound_vec = new Vector2(Mathf.Cos(min_bound), Mathf.Sin(min_bound));
                Vector2 max_bound_vec = new Vector2(Mathf.Cos(max_bound), Mathf.Sin(max_bound));
                Vector2 angle_vec = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                if (angle_vec.DistanceSquaredTo(min_bound_vec) <= angle_vec.DistanceSquaredTo(max_bound_vec)) {
                    angle = min_bound;
                } else {
                    angle = max_bound;
                }
            }
        }
        return angle;
    }


    /// <summary>
    /// The LookAt function used in Node2D, but programmed so it works on a Transform2D since
    /// its not exposed in C++ (as of when this was written)
    /// </summary>
    /// <param name="input">The Transform2D you want to rotate</param>
    /// <param name="target">The target position to rotate towards</param>
    /// <returns></returns>
    public static Transform2D looking_at(Transform2D input, Vector2 target) {
        return new Transform2D(
            (input.Rotation + (target * input.Scale).Angle()),
            input.Scale,
            input.Skew,
            input.Origin
        );
    }

    /// <summary>
    /// Takes a world transform and makes it a global pose (a transform relative to the Skeleton2D node).
    /// This is needed so you can run IK algorithms on Skeletons that have strange (like negative) scales.
    /// This function is similar to the world transform to global pose function used in 3D.
    /// </summary>
    /// <param name="world_trans">The world transform you want to convert</param>
    /// <param name="skeleton">The Skeleton2D you want to make the transform a global pose of</param>
    /// <returns></returns>
    public static Transform2D world_transform_to_global_pose(Transform2D world_trans, Skeleton2D skeleton) {
        return skeleton.GlobalTransform.AffineInverse() * world_trans;
    }

    /// <summary>
    /// Takes a global pose and converts it to a world transform.
    /// This is needed so you can take the transforms for IK from global pose (so it works with strange scales)
    /// but still use it on the GlobalTransform of Twisted_Bone2D nodes.
    /// </summary>
    /// <param name="world_trans"></param>
    /// <param name="skeleton"></param>
    /// <returns></returns>
    public static Transform2D global_pose_to_world_transform(Transform2D world_trans, Skeleton2D skeleton) {
        return skeleton.GlobalTransform * world_trans;
    }

}