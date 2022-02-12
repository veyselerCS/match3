using UnityEngine;

public static class Vector3Extensions 
{
    public static Vector3 SetX(this Vector3 vec, float x)
    {
        return new Vector3(x, vec.y, vec.z);
    }    
    
    public static Vector3 SetY(this Vector3 vec, float y)
    {
        return new Vector3(vec.x, y, vec.z);
    }
    
    public static Vector3 SetZ(this Vector3 vec, float z)
    {
        return new Vector3(vec.x, vec.y, z);
    }    
    
    public static Vector3 IncX(this Vector3 vec, float x)
    {
        return new Vector3(vec.x + x, vec.y, vec.z);
    }    
    
    public static Vector3 IncY(this Vector3 vec, float y)
    {
        return new Vector3(vec.x, vec.y + y, vec.z);
    }
    
    public static Vector3 IncZ(this Vector3 vec, float z)
    {
        return new Vector3(vec.x, vec.y, vec.z + z);
    }
}