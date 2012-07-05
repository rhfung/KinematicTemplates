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
    public class OrbitFilter : PointToolFilter
    {
        public override string ToString()
        {
            return "Orbit";
        }
        #region IToolFilter Members

        protected override RadialFactors GetInnerVelocity(Position.RadialMousePosition mp)
        {
            int signedDirection = Math.Sign(m_Param.V.X);
            if (signedDirection == 0) signedDirection = 1;

            // this formula keeps a constant speed to the orbital radius
            // to match the speed of the conveyor belt template
            // 
            // the length of the arc is m_Param.C * mp.TimeInterval
            // the angle is the ratio of the arc to the radius to the arc's pivot
            return new RadialFactors(1, 1, 0, signedDirection * m_Param.C * mp.TimeInterval / mp.VirtualPoint.R);

            //PointD centre = m_Param.Pt;
            //PointD vectorToPosition = new PointD(m.GetVirtualPointD().X - centre.X , m.GetVirtualPointD().Y - centre.Y );
            //if (vectorToPosition.Magnitude() <= Render.DrawHelper.MIN_SPEED_SENSITIVITY)
            //    return PointD.Empty;
            
            ///*
            //// getting the tangent to the point will put the point off of the curve
            //PointD unitVectorToPosition = vectorToPosition / vectorToPosition.Magnitude();
            //PointD tangential = PointD.Orthogonal(unitVectorToPosition);
            //// so we pretend to put that point there and calculate the vector to new position
            //PointD newPoint = m.GetVirtualPointD() + tangential * m.GetTimeInterval();
            //PointD newPointVector = newPoint - centre;
            //// and place the point back on the curve
            //PointD unitNewVector = newPointVector / newPointVector.Magnitude() * vectorToPosition.Magnitude();
            //PointD newPointOnCurve = unitNewVector + centre;
            //// and then we know the actual velocity, which is slightly off the tangent
            //return (newPointOnCurve - m.GetVirtualPointD()) / m.GetTimeInterval();
            // */
            //PointD orthogonalUnit = vectorToPosition / vectorToPosition.Magnitude();
            //PointD tangentialUnit = PointD.Orthogonal(orthogonalUnit);

            //PointD orthogonalVel = PointD.DotProduct(orthogonalUnit, m.GetVelocity()) * orthogonalUnit;
            //PointD tangentialVel = PointD.DotProduct(tangentialUnit, m.GetVelocity()) * tangentialUnit;

            //double tangentialSpeed = tangentialVel.Magnitude();
            //PointD addTangent = PointD.Empty;

            //if (tangentialSpeed < Math.Sqrt(vectorToPosition.Magnitude() * m_Param.C / 10.0))
            //{
            //    tangentialSpeed = Math.Sqrt(vectorToPosition.Magnitude() * m_Param.C / 10.0);
            //}

            //    //if (Math.Sign(PointD.DotProduct(tangentialUnit, m.GetVelocity())) != 0)
            //    //{
            //    //    m_previousSign = Math.Sign(PointD.DotProduct(tangentialUnit, m.GetVelocity()));
            //    //}
            //addTangent = signedDirection * (tangentialSpeed - tangentialVel.Magnitude()) * tangentialUnit;

            //// so we pretend to put that point there and calculate the vector to new position
            //PointD newPoint = m.GetVirtualPointD() + addTangent * m.GetTimeInterval();
            //PointD newPointVector = newPoint - centre;
            //// and place the point back on the curve
            //if (newPointVector.Magnitude() <= Render.DrawHelper.MIN_SPEED_SENSITIVITY)
            //    return PointD.Empty;
            //PointD unitNewVector = newPointVector / newPointVector.Magnitude() ;
            //PointD newPointOnCurve = unitNewVector * vectorToPosition.Magnitude()  + centre;
            //// and then we know the actual velocity, which is slightly off the tangent
            //if (m.GetTimeInterval() <= 0)
            //    return PointD.Empty;
            //addTangent = (newPointOnCurve - m.GetVirtualPointD()) / m.GetTimeInterval();
            //return addTangent; //-1 * PointD.DotProduct(addTangent - m.GetVelocity(), unitNewVector) * unitNewVector;
        
        }

        public override PointD GetParamHandlePos()
        {
            return PointD.FromSize(m_Param.GetBoundingRect().Size) / 2.0
                - new PointD(0, m_Param.PtRadius / 2.0);
        }


        #endregion

    }
}
