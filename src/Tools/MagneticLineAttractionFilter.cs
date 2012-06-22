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
using System.Drawing;

namespace KinTemplates.Cursor.Tools
{
    public class MagneticLineAttractionFilter : LineToolFilter
    {
        public override string ToString()
        {
            return "Attraction Line";
        }

        public virtual int GetScalar()
        {
            return -1;
        }

        #region IToolFilter Members

        public override PointD GetVelocity(Position.IVirtualMousePosition m)
        {
            /*
            // lets define the line from (100, 100) to (300, 200)
            PointD linePt1 = m_Param.Pt;
            PointD linePt2 = m_Param.V;
            PointD lineVector = linePt2 - linePt1;

            if (lineVector.Magnitude() < 0.00001)
                return PointD.Empty;

            PointD unitLineVector = lineVector / lineVector.Magnitude();
            PointD normalVector = PointD.Orthogonal(unitLineVector);
            PointD theVector = m.GetVirtualPointD() - m_Param.Pt;
            PointD normalComponent = PointD.DotProduct(theVector, normalVector) * normalVector;

            if (normalComponent.Magnitude() < 0.00001)
                return PointD.Empty;

            PointD unitNormalComponent = normalComponent / normalComponent.Magnitude();
            PointD mappedToLine = m.GetVirtualPointD() - normalComponent;
            if (Between(mappedToLine.X, linePt1.X, linePt2.X) &&
                Between(mappedToLine.Y, linePt1.Y, linePt2.Y))
            {
                PointD scaledVector = unitNormalComponent * (GetScalar() * 5 / Math.Pow(normalComponent.Magnitude() + 10, 0.5));
                return scaledVector;
            }
            else
            {
                return PointD.Empty;
            }
             */

            PointD normalVector;
            PointD nearPt;
            double radius;
            PointChargeNormalOfPoint(m.GetVirtualPointD(), out normalVector, out nearPt, out radius);
            
            PointD theVector = m.GetVirtualPointD() - nearPt;
            PointD normalComponent = PointD.DotProduct(theVector, normalVector) * normalVector;

            if (normalComponent.Magnitude() < 2.0)
                return PointD.Empty;

            PointD unitNormalComponent = normalComponent / normalComponent.Magnitude();
            PointD mappedToLine = m.GetVirtualPointD() - normalComponent;
            return unitNormalComponent * m_Param.C * GetScalar();

        }

        public override void DrawArrowForces(System.Drawing.Graphics gc, Render.RenderParameter r, Render.IDrawVisitor drawMethods)
        {
            PointF[] array = m_pointsInPath;
            if (m_pointsInPath == null)
                return;
            for (int i = 1; i < array.Length; i += 20)
            {
                PointD tangent = new PointD(array[i].X - array[i - 1].X, array[i].Y - array[i - 1].Y);
                tangent = tangent / tangent.Magnitude();
                PointD normalVector = PointD.Orthogonal(tangent);

                PointD pointOnLine = new PointD((array[i].X + array[i - 1].X) / 2.0, (array[i].Y + array[i - 1].Y) / 2.0);

                PointD testPt1 = pointOnLine + normalVector * m_Param.PathThickness / 4.0;
                PointD testPt2 = pointOnLine - normalVector * m_Param.PathThickness / 4.0;

                PointD testNormal1 = (testPt1 - pointOnLine);
                testNormal1 = testNormal1 / testNormal1.Magnitude();

                PointD testNormal2 = (testPt2 - pointOnLine);
                testNormal2 = testNormal2 / testNormal2.Magnitude();

                PointD scaledVector1 = testNormal1 * (GetScalar() * 7 * m_Param.C / Math.Pow((m_Param.PathThickness / 2.0) + 10, 0.5));
                PointD scaledVector2 = testNormal2 * (GetScalar() * 7 * m_Param.C / Math.Pow((m_Param.PathThickness / 2.0) + 10, 0.5));

                drawMethods.DrawArrow(gc, r, testPt1, scaledVector1 * Render.DrawHelper.SPEED_AMPLIFIER );
                drawMethods.DrawArrow(gc, r, testPt2, scaledVector2 * Render.DrawHelper.SPEED_AMPLIFIER);
            }
        }

        private bool Between(double testValue, double bound1, double bound2)
        {
            if (bound1 < bound2)
            {
                return bound1 <= testValue && testValue <= bound2;
            }
            else
            {
                return bound2 <= testValue && testValue <= bound1;
            }
        }
            /*
            gc.DrawLine(Pens.Green, new Point(100, 100), new Point(300, 200));
            gc.DrawEllipse(Pens.Green, (float)mappedToLine.X, (float)mappedToLine.Y, 4f, 4f);
            gc.DrawLine(Pens.Green, 100f, 100f, 100f + (float)normalComponent.X, 100f + (float)normalComponent.Y);
             */

        protected override void SecondCache(Render.IDrawVisitor drawMethods)
        {
            
        }


        #endregion
    }
}
