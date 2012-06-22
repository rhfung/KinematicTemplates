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
    public class RubberBandFilter : PointToolFilter
    {
        public override string ToString()
        {
            return "Rubber Band";
        }

        #region IToolFilter Members

        // bool m_previouslyIn;

        public  int GetScalar()
        {
            return -1;
        }


        protected override RadialFactors GetInnerVelocity(Position.RadialMousePosition mp)
        {
            // System.Diagnostics.Debug.Print("{0} {1}\n", mp.VirtualPoint.R, m_Param.PtRadius);
            return new RadialFactors(1, 1, -(10 * m_Param.C) * Math.Pow(mp.VirtualPoint.R / m_Param.PtRadius, 2)   , 0);
            //PointD centre = m_Param.Pt;
            //double outerRadius = m_Param.PtRadius;
            //double innerRadius = (1 - m_Param.C) * outerRadius;

            //PointD vectorToPosition = new PointD(m.GetVirtualPointD().X - centre.X, m.GetVirtualPointD().Y - centre.Y);
            //PointD unitVectorToPosition = vectorToPosition / vectorToPosition.Magnitude();
            //if (vectorToPosition.Magnitude() > innerRadius)
            //{
            //    PointD scaledVector = PointD.Empty;

            //    //if ((vectorToPosition.Magnitude() >= outerRadius) && m_previouslyIn)
            //    //{
            //    //    scaledVector = GetScalar() * 0.5 * unitVectorToPosition * m.GetVelocity().Magnitude();
            //    //}
            //    if (m_previouslyIn && (vectorToPosition.Magnitude() >= innerRadius))
            //    {
            //        scaledVector = GetScalar() * m_Param.C * unitVectorToPosition * (vectorToPosition.Magnitude() - innerRadius) / (outerRadius - innerRadius);

            //    }
            //    m_previouslyIn = m_previouslyIn || (vectorToPosition.Magnitude() < outerRadius);
            //    return scaledVector;
            //}
            //else 
            //{
            //    m_previouslyIn = m_previouslyIn || (vectorToPosition.Magnitude() < outerRadius);
            //    return PointD.Empty;
            //}
        }


        public override void ResetState()
        {
            
        }


        public override PointD GetParamHandlePos()
        {
            return PointD.Empty;
        }


        #endregion

    }
}
