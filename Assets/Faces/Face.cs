using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Face
{
    [Serializable]
    public class Rectangle
    {
        [SerializeField]
        public int top;
        [SerializeField]
        public int left;
        [SerializeField]
        public int width;
        [SerializeField]
        public int height;
    };
    [Serializable]
    public class Attributes
    {
        [Serializable]
        public class Pose
        {
            [SerializeField]
            public float pitch;
            [SerializeField]
            public float roll;
            [SerializeField]
            public float yaw;



        }
        [Serializable]
        public class Hair
        {
            [SerializeField]
            public float moustache;
            [SerializeField]
            public float beard;
            [SerializeField]
            public float sideburns;
        }
        [SerializeField]
        public float smile;
        [SerializeField]
        public string gender;
        [SerializeField]
        public float age;
        [SerializeField]
        public Pose headPose;
        [SerializeField]
        public Hair facialHair;
        [SerializeField]
        public string glasses;
    };
    [SerializeField]
    public Rectangle faceRectangle;
    [SerializeField]
    public Attributes faceAttributes;
    [SerializeField]
    public string faceId;

}
