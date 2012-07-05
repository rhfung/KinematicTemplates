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
    public class IceSheetFilter : RegionToolRegionFilter
    {
        public override string ToString()
        {
            return "Speed Up";
        }

        protected override Brush GetBrush(Render.RenderParameter r)
        {
            //Image texture = new Bitmap(Properties.Resources.ice);
            //Graphics textureContext = Graphics.FromImage(texture);
            //textureContext.FillRectangle(new SolidBrush(Color.FromArgb(192, Color.White)), textureContext.VisibleClipBounds);
            //textureContext.Dispose();

            //TextureBrush brush = new TextureBrush(texture);
            //return brush;

            return Brushes.Transparent; 
        }

        #region IToolFilter Members


        public override PointD GetVelocity(Position.IVirtualMousePosition m)
        {
            PointD orig = m.GetVelocity();
            return orig * (1 + m_Param.C) * GetStrength(m.GetVirtualPointD());
        }

        public override PointD GetParamHandlePos()
        {
            return PointD.Empty;
        }


        #endregion
    }
}
