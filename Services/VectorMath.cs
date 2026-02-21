using System;
using System.Collections.Generic;

namespace Business_App_Dev.Services
{
    public static class VectorMath
    {
        public static float[] Average(List<float[]> vectors)
        {
            if (vectors == null || vectors.Count == 0)
                return Array.Empty<float>();

            int dim = vectors[0].Length;
            var sum = new double[dim];

            foreach (var v in vectors)
            {
                if (v == null || v.Length != dim) continue;
                for (int i = 0; i < dim; i++)
                    sum[i] += v[i];
            }

            var avg = new float[dim];
            for (int i = 0; i < dim; i++)
                avg[i] = (float)(sum[i] / vectors.Count);

            return avg;
        }

        public static double Cosine(float[] a, float[] b)
        {
            if (a == null || b == null) return 0;
            if (a.Length == 0 || b.Length == 0) return 0;
            if (a.Length != b.Length) return 0;

            double dot = 0, na = 0, nb = 0;

            for (int i = 0; i < a.Length; i++)
            {
                dot += a[i] * b[i];
                na += a[i] * a[i];
                nb += b[i] * b[i];
            }

            if (na == 0 || nb == 0) return 0;
            return dot / (Math.Sqrt(na) * Math.Sqrt(nb));
        }

        public static double Clamp01(double x)
        {
            if (x < 0) return 0;
            if (x > 1) return 1;
            return x;
        }
    }
}