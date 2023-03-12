using UnityEngine;

public static class Utils
{ 
    public static Vector3 ChangeX(Vector3 v, float x)
    {
        return new Vector3(v.x + x, v.y, v.z);
    }
 
    public static Vector3 ChangeY(Vector3 v, float y)
    {
        return new Vector3(v.x, v.y + y, v.z);
    }
 
    public static Vector3 ChangeZ(Vector3 v, float z)
    {
        return new Vector3(v.x, v.y, v.z + z);
    }

    public static int flipToInt(bool isFlip)
    {
        return isFlip ? -1 : 1;
    }

}