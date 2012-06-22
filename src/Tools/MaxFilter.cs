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
    public class MaxFilter : RegionToolRegionFilter
    {
        public override string ToString()
        {
            return "Maximum Speed";
        }

        protected override Brush GetBrush(Render.RenderParameter r)
        {
            return Brushes.Transparent; // new SolidBrush( Color.Yellow);
        }

        #region IToolFilter Members

        public override PointD GetVelocity(Position.IVirtualMousePosition m)
        {
            // velocity restriction
            double speed = m.GetSpeed();
            double speedLimit = m_Param.C  * 10 * GetStrength(m.GetVirtualPointD());
            if (speed > speedLimit)
            {
                PointD maxParts = new PointD(speedLimit * m.GetUnitDirection().X, speedLimit * m.GetUnitDirection().Y);
                return -1 * m.GetVelocity() + maxParts;
            }
            else
            {
                return PointD.Empty;
            }

        }

        public override PointD GetParamHandlePos()
        {
            return PointD.Empty;
        }

        #endregion
    }
}
