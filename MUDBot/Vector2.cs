using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MUDBot
{
    public class Vector2
    {
        [JsonProperty("X")]
        public int x;
        [JsonProperty("Y")]
        public int y;

        public Vector2()
        {
            x = 0;
            y = 0;
        }
        [JsonConstructor]
        public Vector2(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static readonly Vector2 Zero = new Vector2();
        
        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x + v2.x, v1.y + v2.y);
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x - v2.x, v1.y - v2.y);
        }

        public static float Distance(Vector2 v1, Vector2 v2)
        {
            return (float)Math.Sqrt((v2.x - v1.x) * (v2.x - v1.x) + (v2.y - v1.y) * (v2.y - v1.y));
        }

        public int ToGridDigit(int maxGridSize)
        {
            return x + (y * maxGridSize);
        }

        public static Vector2 FromGridDigit(int value, int maxGridSize)
        {
            int x = value%maxGridSize;
            int y = value/maxGridSize;
            return new Vector2(x, y);
        }

        public override bool Equals(object obj)
        {
            Vector2 other = (Vector2)obj;
            return (this.x == other.x && this.y == other.y);
        }

        public override string ToString()
        {
            return $"[ {x} , {y} ]";
        }
    }
}
