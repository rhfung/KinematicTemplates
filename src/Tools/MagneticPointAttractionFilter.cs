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
    public class MagneticPointAttractionFilter : PointToolFilter 
    {
        public MagneticPointAttractionFilter() : this(true)
        {
        }

        public MagneticPointAttractionFilter(bool isAttraction)
        {
            if (isAttraction)
                m_Param.V = new PointD(0, 1);
            else
                m_Param.V = new PointD(0, -1);
        }


        public override string ToString()
        {
            return "Magnetic Point";
        }

        #region IToolFilter Members


        protected override RadialFactors GetInnerVelocity(Position.RadialMousePosition mp)
        {
            if (double.IsNaN(m_Param.V.Y))
                m_Param.V = new PointD(0, 1);
            int dir = -Math.Sign(m_Param.V.Y);
            if (dir == 0)
                dir = -1;

            return new RadialFactors(1, 1, dir * m_Param.C * 8, 0);
            //PointD centre = m_Param.Pt;
            //PointD vectorToPosition = new PointD(m.GetVirtualPointD().X - centre.X, m.GetVirtualPointD().Y - centre.Y);
            //if (vectorToPosition.Magnitude() <= 2.0)
            //    return PointD.Empty;

            //PointD unitVectorToPosition = vectorToPosition / vectorToPosition.Magnitude();
            //    PointD scaledVector = unitVectorToPosition * (dir * 7 * m_Param.C / Math.Pow(vectorToPosition.Magnitude() + 10, 0.5));
            //    return scaledVector;
        }

        public override PointD GetParamHandlePos()
        {
            return PointD.FromSize(m_Param.GetBoundingRect().Size) / 2.0
                - new PointD(0, m_Param.PtRadius / 2.0);
        }


        #endregion
    }
}
