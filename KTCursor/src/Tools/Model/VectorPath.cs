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
    public class VectorPath : IPath
    {
        protected bool m_disposed = false;
        protected GraphicsPath m_path;

        public VectorPath()
        {
            m_path = new GraphicsPath();
        }

        public VectorPath(RectangleF rectangle) 
        {
            m_path = new GraphicsPath();
            m_path.AddRectangle(rectangle);
        }

        public VectorPath(PointF pt1, PointF pt2)
        {
            m_path = new GraphicsPath();
            m_path.AddLine(pt1, pt2);
        }

        public VectorPath(GraphicsPath gp)
        {
            m_path = new GraphicsPath();
            m_path.AddPath(gp, false);
        }

        public static implicit operator GraphicsPath(VectorPath p)
        {
            return p.m_path;
        }

        public GraphicsPath InternalPath
        {
            get
            {
                return m_path;
            }
        }

        #region IPath Members

        public void StartFigure()
        {
            m_path.StartFigure();
            
        }

        public void AddLine(PointF point1, PointF point2)
        {
            m_path.AddLine(point1, point2);
        }

        public void AddLines(PointF[] points)
        {
            m_path.AddLines(points);
        }

        public void AddPath(IPath path, bool connect)
        {
            if (path is VectorPath)
            {
                if (path.PointCount > 0)
                    m_path.AddPath(((VectorPath)path).InternalPath, connect);
            }
            else
                throw new Exception("Not handled");
        }
        public void AddRectangle(RectangleF rect)
        {
            m_path.AddRectangle(rect);
        }

        public void CloseFigure()
        {
            m_path.CloseFigure();
        }

        public PointF GetFirstPoint()
        {
            return m_path.PathPoints[0];
        }

        public PointF GetLastPoint()
        {
            return m_path.GetLastPoint();
        }

        public RectangleF GetBounds()
        {
            return m_path.GetBounds();
        }

        public int PointCount
        {
            get 
            {
                return m_path.PointCount;
            }
        }

        public void AddEllipse(RectangleF rect)
        {
            m_path.AddEllipse(rect);
        }

        public void Translate(float shiftX, float shiftY)
        {
            Matrix m = new Matrix();
            m.Translate(shiftX, shiftY);
            m_path.Transform(m);
        }

        public void Transform(Matrix matrix)
        {
            m_path.Transform(matrix);
        }

        public bool IsVisible(PointF point)
        {
            return m_path.IsVisible( point);
        }
        public bool IsOutlineVisible(PointF point, Pen test)
        {
            return m_path.IsOutlineVisible(point, test);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            m_path.Dispose();
            GC.SuppressFinalize(this);
            m_disposed = true;
        }

        ~VectorPath()
        {
            if (!m_disposed)
                m_path.Dispose();
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            VectorPath newPath = new VectorPath();
            if (this.PointCount > 0)
                newPath.AddPath(this, false);
            return newPath;
        }

        #endregion
    }
}
