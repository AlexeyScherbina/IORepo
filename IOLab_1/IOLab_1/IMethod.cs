using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOLab_1
{
    public interface IMethod
    {
        double Solve(object[] args, IFunction f);
    }
    public interface IFunction
    {
        double Func(double x);
        double Derivative(double x);
    }
    public class Golden : IMethod
    {
        public double Solve(object[] args, IFunction f)
        {
            double a = (double)args[0];
            double b = (double)args[1];
            bool is_min = (bool)args[2];
            double e = (double)args[3];
            if (e <= 0)
                return double.NaN;

            if (a >= b)
                return double.NaN;

            double GS_proportion = (1 + Math.Sqrt(5)) / 2;

            while (Math.Abs(b - a) > e)
            {
                double x1 = b - (b - a) / GS_proportion;
                double x2 = a + (b - a) / GS_proportion;

                double y1 = f.Func(x1);
                double y2 = f.Func(x2);

                if (is_min == true && y1 >= y2 || is_min == false && y1 <= y2)
                {
                    a = x1;
                    x1 = x2;
                    x2 = a + (b - a) / GS_proportion;
                }
                else
                {
                    b = x2;
                    x2 = x1;
                    x1 = b - (b - a) / GS_proportion;
                }
            }

            return (a + b) / 2;
        }
    }

    public class Cubic : IMethod
    {
        public double Solve(object[] args, IFunction f)
        {
            double x0 = (double)args[0];
            double step = (double)args[1];
            double eps1 = (double)args[2];
            double eps2 = (double)args[3];
            if (x0 > 0)
                return double.NaN;

            if (step <= 0)
                return double.NaN;

            //Шаг 1 + 2.1
            List<double> x = new List<double>();
            x.Add(x0);
            int k = 0;
            do
            {
                if (f.Derivative(x[x.Count - 1]) < 0)
                    x.Add(x[x.Count - 1] + Math.Pow(2, k) * step);

                if (f.Derivative(x[x.Count - 1]) > 0)
                    x.Add(x[x.Count - 1] - Math.Pow(2, k) * step);

                k++;
            }
            while (f.Derivative(x[x.Count - 2]) * f.Derivative(x[x.Count - 1]) >= 0);

            //Шаг 2.2
            double x1 = x[x.Count - 2];
            double x2 = x[x.Count - 1];

            //Шаг 2.3
            double f1 = f.Func(x1);
            double f2 = f.Func(x2);
            double f1_d = f.Derivative(x1);
            double f2_d = f.Derivative(x2);

            double z = (3 * (f1 - f2)) / (x2 - x1) + f1_d + f2_d;

            double w = double.NaN;
            if (x1 < x2)
                w = Math.Pow(Math.Pow(z, 2) - f1_d * f2_d, 0.5);
            if (x1 > x2)
                w = -Math.Pow(Math.Pow(z, 2) - f1_d * f2_d, 0.5);

            double m = (f2_d + w - z) / (f2_d - f1_d + 2 * w);

            //Шаг 3
            double x_stationary = double.NaN;
            if (m < 0)
                x_stationary = x2;
            if (0 <= m && m <= 1)
                x_stationary = x2 - m * (x2 - x1);
            if (m > 1)
                x_stationary = x2;

            //Шаг 4
            while (f.Func(x_stationary) > f.Func(x1))
                x_stationary = x_stationary + 0.5 * (x_stationary - x1);

            //Шаг 5
            for (int c = 0; c < 500; c++)
            {
                if (f.Derivative(x_stationary) <= eps1 && Math.Abs((x_stationary - x1) / x_stationary) <= eps2)
                    return x_stationary;

                if (f.Derivative(x_stationary) * f.Derivative(x1) < 0)
                {
                    x2 = x1;
                    x1 = x_stationary;
                }

                if (f.Derivative(x_stationary) * f.Derivative(x2) < 0)
                {
                    x1 = x_stationary;
                }

                //Переход на шаг 3
                x_stationary = double.NaN;
                if (m < 0)
                    x_stationary = x2;
                if (0 <= m && m <= 1)
                    x_stationary = x2 - m * (x2 - x1);
                if (m > 1)
                    x_stationary = x2;
            }

            return x_stationary;
        }
    }
}
