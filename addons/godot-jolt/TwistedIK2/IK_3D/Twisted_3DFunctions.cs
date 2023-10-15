using Godot;
using System;

///<summary>
/// A helper class that has several helper functions for 3D use.
///</summary>
public static class Twisted_3DFunctions {

    ///<summary>
    /// Used to get the Quaternion offset from one direction to the desired direction. The returned Quaternion will
    /// only calculate the swing needed to get the input direction to the desired direction, unlike <c>look_at</c>
    /// which will use twist rotation in addition to swing.
    /// </summary>
    /// <returns>
    /// Returns a Quaternion that is the offset needed to get the <paramref name="start_direction" /> to the
    /// <paramref name = "end_direction" />. This function just returns the offset required, the returned
    /// Quaternion is not the rotation itself.
    /// </returns>
    ///<param name="start_direction">The direction the Transform3D is currently facing in</param>
    ///<param name="end_direction">The direction you want the Transform3D to face</param>
    public static Quaternion quat_from_two_vectors(Vector3 start_direction, Vector3 end_direction) {
        Quaternion return_quat = Quaternion.Identity;

        Vector3 v0 = start_direction.Normalized();
        Vector3 v1 = end_direction.Normalized();
        float d = v0.Dot(v1);

        if (d >= 1.0f) {
            return Quaternion.Identity;
        }

        if (d < (1e-6 - 1.0)) {
            // Generate an axis (TODO: Add fallback support?)
            Vector3 axis = Vector3.Right.Cross(start_direction);
            if (axis.LengthSquared() == 0) {
                axis = Vector3.Up.Cross(start_direction);
            }
            axis = axis.Normalized();
            return_quat = new Quaternion(axis, Mathf.Pi);
        } 
        else
        {
            float s = Mathf.Sqrt((1+d) * 2);
            float invs = 1 / s;

            Vector3 c = v0.Cross(v1);

            return_quat.X = c.X * invs;
            return_quat.Y = c.Y * invs;
            return_quat.Z = c.Z * invs;
        }

        return return_quat.Normalized();
    }

    /// <summary>
    /// Works the same way as the LookAt function, but it uses quaternions instead of reconstructing the Basis.
    /// The advantage of doing it this way is that it doesn't twist to get to the desired position, it only uses swing rotation.
    /// </summary>
    /// <param name="input_trans">The transform you want to rotate</param>
    /// <param name="target_position">The point (in global space) that the transform rotates to</param>
    /// <returns>A transform where the negative Z axis faces the target position passed</returns>
    public static Transform3D quat_based_lookat(Transform3D input_trans, Vector3 target_position) {
        Transform3D output_trans = input_trans;
        Vector3 input_to_target_dir = input_trans.Origin.DirectionTo(target_position);
        Quaternion rotation_quat = quat_from_two_vectors(-input_trans.Basis.Z.Normalized(), input_to_target_dir);
        output_trans.Basis = new Basis(rotation_quat * get_rotation_quat(input_trans.Basis));
        return output_trans;
    }

    /// <summary>
    ///     Returns the input Basis and converts it to a Quaternion. Identical to the <c>get_rotation_quat</c> function in GDScript.
    /// </summary>
    /// <returns>
    ///     Returns the Basis converted to a Quaternion.
    ///     This is different than the <c>Basis.Quaternion()</c> function, this function has some extra features
    ///     that handle special conditions in the Basis.
    ///     (This code is ported from the Godot C++ source, as for some reason C# doesn't include it by default)
    /// </returns>
    /// <param name="input">The Basis that you want in Quaternion form</param>
    public static Quaternion get_rotation_quat(Basis input) {
        Basis m = input.Orthonormalized();
        float det = m.Determinant();
        if (det < 0) {
            m = m.Scaled( new Vector3(-1, -1, -1));
        }

        return m.GetRotationQuaternion();
    }
}