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
    public class InertiaFilter : RegionToolPathFilter
    {
        private PointD m_previousVelocity;

        public override string ToString()
        {
            return "Inertia";
        }

        public override void DrawRegionRepresentation(System.Drawing.Graphics gc, Render.RenderParameter r, Render.IDrawVisitor drawMethods, PointD mousePosition)
        {
            if (m_Param.Path != null)
            {
                if (m_Param.Path.IsVisible((Point)mousePosition))
                {
                    gc.DrawString("+ speed", r.FontType, new SolidBrush(r.RegionGuides.Color), new PointF((float)mousePosition.X, (float)mousePosition.Y - 15));
                }
            }
            else
            {
                gc.DrawString("+ speed", r.FontType, new SolidBrush(r.RegionGuides.Color), new PointF((float)mousePosition.X, (float)mousePosition.Y - 15));
            }
        }

        #region IToolFilter Members


        public override  PointD GetVelocity(Position.IVirtualMousePosition m)
        {
            m_previousVelocity = m_previousVelocity * ( 1 - Math.Pow(m_Param.C - 1, 2))  +  m.GetVelocity() * 0.3;
            return m_previousVelocity;
        }

        public override void ResetState()
        {
            m_previousVelocity = PointD.Empty;
        }

        public override PointD GetParamHandlePos()
        {
            return PointD.Empty;
        }


        //public override bool HitTest(PointD point)
        //{
        //    if (m_Param.Path == null)
        //    {
        //        return true; //  this one is special case that works without 
        //    }
        //    else
        //    {
        //        return m_Param.Path.IsVisible((Point)point);
        //    }
        //}


        #endregion
    }
}
