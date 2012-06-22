// Kinematic Templates Cursor Manipulation Library
// Copyright (C) 2009, Richard H. Fung
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;

namespace KinTemplates
{
    public struct PointD 
    {
        private double m_X;
        private double m_Y;
        public static readonly  PointD Empty = new PointD();

        public override string ToString()
        {
            return ToString("PointD");
        }

        public string ToString(string customMessage)
        {
            return customMessage + " {" + m_X + ", " + m_Y + "}";
        }

        public string ToDescriptiveString()
        {
            return ToString("PointD") + " l:" + Magnitude().ToString();
        }

        public PointD(double aX, double aY)
        {
            m_X = aX;
            m_Y = aY;
        }

        public double Magnitude()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public static PointD operator +(PointD a, PointD b)
        {
            return new PointD(a.X + b.X, a.Y + b.Y);
        }

        public static PointD operator -(PointD a, PointD b)
        {
            return new PointD(a.X - b.X, a.Y - b.Y);
        }

        public static PointD operator *(PointD a, double c)
        {
            return new PointD(a.X * c, a.Y * c);
        }

        public static PointD operator *(double c, PointD a)
        {
            return new PointD(a.X * c, a.Y * c);
        }

        public static PointD operator /(PointD a, double c)
        {
            return new PointD(a.X / c, a.Y / c);
        }

        public static bool operator ==(PointD left, PointD right)
        {
            // 3 digits after decimal point
            return (Math.Round(left.X, 3) == Math.Round(right.X, 3)) &&
                 (Math.Round(left.Y, 3) == Math.Round(right.Y, 3));
        }

        public static bool operator !=(PointD left, PointD right)
        {
            // 3 digits after decimal point
            return (Math.Round(left.X, 3) != Math.Round(right.X, 3)) ||
                 (Math.Round(left.Y, 3) != Math.Round(right.Y, 3));

        }

        // TODO: this doesn't work!!
        public override int GetHashCode()
        {
            return base.GetHashCode();
        } 


        public override bool Equals(object obj)
        {
            if (!(obj is PointD))
            {
                return false;
            }
            return (PointD)obj == this;
        }


        public static explicit operator System.Drawing.Point(PointD a)
        {
            return new System.Drawing.Point((int)a.X, (int)a.Y);
        }

        public static explicit operator System.Drawing.PointF(PointD a)
        {
            return new System.Drawing.PointF((float)a.X, (float)a.Y);
        }

        public static explicit operator PointD(System.Drawing.PointF a)
        {
            return new PointD(a.X, a.Y);
        }

        public static explicit operator PointD(System.Drawing.Point a)
        {
            return new PointD(a.X, a.Y);
        }

        public static double DotProduct(PointD a, PointD b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        public static PointD Orthogonal(PointD a)
        {
            PointD newPoint = new PointD();
            double temp = -a.Y;
            newPoint.Y = a.X;
            newPoint.X = temp;
            return newPoint;
        }

        public static System.Drawing.Rectangle PointBounds(PointD point1, PointD point2)
        {
            double left = Math.Min(point1.X, point2.X);
            double top = Math.Min(point1.Y, point2.Y);
            double right = Math.Max(point1.X, point2.X);
            double bottom = Math.Max(point1.Y, point2.Y);
            return new System.Drawing.Rectangle((int)left, (int)top, (int)(right - left), (int)(bottom - top));
        }

        public static PointD FromSize(System.Drawing.Size s)
        {
            return new PointD(s.Width, s.Height);
        }

        public static PointD FromSize(System.Drawing.SizeF s)
        {
            return new PointD(s.Width, s.Height);
        }

        public static PointD UnitVector(PointD vector)
        {
            if (vector.Magnitude() < 0.0001)
                return PointD.Empty;
            else
                return vector / vector.Magnitude();
        }

        public void Offset(double offsetX, double offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        public bool IsEmpty
        {
            get
            {
                return (X == 0 && Y == 0);
            }
        }

        public double X
        {
            get
            {
                return m_X;
            }
            set
            {
                m_X = value;
            }
        }

        public double Y
        {
            get
            {
                return m_Y;
            }
            set
            {
                m_Y = value;
            }
        }
    }
}