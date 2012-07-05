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

namespace KinTemplates.Cursor.Logger
{
    public class MouseLogger : IMouseLogger
    {
        public const int TAIL_LENGTH = 16384;

        // physical info
        private PointD m_startPt;
        private List<PointD> m_realDelta;

        // virtual info
        private List<PointD> m_virtualDelta;
        #region IMouseLogger Members

       public MouseLogger()
       {
       }

        // called very frequently
        public void MouseMoved(Position.VirtualMousePosition mouseMove)
        {
            m_virtualDelta.Add(mouseMove.GetVirtualPointD() - mouseMove.GetLastVirtualPointD());
            m_realDelta.Add(new PointD (mouseMove.GetPhysicalPoint().X - mouseMove.GetLastPhysicalPoint().X,
                                        mouseMove.GetPhysicalPoint().Y - mouseMove.GetLastPhysicalPoint().Y));
            if (m_virtualDelta.Count > MouseLogger.TAIL_LENGTH)
            {
                // BUG: sometimes mouseMoved gets fired with no movement of the mouse,
                // flooding the event queue with many points of data estimated to be
                // > 40,000 points. Remove first half of the point data when such a 
                // condition may occur.
                m_virtualDelta.RemoveRange(0, MouseLogger.TAIL_LENGTH / 2);
                m_realDelta.RemoveRange(0, MouseLogger.TAIL_LENGTH / 2); 
            }
        }

        public void MouseDown(Position.VirtualMousePosition mouseMove)
        {
            m_startPt = (PointD) mouseMove.GetPhysicalPoint();
            m_virtualDelta = new List<PointD>();
            m_realDelta = new List<PointD>();
            
            // initial movement is nothing
            m_virtualDelta.Add(PointD.Empty);
            m_realDelta.Add(PointD.Empty);
        }

        public int QueuedEvents
        {
            get
            {
                return m_virtualDelta.Count;
            }
        }

        public PointD[] GetQueueItems()
        {
            return m_virtualDelta.ToArray();
        }

        private static string GetInputPath(PointD startPoint, List<PointD> deltaList)
        {
            // see also: RenderPath.GetPathString

            // 000.00,000.00
            StringBuilder inputPts = new StringBuilder(deltaList.Count * 15);
            PointD ptLoc = (PointD)startPoint;
            foreach (PointD delta in deltaList)
            {
                ptLoc += (PointD)delta;

                // format string 0 - required, # - optional placeholder
                inputPts.Append(ptLoc.X.ToString("#0.##"));
                inputPts.Append(",");
                inputPts.Append(ptLoc.Y.ToString("#0.##"));
                inputPts.Append(" ");
            }

            return inputPts.ToString();
        }

        public string GetRealInputPath()
        {
            return GetInputPath(m_startPt, m_realDelta);
        }

        public string GetVirtualOutputPath()
        {
            return GetInputPath(m_startPt, m_virtualDelta);
        }



        public void Stop()
        {
            // happens when the application quits 
        }



        public void MouseUp(Position.VirtualMousePosition mouseMove)
        {
        }

        #endregion

    }
}
