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
using System.Collections.Generic;
using System.Text;

namespace KinTemplates
{
    public class RadialPointD
    {
        private double m_r;
        private double m_theta;
        public static readonly RadialPointD Empty = new RadialPointD();

        public override string ToString()
        {
            return "RadialPointD {r: " + m_r + ", θ " + m_theta + "}";

        }

        public double R
        {
            get { return m_r; }
            set { m_r = value; }
        }

        public double Theta
        {
            get { return m_theta; }
            set { m_theta = value; }
        }

        public RadialPointD()
        {
            m_r = 0;
            m_theta = 0;
        }

        public RadialPointD(double radius, double theta)
        {
            m_r = radius;
            m_theta = theta;
        }

        public static RadialPointD ToPolar(PointD location, PointD origin)
        {
            PointD v = location - origin;
            RadialPointD n = new RadialPointD();
            n.m_r = v.Magnitude();
            n.m_theta = Math.Atan2(v.Y, v.X);
            return n;
        }

        public static PointD ToCartesian(RadialPointD location, PointD origin)
        {
            PointD p = new PointD();
            p.X = location.R * Math.Cos(location.Theta);
            p.Y = location.R * Math.Sin(location.Theta);
            return p + origin;
        }

        public static RadialPointD operator +(RadialPointD a, RadialPointD b)
        {
            return new RadialPointD(a.R + b.R, a.Theta + b.Theta);
        }

        public static RadialPointD operator -(RadialPointD a, RadialPointD b)
        {
            return new RadialPointD(a.R - b.R, a.Theta - b.Theta);
        }

    }
}
