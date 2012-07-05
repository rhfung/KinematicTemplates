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
using System.Drawing.Drawing2D;

namespace KinTemplates.Cursor.Tools.Model
{
    public class AspectRatioToolParameter : ToolParameter
    {
        private PointD m_ctrPt;
        private PointD m_size;

        public AspectRatioToolParameter(PathShape shape)
            : base(shape)
        {

        }

        public override IPath Path
        {
            get
            {
                VectorPath v;

                Matrix transformMatrix = new Matrix();
                transformMatrix.RotateAt((float)this.Rotation, (PointF)this.Pt);

                switch (Shape)
                {
                    case PathShape.Rectangle:
                        v =  new VectorPath();
                        v.AddRectangle(UnrotatedBounds);
                        v.Transform(transformMatrix);
                        return v;
                    case PathShape.Ellipse:
                        v = new VectorPath();
                        v.AddEllipse(UnrotatedBounds);
                        v.Transform(transformMatrix);
                        return v;
                    default:
                        throw new NotImplementedException();
                }
            }
            set
            {
                // not ideal, but don't really care about the shape of the paths
                SetBoundingRect(value.GetBounds());
            }
        }


        public override PointD Pt
        {
            get
            {
                return m_ctrPt;
            }
            set
            {
                m_ctrPt = value;
            }
        }

        public override PointD PtSize
        {
            get
            {
                return m_size;
            }
            set
            {
                m_size = value;
            }
        }

        public override System.Drawing.RectangleF GetBoundingRect()
        {
            return UnrotatedBounds;
        }

        public override RectangleF GetVisibleRect()
        {
            return Path.GetBounds();
        }

        private System.Drawing.RectangleF UnrotatedBounds
        {
            get
            {
                return new RectangleF((float)(m_ctrPt.X - m_size.X / 2), (float)(m_ctrPt.Y - m_size.Y / 2),
                    (float)m_size.X, (float)m_size.Y);
            }
        }

        public override void SetBoundingRect(System.Drawing.RectangleF bounds)
        {
            m_ctrPt = new PointD(bounds.X + bounds.Width / 2,
                bounds.Y + bounds.Height / 2);
            m_size = new PointD(bounds.Width, bounds.Height);
        }

        public override object Clone()
        {
            AspectRatioToolParameter n = new AspectRatioToolParameter(this.Shape);

            n.Pt = this.Pt;
            n.PtSize = this.PtSize;

            ToolParameter.CopyObject(this, n);

            return n;
        }
    }
}
