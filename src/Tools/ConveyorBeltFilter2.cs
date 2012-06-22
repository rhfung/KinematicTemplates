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
    public class ConveyorBeltFilter2 : RegionToolPathFilter
    {
        public override string ToString()
        {
            return "Conveyor Belt v.2";
        }

        #region IToolFilter Members


        public override PointD GetVelocity(Position.IVirtualMousePosition m)
        {
            if (m.GetDistance() > 0.1)
            {
                PointD dir = m_Param.V;
                return new PointD(dir.X * m_Param.C, dir.Y * m_Param.C);
            }
            else
            {
                return PointD.Empty;
            }

        }

        public override void DrawRegionRepresentation(System.Drawing.Graphics gc, Render.RenderParameter r, Render.IDrawVisitor drawMethods, PointD mousePosition)
        {
            RectangleF bounds = m_Param.Path.GetBounds();
            for (double i = bounds.Width / (Render.DrawHelper.X_ARROWS + 1); i < bounds.Width; i += bounds.Width / (Render.DrawHelper.X_ARROWS + 1))
            {
                for (double j = bounds.Height / (Render.DrawHelper.Y_ARROWS + 1); j < bounds.Height; j += bounds.Height / (Render.DrawHelper.Y_ARROWS + 1))
                {
                    Point thePoint = new Point((int)(bounds.Left + i), (int)(bounds.Top + j));
                    if (m_Param.Path.IsVisible(thePoint))
                    {
                        Position.VirtualMousePosition mouse = new Position.VirtualMousePosition((Point)thePoint);
                        PointD vector = GetVelocity(mouse) * Render.DrawHelper.SPEED_AMPLIFIER * (m_Param.C + 0.1);
                        drawMethods.DrawArrow(gc, r, (PointD)thePoint, vector);
                    }
                }

            }
        }

        public override PointD GetParamHandlePos()
        {
            return PointD.FromSize(m_Param.Path.GetBounds().Size) / 2.0;
        }

        #endregion
    }
}
