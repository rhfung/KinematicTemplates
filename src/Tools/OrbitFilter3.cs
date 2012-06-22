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
    public class OrbitFilter3 : PointToolFilter
    {
        public override string ToString()
        {
            return "Orbit v.3";
        }
        #region IToolFilter Members

        protected override RadialFactors GetInnerVelocity(Position.RadialMousePosition mp)
        {
            int signedDirection = Math.Sign(m_Param.V.X);
            if (signedDirection == 0) signedDirection = 1;

            //int scaleRadius = 0;
            //if (mp.VirtualPoint.Theta < 0)
            //    scaleRadius = -1;
            //else
            //    scaleRadius = 1;

            //return new RadialFactors(scaleRadius, 0, 0, signedDirection * m_Param.C / 4);

            return new RadialFactors(0, 0, 0, signedDirection * m_Param.C / 4);

        }

        public override PointD GetParamHandlePos()
        {
            return PointD.FromSize(m_Param.GetBoundingRect().Size) / 2.0
                - new PointD(0, m_Param.PtRadius / 2.0);
        }
    }
        #endregion
}
