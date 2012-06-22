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

namespace KinTemplates.Cursor.Tools
{
    public class SteadyHandFilter : RegionToolPathFilter
    {
        public SteadyHandFilter()
        {
            m_Param.C = 1.0;
        }

        public override string ToString()
        {
            return "Steady Hand";
        }

        public override void DrawRegionRepresentation(System.Drawing.Graphics gc, Render.RenderParameter r, Render.IDrawVisitor drawMethods, PointD mousePosition)
        {
            if (m_Param.Path.IsVisible((System.Drawing.Point)mousePosition))
            {
                gc.DrawString(ToString(), r.FontType, new System.Drawing.SolidBrush(r.RegionGuides.Color), new System.Drawing.PointF((float)mousePosition.X, (float)mousePosition.Y - 15));
            }
        }

        #region IToolFilter Members

        PointD m_direction;  // should just use V instead


        public override PointD GetVelocity(Position.IVirtualMousePosition m)
        {
            PointD direction = m.GetUnitDirection();
            if (direction.Magnitude() > 0.1)
            {
                m_direction = direction * 0.1 + m_direction * 0.9; // mixture of previous and current input
            }

            if (m_direction.Magnitude() > Render.DrawHelper.MIN_SPEED_SENSITIVITY)
            {
                m_direction = m_direction / m_direction.Magnitude(); // unit vector
                m_Param.V = m_direction; // direction stored internally, but exposed 
                PointD velParallel;

                // prevent motion retracing the path (going in opposite direction)
                if (PointD.DotProduct(m_direction, m.GetVelocity()) > 0)
                {
                    velParallel = PointD.DotProduct(m_direction, m.GetVelocity()) * m_direction;
                }
                else
                {
                    // curving opposite motion slightly until more direction events received 
                    // (how much velocity to go backwards)
                    velParallel = PointD.DotProduct(m_direction, m.GetVelocity()) * -(1 - m_Param.C) * m_direction;
                    //m_direction = -0.5 * m_direction;
                    //velParallel = PointD.DotProduct(m_direction, m.GetVelocity()) * m_direction;// PointD.DotProduct(m_direction, m.GetVelocity()) * -(1 - m_Param.C) * m_direction;
                }

                PointD origVelocityTangent = PointD.DotProduct(m_direction, m.GetVelocity()) * m_direction;
                PointD origVelocityNormal = PointD.DotProduct(PointD.Orthogonal(m_direction), m.GetVelocity()) * PointD.Orthogonal(m_direction);

                // first cancel out the original velocity, then put the new one in
                return -1 * origVelocityTangent + -m_Param.C * origVelocityNormal + velParallel;
            }
            else
            {
                return PointD.Empty;
            }
        }

        public override void ResetState()
        {
            m_direction = PointD.Empty;
            m_Param.V = PointD.Empty; 
        }

        public override PointD GetParamHandlePos()
        {
            return PointD.Empty;
        }


        #endregion

    }
}
