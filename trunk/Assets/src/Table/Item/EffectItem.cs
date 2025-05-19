using System;
using System.Numerics;

namespace TableItem
{
    public class Effect
    {
        public string Name { get; set; }
        public string Prefab { get; set; }
        public float Time { get; set; }
        public bool Loop { get; set; }
        public string Node { get; set; }
        public float[] Offset { get; set; }
        public float[] Scale { get; set; }
    }
}