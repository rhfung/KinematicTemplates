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
    public class SpeedPreservingHatchingFilter : RegionToolPathFilter
    {
        public override string ToString()
        {
            return "Hatching v.2";
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
                    PointD pt1 = refPt + PointD.Orthogonal(m_Param.V) * i * drawMethods.Spacing(m_Param.C);
                    PointD pt2 = pt1 + m_Param.V * rect.Width * rect.Height;
                    PointD pt3 = pt1 - m_Param.V * rect.Width * rect.Height;

                    fill.StartFigure();
                    fill.AddLine((Point)pt2, (Point)pt3);

                }

                GraphicsContainer c = gc.BeginContainer();
                gc.SetClip((Tools.Model.VectorPath)m_Param.Path);
                gc.DrawPath(r.RegionGuides, fill);
                gc.EndContainer(c);

            }
        }

        #region IToolFilter Members


        public override PointD GetVelocity(Position.IVirtualMousePosition m)
        {
            double speed = m.GetVelocity().Magnitude();
            if (speed > 0.01)
            {
                PointD dir = PointD.UnitVector(PointD.DotProduct(m_Param.V, PointD.UnitVector(m.GetVelocity())) * m_Param.V);
                PointD usr = PointD.UnitVector(m.GetVelocity());

                // this will deaden any movement (dir is NULL, usr is NULL :: no movement of the cursor)
                if (dir.Magnitude() < 0.001)
                    usr = dir;

                return -1 * m.GetVelocity()
                    + PointD.UnitVector(m_Param.C * dir + (1 - m_Param.C) * usr) * speed;
            }
            else
            {
                return -1 * m.GetVelocity();
            }
        }

        public override PointD GetParamHandlePos()
        {
            return PointD.FromSize(m_Param.Path.GetBounds().Size) / 2.0;
        }


        #endregion
    }
}
