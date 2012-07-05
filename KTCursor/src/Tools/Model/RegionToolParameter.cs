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

namespace KinTemplates.Cursor.Tools.Model
{
    public class RegionToolParameter : ToolParameter
    {
        private IPath m_path;

        public RegionToolParameter(PathShape shape) : base(shape)
        {

        }

        public override IPath Path
        {
            get
            {
                return m_path;
            }
            set
            {
                m_path = value;
            }
        }


        public override PointD Pt
        {
            get
            {
                RectangleF bounds = m_path.GetBounds();
                return new PointD(bounds.X + bounds.Width / 2,
                    bounds.Y + bounds.Height / 2);
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override PointD PtSize
        {
            get
            {
                RectangleF bounds = m_path.GetBounds();
                return new PointD(bounds.Width, bounds.Height);
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override System.Drawing.RectangleF GetBoundingRect()
        {
            return m_path.GetBounds();
        }

        public override RectangleF GetVisibleRect()
        {
            return m_path.GetBounds();
        }

        public override void SetBoundingRect(System.Drawing.RectangleF bounds)
        {
            // this method hasn't been used yet
            switch (this.Shape)
            {
                case PathShape.Curve:
                    throw new NotImplementedException();
                case PathShape.Ellipse:
                    //m_path = new VectorPath();
                    //m_path.AddEllipse(bounds);
                    //break;
                case PathShape.Rectangle:
                    //m_path = new VectorPath();
                    //m_path.AddRectangle(bounds);
                    //break;
                case PathShape.Freeform:
                    RectangleF oldBounds = GetBoundingRect();
                    SizeF oldSize = oldBounds.Size;
                    SizeF newSize = bounds.Size;
                    PointF oldCorner = oldBounds.Location;
                    PointF newCorner = bounds.Location;
                    System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix();
                    m.Translate(-oldCorner.X, -oldCorner.Y);
                    m.Scale(newSize.Width / oldSize.Width, newSize.Height / oldSize.Height);
                    m.Translate(newCorner.X, newCorner.Y);
                    m_path.Transform(m);
                    break;
            }

        }

        public override object Clone()
        {
            RegionToolParameter n = new RegionToolParameter(this.Shape);

            if (this.Path != null)
                n.Path = (Tools.Model.IPath)this.Path.Clone();
            ToolParameter.CopyObject(this, n);

            return n;
        }
    }
}
