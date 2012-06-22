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
    public class SandpaperFilter : RegionToolRegionFilter
    {
        public SandpaperFilter()
        {
            m_Param.C = 0.75;
        }


        public override string ToString()
        {
            return "Slow Down";
        }

        protected override Brush GetBrush(Render.RenderParameter r)
        {
            int size = (int)((1.1 - m_Param.C) * Render.DrawHelper.SPEED_AMPLIFIER);
            Image texture = new Bitmap(size, size);

            Graphics textureContext = Graphics.FromImage(texture);
            textureContext.DrawEllipse(r.RegionGuides, 2, 2, 1, 1);
            textureContext.DrawEllipse(r.RegionGuides, (size + 1) / 2, (size + 1) / 2, 1, 1);
            textureContext.Dispose();

            TextureBrush brush = new TextureBrush(texture);
            return brush;
        }

       
        #region IToolFilter Members


        public override PointD GetVelocity(Position.IVirtualMousePosition m)
        {
            PointD orig = m.GetVelocity();
            return orig * -m_Param.C * GetStrength(m.GetVirtualPointD());
        }

        public override PointD GetParamHandlePos()
        {
            return PointD.Empty;
        }


        #endregion
    }
}
