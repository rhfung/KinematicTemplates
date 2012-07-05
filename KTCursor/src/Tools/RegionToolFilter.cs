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
    public abstract class RegionToolFilter : IToolFilter
    {
        protected Model.ToolParameter m_Param = new Model.RegionToolParameter(Model.PathShape.Rectangle);
        private int m_uid;

        #region IToolFilter Members

        // each filter implements these methods

        public abstract PointD GetVelocity(Position.IVirtualMousePosition m);
        public abstract PointD GetParamHandlePos();

        // 

        public abstract bool HitTest(PointD test);
        public abstract void DrawOutline(System.Drawing.Graphics gc, Render.RenderParameter r);
        public abstract void Draw(System.Drawing.Graphics gc, Render.RenderParameter r, Render.RenderHint editState, Render.IDrawVisitor drawMethods, PointD mousePosition);

        public abstract Tools.Model.IPath GetNewPath();

        // default implementation for these rendering methods

        public RegionToolFilter()
        {
            m_uid = FilterUID.GetNewUID();
        }

        public int UID
        {
            get
            {
                return m_uid;
            }
        }

        public virtual void DrawRegionRepresentation(System.Drawing.Graphics gc, Render.RenderParameter r, Render.IDrawVisitor drawMethods, PointD mousePosition)
        {
            gc.DrawString(this.ToString(), r.FontType, new SolidBrush(r.RegionGuides.Color), m_Param.Path.GetLastPoint());
            // do nothing here
        }

        // do not allow overriding of these methods

        public Model.ToolParameter  Parameters
        {
            get
            {
                return m_Param;
            }

            set
            {
                m_Param = value;
            }

        }

        public virtual void ResetState()
        {
        }

        public void Cache(Render.IDrawVisitor drawMethods)
        {
        }

        #endregion
    }
}
