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

namespace KinTemplates.Cursor.Position
{
    public class ParameterizedMousePosition : IVirtualMousePosition
    {
        private readonly IVirtualMousePosition m_mouse;
        private readonly PointD m_newAcceleration;
        private long m_fakeTime;
        private PointD m_newPosition;
        private PointD m_newVelocity;
        private PointD m_newDisplacement;

        #region IVirtualMousePosition Members

        public ParameterizedMousePosition(Position.IVirtualMousePosition mousePos)
        {
            m_mouse = mousePos;
            m_newPosition = mousePos.GetVirtualPointD();
            m_newAcceleration = mousePos.GetAcceleration();
            m_newVelocity = mousePos.GetVelocity();
            m_newDisplacement = m_mouse.GetDisplacement();
            m_fakeTime = mousePos.GetTimeInterval();
        }

        public ParameterizedMousePosition(Position.IVirtualMousePosition mousePos, int fakeTime)
        {
            m_mouse = mousePos;
            m_newPosition = mousePos.GetVirtualPointD();
            m_newAcceleration = mousePos.GetAcceleration();
            m_newVelocity = mousePos.GetVelocity();
            m_newDisplacement = m_mouse.GetDisplacement();
            m_fakeTime = fakeTime;
        }

        /*
        public ParameterizedMousePosition(Position.IVirtualMousePosition mousePos, PointD newVirtualPosition, PointD newVelocity, PointD newAcceleration)
        {
            m_mouse = mousePos;
            m_newPosition = newVirtualPosition;
            m_newAcceleration = newAcceleration;
            m_newVelocity = newVelocity;
         * m_newDisplacement = m_mouse.GetDisplacement();
            m_fakeTime = mousePos.GetTimeInterval();
        }*/


        public ParameterizedMousePosition(Position.IVirtualMousePosition mousePos, PointD newVelocity, PointD newAcceleration)
        {
            m_mouse = mousePos;
            m_newPosition = mousePos.GetVirtualPointD();
            m_newAcceleration = newAcceleration;
            m_newVelocity = newVelocity;
            m_newDisplacement = m_mouse.GetDisplacement();
            m_fakeTime = mousePos.GetTimeInterval();
        }

        public long GetTimeInterval()
        {
            return m_fakeTime;
        }

        public void SetTimeInterval(long newTimeInterval)
        {
            m_fakeTime = newTimeInterval;
        }

        public long GetTimeStamp()
        {
            return m_mouse.GetTimeStamp();
        }

        public System.Drawing.Point GetLastPhysicalPoint()
        {
            return m_mouse.GetLastPhysicalPoint();
        }

        public System.Drawing.Point GetPhysicalPoint()
        {
            return m_mouse.GetPhysicalPoint();
        }

        public System.Drawing.Point GetLastVirtualPoint()
        {
            return m_mouse.GetLastVirtualPoint();
        }

        public System.Drawing.Point GetVirtualPoint()
        {
            return (System.Drawing.Point) m_newPosition;
        }

        public void SetVirtualPointD(PointD newPoint)
        {
            m_newPosition = newPoint;
        }

        public PointD GetDisplacement()
        {
            return m_newDisplacement;
        }

        public void SetDisplacement(PointD newDisplacement)
        {
            m_newDisplacement = newDisplacement;
        }


        public double GetDistance()
        {
            return m_newDisplacement.Magnitude(); //m_mouse.GetDistance();
        }

        public PointD GetUnitDirection()
        {
            return m_newDisplacement / m_newDisplacement.Magnitude();
        }

        public PointD GetLastVirtualPointD()
        {
            return m_mouse.GetLastVirtualPointD();
        }

        public PointD GetVirtualPointD()
        {
            return m_newPosition;
        }

        public double GetSpeed()
        {
            return m_newVelocity.Magnitude();
        }

        public PointD GetVelocity()
        {
            return m_newVelocity;
        }

        public void SetVelocity(PointD newVelocity)
        {
            System.Diagnostics.Debug.Assert(newVelocity.X != double.NaN && newVelocity.Y != double.NaN);
             System.Diagnostics.Debug.Assert(newVelocity.X > -10000 && newVelocity.X < 100000
                && newVelocity.Y > -10000 && newVelocity.Y < 10000);

            if ((newVelocity.X > -10000 && newVelocity.X < 100000
                && newVelocity.Y > -10000 && newVelocity.Y < 10000))
            {
                m_newVelocity = newVelocity;
            }
            else
            {
                m_newVelocity = new PointD(0,0);
            }
        }

        public PointD GetAcceleration()
        {
            return m_newAcceleration;
        }

        #endregion
    }
}
