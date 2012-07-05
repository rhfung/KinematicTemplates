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
    public class GridFilter : RegionToolPathFilter 
    {
        public GridFilter()
        {
            m_Param.V = new PointD(0, 1);
        }

        public override string ToString()
        {
            return "Grid";
        }

        public override void DrawRegionRepresentation(Graphics gc, Render.RenderParameter r, Render.IDrawVisitor drawMethods, PointD mousePosition)
        {
            if (m_Param.Path.PointCount > 0)
            {
                GraphicsPath fill = new GraphicsPath();
                RectangleF rect = m_Param.Path.GetBounds();
                PointD refPt = (PointD)rect.Location + ((PointD)rect.Size.ToPointF()) / 2;
                // this will draw beyond the shape's location
                for (double i = -rect.Height; i < rect.Height; i++)
                {
                    PointD orth = PointD.Orthogonal(m_Param.V);
                    PointD pt1 = refPt + orth * i * drawMethods.Spacing(m_Param.C);
                    PointD pt2 = pt1 + m_Param.V * rect.Width * rect.Height;
                    PointD pt3 = pt1 - m_Param.V * rect.Width * rect.Height;

                    PointD pt4 = refPt + m_Param.V * i * drawMethods.Spacing(m_Param.C);
                    PointD pt5 = pt4 + orth * rect.Width * rect.Height;
                    PointD pt6 = pt4 - orth * rect.Width * rect.Height;

                    fill.StartFigure();
                    fill.AddLine((Point)pt2, (Point)pt3);

                    fill.StartFigure();
                    fill.AddLine((Point)pt5, (Point)pt6);

                }

                GraphicsContainer c = gc.BeginContainer();
                gc.SetClip( (Tools.Model.VectorPath) m_Param.Path);
                gc.DrawPath(r.RegionGuides, fill);
                gc.EndContainer(c);

            }
        }

        #region IToolFilter Members

        public override PointD GetVelocity(Position.IVirtualMousePosition m)
        {
            PointD dir = m_Param.V;
            PointD normalComponent = PointD.DotProduct(dir, m.GetVelocity()) * dir;
            PointD tangentComponent = m.GetVelocity() - normalComponent;

            if (normalComponent.Magnitude()  > tangentComponent.Magnitude())
            {
                return -1 * tangentComponent * m_Param.C;
            }
            else
            {
                return -1 * normalComponent * m_Param.C;
            }

            

            //if (Math.Abs(m.GetVelocity().X ) > Math.Abs(m.GetVelocity().Y))
            //{
            //    return new PointD(0, -m.GetVelocity().Y * .5);
            //}
            //else if (Math.Abs(m.GetVelocity().X) < Math.Abs(m.GetVelocity().Y))
            //{
            //    return new PointD(-m.GetVelocity().X * .5, 0);
            //}
            //else
            //{
            //    return PointD.Empty;
            //}
        }

        public override PointD GetParamHandlePos()
        {
            return PointD.FromSize(m_Param.Path.GetBounds().Size) / 2.0;
        }


        #endregion
    }
}
