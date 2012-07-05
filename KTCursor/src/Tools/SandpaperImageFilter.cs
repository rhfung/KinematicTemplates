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
    [Obsolete()]
    public class SandpaperImageFilter : RegionToolPathFilter
    {
        Image m_img;
        Point m_offset;
        Tools.Model.VectorPath m_path;

        public SandpaperImageFilter(Image mask)
        {
            m_img = mask;

            m_offset = new Point(0, 0);//Form1.GetInstance().GetCanvasVisibleRect().Location;
            m_Param.C = 0.75;
            m_path = new Tools.Model.VectorPath();
            m_path.AddRectangle(new Rectangle(m_offset, m_img.Size));
            m_Param.Path = m_path;
        }


        public override void DrawRegionRepresentation(System.Drawing.Graphics gc, Render.RenderParameter r, Render.IDrawVisitor drawMethods, PointD mousePosition)
        {
            gc.DrawImage(m_img,m_offset.X,m_offset.Y, m_img.Width,m_img.Height);
        }

        public override string ToString()
        {
            return "Sandpaper (Image Mask)";
        }


        public override PointD GetVelocity(Position.IVirtualMousePosition m)
        {
            Bitmap i = (Bitmap)m_img;
            Color c = i.GetPixel(m.GetVirtualPoint().X - m_offset.X,
                m.GetVirtualPoint().Y - m_offset.Y);
            PointD orig = m.GetVelocity();
            return orig * -m_Param.C * (255 - c.G) / 255.0;
        }

        public override PointD GetParamHandlePos()
        {
            return PointD.Empty;
        }

        public override bool HitTest(PointD point)
        {
            if (m_Param.Path != m_path)
            {
                m_Param.Path = m_path;
            }
            return base.HitTest(point);
        }

    }
}
