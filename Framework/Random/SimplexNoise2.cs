//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Framework.Random
//{
//    public class SimplexNoise
//    {
//        /* Unit vectors for gradients to points on cube,equal distances apart (ie vector from center to the middle of each side */
//        private static readonly int[][] _grad3 = new[]
//      {
//        new[] { 1, 1, 0 }, new[] { -1, 1, 0 }, new[] { 1, -1, 0 }, new[] { -1, -1, 0 },
//        new[] { 1, 0, 1 }, new[] { -1, 0, 1 }, new[] { 1, 0, -1 }, new[] { -1, 0, -1 },
//        new[] { 0, 1, 1 }, new[] { 0, -1, 1 }, new[] { 0, 1, -1 }, new[] { 0, -1, -1 }
//      };

//        //0..255, randomized
//        private static readonly int[] _p = 
//      {
//        151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103,
//        30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197,
//        62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20,
//        125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231,
//        83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102,
//        143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200,
//        196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226,
//        250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16,
//        58, 17, 182, 189, 28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70,
//        221, 153, 101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224,
//        232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12,
//        191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199,
//        106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93,
//        222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
//      };

//        private static readonly int[] _perm = new int[512];

//        private static readonly double Sqrt3 = Math.Sqrt(3);
//        private static readonly double F2 = 0.5 * (Sqrt3 - 1.0);
//        private static readonly double G2 = (3.0 - Sqrt3) * Div6;
//        private const double Div6 = 1.0 / 6.0;
//        private const double Div3 = 1.0 / 3.0;

//        static SimplexNoise()
//        {
//            for (int i = 0; i < 512; i++)
//            {
//                _perm[i] = _p[i & 255];
//            }

//            Singleton = new SimplexNoise();
//        }

//        public static SimplexNoise Singleton { get; private set; }

//        public double Noise01(double x, double y)
//        {
//            return (Noise(x, y) + 1) * 0.5;
//        }

//        public double MultiNoise(int octaves, double x, double y)
//        {
//            double value = 0.0;
//            float mul = 1;
//            for (int i = 0; i < octaves; i++)
//            {
//                value += Noise((x + 10) * mul, (y + 15) * mul) / mul;

//                mul *= 2;
//            }
//            return value;
//        }

//        public double MultiNoise01(int octaves, double x, double y)
//        {
//            return (MultiNoise(octaves, x, y) + 1.0) * 0.5;
//        }

//        public double RidgedMulti(int octaves, double x, double y)
//    {
//      double value = 0.0;
//      double mul = 1;
//      for (int i = 0; i  y0)
//      {
//        // lower triangle, XY order: (0,0)->(1,0)->(1,1)
//        i1 = 1;
//        j1 = 0;
//      }
//      else
//      {
//        i1 = 0;
//        j1 = 1;
//      } // upper triangle, YX order: (0,0)->(0,1)->(1,1)
 
//      // A step of (1,0) in (i,j) means a step of (1-c,-c) in (x,y), and
//      // a step of (0,1) in (i,j) means a step of (-c,1-c) in (x,y), where
//      // c = (3-sqrt(3))/6
 
//      double x1 = x0 - i1 + G2; // Offsets for middle corner in (x,y) unskewed coords
//      double y1 = y0 - j1 + G2;
//      double x2 = x0 - 1.0 + 2.0 * G2; // Offsets for last corner in (x,y) unskewed coords
//      double y2 = y0 - 1.0 + 2.0 * G2;
 
//      // Work out the hashed gradient indices of the three simplex corners
//      int ii = i & 255;
//      int jj = j & 255;
//      int gi0 = _perm[ii + _perm[jj]] % 12;
//      int gi1 = _perm[ii + i1 + _perm[jj + j1]] % 12;
//      int gi2 = _perm[ii + 1 + _perm[jj + 1]] % 12;
 
//      // Calculate the contribution from the three corners
//      double t0 = 0.5 - x0 * x0 - y0 * y0;
//      if (t0 < 0)
//      {
//        n0 = 0.0;
//      }
//      else
//      {
//        t0 *= t0;
//        n0 = t0 * t0 * Dot(_grad3[gi0], x0, y0); // (x,y) of grad3 used for 2D gradient
//      }
//      double t1 = 0.5 - x1 * x1 - y1 * y1;
//      if (t1 < 0)
//      {
//        n1 = 0.0;
//      }
//      else
//      {
//        t1 *= t1;
//        n1 = t1 * t1 * Dot(_grad3[gi1], x1, y1);
//      }
//      double t2 = 0.5 - x2 * x2 - y2 * y2;
//      if (t2 < 0)
//      {
//        n2 = 0.0;
//      }
//      else
//      {
//        t2 *= t2;
//        n2 = t2 * t2 * Dot(_grad3[gi2], x2, y2);
//      }
//      // Add contributions from each corner to get the final noise value.
//      // The result is scaled to return values in the interval [-1,1].
//      return 70.0 * (n0 + n1 + n2);
//    }

//        public double Multi01(int octaves, double x, double y, double z)
//        {
//            return (Multi(octaves, x, y, z) + 1) * 0.5;
//        }

//        public double Multi(int octaves, double x, double y, double z)
//        {
//            double value = 0.0;
//            double mul = 1;
//            for (int i = 0; i < octaves; i++)
//            {
//                double added = Noise(x * mul, y * mul, z * mul) / mul;
//                value += added;
//                mul *= 2;
//            }
//            return value;
//        }

//        public double Noise01(double x, double y, double z)
//        {
//            // Noise  is in the range -1 to +1
//            double val = Noise(x, y, z);
//            return (val + 1) * 0.5;
//        }

//        public double RidgedMulti(int octaves, double x, double y, double z)
//    {
//      double value = 0.0;
//      double mul = 1;
//      for (int i = 0; i = y0)
//      {
//        if (y0 >= z0)
//        {
//          i1 = 1;
//          j1 = 0;
//          k1 = 0;
//          i2 = 1;
//          j2 = 1;
//          k2 = 0;
//        }
//        else if (x0 >= z0)
//        {
//          i1 = 1;
//          j1 = 0;
//          k1 = 0;
//          i2 = 1;
//          j2 = 0;
//          k2 = 1;
//        }
//        else
//        {
//          i1 = 0;
//          j1 = 0;
//          k1 = 1;
//          i2 = 1;
//          j2 = 0;
//          k2 = 1;
//        }
//      }
//      else
//      {
//        // x0<y0
//        if (y0 < z0)
//        {
//          i1 = 0;
//          j1 = 0;
//          k1 = 1;
//          i2 = 0;
//          j2 = 1;
//          k2 = 1;
//        }
//        else if (x0 < z0)
//        {
//          i1 = 0;
//          j1 = 1;
//          k1 = 0;
//          i2 = 0;
//          j2 = 1;
//          k2 = 1;
//        }
//        else
//        {
//          i1 = 0;
//          j1 = 1;
//          k1 = 0;
//          i2 = 1;
//          j2 = 1;
//          k2 = 0;
//        }
//      }
//      // A step of (1,0,0) in (i,j,k) means a step of (1-c,-c,-c) in (x,y,z),
//      // a step of (0,1,0) in (i,j,k) means a step of (-c,1-c,-c) in (x,y,z), and
//      // a step of (0,0,1) in (i,j,k) means a step of (-c,-c,1-c) in (x,y,z), where
//      // c = 1/6.
//      double x1 = x0 - i1 + Div6; // Offsets for second corner in (x,y,z) coords
//      double y1 = y0 - j1 + Div6;
//      double z1 = z0 - k1 + Div6;
//      double x2 = x0 - i2 + 2.0 * Div6; // Offsets for third corner in (x,y,z) coords
//      double y2 = y0 - j2 + 2.0 * Div6;
//      double z2 = z0 - k2 + 2.0 * Div6;
//      double x3 = x0 - 1.0 + 3.0 * Div6; // Offsets for last corner in (x,y,z) coords
//      double y3 = y0 - 1.0 + 3.0 * Div6;
//      double z3 = z0 - 1.0 + 3.0 * Div6;
//      // Work out the hashed gradient indices of the four simplex corners
//      int ii = i & 255;
//      int jj = j & 255;
//      int kk = k & 255;
//      int gi0 = _perm[ii + _perm[jj + _perm[kk]]] % 12;
//      int gi1 = _perm[ii + i1 + _perm[jj + j1 + _perm[kk + k1]]] % 12;
//      int gi2 = _perm[ii + i2 + _perm[jj + j2 + _perm[kk + k2]]] % 12;
//      int gi3 = _perm[ii + 1 + _perm[jj + 1 + _perm[kk + 1]]] % 12;
//      // Calculate the contribution from the four corners
//      double t0 = 0.6 - x0 * x0 - y0 * y0 - z0 * z0;
//      if (t0 < 0)
//      {
//        n0 = 0.0;
//      }
//      else
//      {
//        t0 *= t0;
//        n0 = t0 * t0 * Dot(_grad3[gi0], x0, y0, z0);
//      }
//      double t1 = 0.6 - x1 * x1 - y1 * y1 - z1 * z1;
//      if (t1 < 0)
//      {
//        n1 = 0.0;
//      }
//      else
//      {
//        t1 *= t1;
//        n1 = t1 * t1 * Dot(_grad3[gi1], x1, y1, z1);
//      }
//      double t2 = 0.6 - x2 * x2 - y2 * y2 - z2 * z2;
//      if (t2 < 0)
//      {
//        n2 = 0.0;
//      }
//      else
//      {
//        t2 *= t2;
//        n2 = t2 * t2 * Dot(_grad3[gi2], x2, y2, z2);
//      }
//      double t3 = 0.6 - x3 * x3 - y3 * y3 - z3 * z3;
//      if (t3  0 ? (int)x : (int)x - 1;
//    }

//        private static double Dot(int[] g, double x, double y)
//        {
//            return g[0] * x + g[1] * y;
//        }

//        private static double Dot(int[] g, double x, double y, double z)
//        {
//            return g[0] * x + g[1] * y + g[2] * z;
//        }

//        private static double Dot(int[] g, double x, double y, double z, double w)
//        {
//            return g[0] * x + g[1] * y + g[2] * z + g[3] * w;
//        }
//    }
//}
