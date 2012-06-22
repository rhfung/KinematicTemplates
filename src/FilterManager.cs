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
//using System.Text;

namespace KinTemplates.Cursor
{
    // FilterManager decides which templates should modify the cursor's movement
    public class FilterManager
    {
        private List<Tools.IToolFilter> m_Filters ;
        private PointD m_computedAcceleration;
        private PointD m_computedVelocity;
        private Tools.Render.OnlineFeedback m_ri = null;
        private int m_maxNumInUse = 0;

        public FilterManager()
        {
            m_Filters = new List<KinTemplates.Cursor.Tools.IToolFilter>();
        }

        public void ClearFilters()
        {
            m_Filters.Clear();
        }


        public void ResetState()
        {
            foreach (Tools.IToolFilter filter in m_Filters)
            {
                filter.Parameters.InUse = false;
                filter.Parameters.UseCounter = 0;
                filter.ResetState();
            }
            // TODO: not sure what does this do here.
            // Need to see the call log usage.
            m_maxNumInUse = 0;
        }

        public void Compute(Position.IVirtualMousePosition mousePosition, out bool filtersChanged)
        {
            int i = 0;
            PointD sumVel = mousePosition.GetVelocity();
            PointD sumAccel = mousePosition.GetAcceleration();
            
            PointD displ = mousePosition.GetDisplacement();
            Position.ParameterizedMousePosition mp = new Position.ParameterizedMousePosition(mousePosition);

            int numInUse = 0;// instrumentation
            filtersChanged = false; // flag out if filter state changes

            for (i = 0; i < m_Filters.Count; i++)
            {
                if (m_Filters[i].Parameters.FilterEnabled )
                {
                    if (m_Filters[i].HitTest(mousePosition.GetVirtualPointD()))
                    {
                        if (!m_Filters[i].Parameters.InUse)
                            m_Filters[i].Parameters.UseCounter++;

                        if (!m_Filters[i].Parameters.InUse)
                            filtersChanged = true;

                        m_Filters[i].Parameters.InUse = true;
                        numInUse++;// instrumentation
                        
                        PointD vel = m_Filters[i].GetVelocity(mp);
                        if (!(double.IsNaN(vel.X) || double.IsInfinity(vel.X)
                            || double.IsNaN(vel.Y) || double.IsNaN(vel.Y)))
                        {
                            sumVel += vel;
                            mp.SetVelocity(sumVel);
                        }

                    }
                    else
                    {
                        if (m_Filters[i].Parameters.InUse)
                            filtersChanged = true;

                        m_Filters[i].Parameters.InUse = false;
                    }
                }
            }
            m_maxNumInUse = Math.Max(numInUse, m_maxNumInUse); // instrumentation
            m_computedAcceleration = sumAccel;
            m_computedVelocity = sumVel;
        }

        [Obsolete()]
        public PointD GetAcceleration()
        {
            return m_computedAcceleration;
        }

        public PointD GetVelocity()
        {
            return m_computedVelocity;
        }


        public void DrawOutline(System.Drawing.Graphics gc, Tools.Render.RenderParameter rp, bool mousePressedDown)
        {

            foreach (Tools.IToolFilter filter in m_Filters)
            {
                if ((filter.Parameters.FilterVisible || !mousePressedDown) && filter.Parameters.FilterEnabled)
                {
                    filter.DrawOutline(gc, rp);
                }
            }


        }

        public void Draw(System.Drawing.Graphics gc, Tools.Render.RenderParameter rp, Tools.Render.RenderHint state, Tools.Render.IDrawVisitor drawMethods, PointD mousePosition, bool mousePressedDown)
        {

            foreach (Tools.IToolFilter filter in m_Filters)
            {
                if ((filter.Parameters.FilterVisible || !mousePressedDown) && filter.Parameters.FilterEnabled)
                {
                    filter.Draw(gc, rp, state, drawMethods, mousePosition);
                }
            }
        }

        public static void DrawTemplates(List<Tools.IToolFilter> templates, System.Drawing.Graphics gc, Tools.Render.RenderParameter rp, Tools.Render.RenderHint state, Tools.Render.IDrawVisitor drawMethods, PointD mousePosition)
        {
            foreach (Tools.IToolFilter filter in templates)
            {
                filter.Draw(gc, rp, state, drawMethods, mousePosition);
            }
    }


        public Tools.Render.OnlineFeedback GetOnlineFeedback(Tools.Render.FeedbackParameter r, PointD mousePosition)
        {
            if (m_ri == null)
                m_ri = new Tools.Render.OnlineFeedback();
            else
                m_ri.ClearFeedback();
            foreach (Tools.IToolFilter filter in m_Filters)
            {
                if (filter.Parameters.FilterEnabled)
                   m_ri.AddFilter(filter, r, mousePosition);
            }
            return m_ri;
        }

        public Tools.Render.StaticFeedback GetStaticFeedback(Tools.Render.FeedbackParameter r)
        {
            Tools.Render.StaticFeedback ri = new Tools.Render.StaticFeedback();
            foreach (Tools.IToolFilter filter in m_Filters)
            {
                if (filter.Parameters.FilterEnabled && filter.Parameters.FilterVisible)
                    ri.AddFilter(filter, r);
            }
            return ri;
        }

        public IList<Tools.IToolFilter> Filters
        {
            get
            {
                return m_Filters;
            }
        }

    }
}
