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

namespace KinTemplates.Cursor.Position
{
    public class VirtualMousePosition : IVirtualMousePosition 
    {
        protected Point m_StartPos;       // physical position
        protected Point m_LastPhysicalPos;// physical position
        protected Point m_CurPhysicalPos;

        protected PointD m_TrackingVPos;   // virtual position
        protected PointD m_LastVPos;       // virtual position

        protected PointD m_LastVelocity;
        protected long m_LastTime = 0;       // milliseconds
        protected long m_TimeInterval = 0;   // milliseconds
        protected HiPerfTimer m_Timer;

        public VirtualMousePosition(System.Drawing.Point startPosition)
        {
            m_StartPos = startPosition;

            // real position
            m_LastPhysicalPos = startPosition;
            m_CurPhysicalPos = startPosition;

            // virtual position
            m_TrackingVPos = new PointD( startPosition.X, startPosition.Y );
            m_LastVPos = new PointD(startPosition.X, startPosition.Y);

            m_LastTime = Environment.TickCount;
            m_Timer = new HiPerfTimer();
            m_Timer.Start();
        }


        public virtual bool SetPhysicalPoint(System.Drawing.Point physicalPosition)
        {

            int curTime = Environment.TickCount;
            m_Timer.Stop();
            // block division conditions that lead to zero m_TimeInterval (see division line below)
            if (m_Timer.DurationInterval < 10000)
            {
                m_Timer.Start();
                return false;
            }
            else
            {
                //m_LastVelocity = new PointD(
                //    GetVelocity().X + 1.0 / 2.0 * GetAcceleration().X * GetTimeInterval(),
                //    GetVelocity().Y + 1.0 / 2.0 * GetAcceleration().Y * GetTimeInterval());
                m_LastVelocity = GetVelocity();

                m_LastPhysicalPos = m_CurPhysicalPos;
                m_CurPhysicalPos = physicalPosition;

                m_TimeInterval = m_Timer.DurationInterval / 10000;
                m_LastTime = curTime;
                m_Timer.Start();

                return true;
            }
        }

        public virtual void SetVirtualPoint(PointD virtualPosition)
        {
            System.Diagnostics.Debug.Assert(virtualPosition.X != double.NaN && virtualPosition.Y != double.NaN);
            m_LastVPos = m_TrackingVPos;
            m_TrackingVPos = virtualPosition;
        }

        public long GetTimeInterval()
        {
            return m_TimeInterval;
        }

        public long GetTimeStamp()
        {
            return m_LastTime;
        }

        protected static double Distance(Point firstPoint, Point secondPoint)
        {
            return Math.Sqrt((firstPoint.X - secondPoint.X) * (firstPoint.X - secondPoint.X) +
                             (firstPoint.Y - secondPoint.Y) * (firstPoint.Y - secondPoint.Y));
        }

        protected static double Distance(PointD firstPoint, PointD secondPoint)
        {
            return Math.Sqrt((firstPoint.X - secondPoint.X) * (firstPoint.X - secondPoint.X) +
                             (firstPoint.Y - secondPoint.Y) * (firstPoint.Y - secondPoint.Y));
        }

        public double GetSpeed()
        {
            if (GetTimeInterval() < 1)
            {
                return 0;
            }
            else
            {
                return GetDistance() / GetTimeInterval();
            }
        }

        public PointD GetVelocity()
        {
            return new PointD(  GetSpeed() * GetUnitDirection().X,
                                GetSpeed() * GetUnitDirection().Y);

        }

        public double GetDistance()
        {
            return Distance(m_CurPhysicalPos, m_LastPhysicalPos);
        }

        public PointD GetUnitDirection()
        {
            if (GetDistance() < 0.00001)
            {
                return new PointD(0, 0);
            }
            else
            {
                return new PointD(  GetDisplacement().X / GetDistance(),
                                    GetDisplacement().Y / GetDistance());
            }
        }

        public PointD GetDisplacement()
        {
            return new PointD(m_CurPhysicalPos.X - m_LastPhysicalPos.X,
                              m_CurPhysicalPos.Y - m_LastPhysicalPos.Y);
        }

        public PointD GetAcceleration()
        {
            PointD vel = GetVelocity();
            if (GetTimeInterval() < 1)
            {
                return new PointD(0, 0);
            }
            else
            {
                double deltaSpeed = vel.Magnitude() - m_LastVelocity.Magnitude();
                double magnitudeVel = Distance(vel, m_LastVelocity);
                PointD unitAccel;
                if (Math.Abs( magnitudeVel) > 0.000001)
                    unitAccel = new PointD( (vel.X - m_LastVelocity.X) / magnitudeVel, 
                                            (vel.Y - m_LastVelocity.Y) / magnitudeVel);
                else
                    unitAccel = new PointD(0,0);
                    
                return new PointD(unitAccel.X * deltaSpeed / GetTimeInterval(), 
                                  unitAccel.Y * deltaSpeed / GetTimeInterval()) ;
            }
        }

        public System.Drawing.Point GetLastPhysicalPoint()
        {
            return m_LastPhysicalPos ;
        }

        public System.Drawing.Point GetPhysicalPoint()
        {
            return m_CurPhysicalPos;
        }

        public System.Drawing.Point GetLastVirtualPoint()
        {
            return new Point( (int) (m_LastVPos.X ), (int) (m_LastVPos.Y ));
        }

        public System.Drawing.Point GetVirtualPoint()
        {
            return new Point( (int) (m_TrackingVPos.X ),(int) ( m_TrackingVPos.Y));
        }

        public PointD GetLastVirtualPointD()
        {
            return m_LastVPos;
        }

        public PointD GetVirtualPointD()
        {
            return m_TrackingVPos;
        }
    }
}
