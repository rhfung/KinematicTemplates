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
using System.Drawing.Drawing2D;

namespace KinTemplates.Cursor.Tools
{
    public class TunnelLineFilter : LineToolFilter
    {
        List<PointF>[] m_traceLines;

        public override string ToString()
        {
            return "Tunnel Line";
        }

        public  int GetScalar()
        {
            return -1;
        }

        #region IToolFilter Members


        public override PointD GetVelocity(Position.IVirtualMousePosition m)
        {
            PointD normalVector;
            PointD nearPt;
            double radius;
            ExtendedNormalOfPoint(m.GetVirtualPointD(), out normalVector, out nearPt, out radius);

            PointD dir = PointD.Orthogonal(normalVector);
            PointD normalComponent = PointD.DotProduct(dir, m.GetVelocity()) * dir;
            PointD tangentComponent = m.GetVelocity() - normalComponent;

            return -1 * tangentComponent * m_Param.C;
        }

        public override void DrawArrowForces(System.Drawing.Graphics gc, Render.RenderParameter r, Render.IDrawVisitor drawMethods)
        {
            if (m_visibleThickPath != null)
            {
                GraphicsState o = gc.Save();
                gc.SetClip(m_visibleThickPath);
                if ((m_traceLines != null) && (m_traceLines.Length > 0) && (m_traceLines[0].Count > 1))
                {
                    for (int k = 0; k < m_traceLines.Length; k++)
                    {
                        gc.DrawLines(r.RegionGuides, m_traceLines[k].ToArray());
                    }
                }
                gc.Restore(o);
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
            if (m_Param.Path.PointCount == 0)
                return;

            PointF[] array = ((Tools.Model.VectorPath)m_Param.Path).InternalPath.PathPoints;

            // calculate number of lines to show for feedback
            int spacing = (int)drawMethods.Spacing(m_Param.C);
            int numLines = (int)m_Param.PathThickness / spacing;

            // build a number of trace lines
            m_traceLines = new List<PointF>[numLines];
            for (int j = 0; j < numLines; j++)
                m_traceLines[j] = new List<PointF>(array.Length);

            // render all the points
            for (int i = 1; i < array.Length; i += 2)
            {
                PointD tangent = new PointD(array[i].X - array[i - 1].X, array[i].Y - array[i - 1].Y);
                tangent = tangent / tangent.Magnitude();
                PointD normalVector = PointD.Orthogonal(tangent);

                PointD pointOnLine = new PointD((array[i].X + array[i - 1].X) / 2.0, (array[i].Y + array[i - 1].Y) / 2.0);

                for (int k = 0; k < numLines; k++)
                {
                    PointD testPt = pointOnLine + normalVector * spacing * (k - numLines / 2);
                    m_traceLines[k].Add((PointF)testPt);
                }

                //PointD testNormal1 = (testPt1 - pointOnLine);
                //testNormal1 = testNormal1 / testNormal1.Magnitude();

                //PointD testNormal2 = (testPt2 - pointOnLine);
                //testNormal2 = testNormal2 / testNormal2.Magnitude();

                //PointD scaledVector1 = testNormal1 * (GetScalar() * 7 * m_Param.C / Math.Pow((m_Param.PtRadius / 2.0) + 10, 0.5));
                //PointD scaledVector2 = testNormal2 * (GetScalar() * 7 * m_Param.C / Math.Pow((m_Param.PtRadius / 2.0) + 10, 0.5));

                //drawMethods.DrawArrow(gc, r, testPt1, scaledVector1 * Render.DrawHelper.SPEED_AMPLIFIER);
                //drawMethods.DrawArrow(gc, r, testPt2, scaledVector2 * Render.DrawHelper.SPEED_AMPLIFIER);
            }
        }

        #endregion

    }
}
