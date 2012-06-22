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

namespace KinTemplates.Cursor.Tools
{
    /// <summary>
    /// When using the LineToolFilter, every time you finished modifying the line's path,
    /// you have to make a call to SubsamplePoints() and then Cache().  This algorithm
    /// implemented here is horribly inefficient, be forewarned!
    /// </summary>
    public abstract class LineToolFilter : IToolFilter
    {
        protected Model.ToolParameter m_Param = new Model.RegionToolParameter(Model.PathShape.Curve);
        protected GraphicsPath m_thickPath;
        protected GraphicsPath m_visibleThickPath;
        protected PointF[] m_pointsInPath;
        private int m_uid;

        public LineToolFilter()
        {
            m_Param.PathThickness = 30;
            m_uid = FilterUID.GetNewUID();
        }

        #region IToolFilter Members

        public abstract PointD GetVelocity(Position.IVirtualMousePosition m);
        
        public PointD GetParamHandlePos()
        {
            if (m_pointsInPath != null && m_pointsInPath.Length > 1)
            {
                PointD firstPt = (PointD) m_pointsInPath[m_pointsInPath.Length / 2];
                PointD secondPt = (PointD) m_pointsInPath[m_pointsInPath.Length / 2 + 1];
                PointD tangent = firstPt - secondPt;
                tangent = tangent / tangent.Magnitude(); // normalize
                PointD normal = PointD.Orthogonal(tangent);
                PointD drawPt = (normal * m_Param.PathThickness / 2) + firstPt - (PointD) GetEffectiveBoundingRect().Location; // should be BoundingRect, hacked in ResizeHandles
                return drawPt;
            }
            else
            {
                return PointD.Empty;
            }
        }

        public RectangleF GetEffectiveBoundingRect()
        {
            return m_thickPath.GetBounds();
        }

        public int UID
        {
            get
            {
                return m_uid;
            }
        }

        public void DrawOutline(System.Drawing.Graphics gc, Render.RenderParameter r)
        {
            try
            {
                if (m_visibleThickPath != null)
                {
                    gc.DrawPath(r.RegionOutline, m_visibleThickPath);
                }
                if (m_Param.Path != null)
                {
                    gc.DrawPath(r.RegionOutline, (Tools.Model.VectorPath)m_Param.Path);
                }
            }
            catch
            {
            }
        }

        public void Draw(System.Drawing.Graphics gc, Render.RenderParameter r, Render.RenderHint editState, Render.IDrawVisitor drawMethods, PointD mousePosition)
        {
            if (m_Param.Path != null && m_Param.Path.PointCount >= 2)
            {
                try
                {
                    if (m_visibleThickPath != null)
                    {
                        if (r.RegionInsideFill != null)
                            gc.FillPath(r.RegionInsideFill, m_visibleThickPath);
                        gc.DrawPath(r.RegionOutline, m_visibleThickPath);
                    }
                    gc.DrawPath(r.RegionOutline, (Tools.Model.VectorPath)m_Param.Path);
                }
                catch 
                {
                    // don't know what happened here
                }

                if (editState ==  Render.RenderHint.Start)
                {
                    Pen dashPen = (Pen)r.RegionOutline.Clone();
                    dashPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    if (m_Param.Path.PointCount > 0)
                    {
                        PointD firstPoint = mousePosition; // TODO ((States.LineHover)editState).FirstPoint;
                        gc.DrawEllipse(dashPen, (float)firstPoint.X - Tools.Render.DrawHelper.TARGET_SIZE / 2.0f, (float)firstPoint.Y - Tools.Render.DrawHelper.TARGET_SIZE / 2.0f, (float)Tools.Render.DrawHelper.TARGET_SIZE, (float)Tools.Render.DrawHelper.TARGET_SIZE);
                    }

                    /*
                    PointD normal;
                    double radius;
                    NormalOfPoint(mousePosition, out normal, out radius);
                    gc.DrawLine(r.ActivePen, (Point)mousePosition, (Point)(mousePosition - normal * radius));
                     */

                    
                }
                else
                {
                    DrawArrowForces(gc, r, drawMethods);
                }

                    drawMethods.DrawHandles(gc, this, r);
                    // TODO ((States.IFilterHandles)editState).Handles.DrawHandles(gc, m_Param, r);

            }
        }

        public abstract void DrawArrowForces(System.Drawing.Graphics gc, Render.RenderParameter r, Render.IDrawVisitor drawMethods);

        public Tools.Model.ToolParameter Parameters
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

        public bool HitTest(PointD point)
        {
            if (m_Param.Path == null)
            {
                return false;
            }
            else
            {
                try
                {
                    if (m_thickPath != null)
                    {
                        return m_thickPath.IsVisible((Point)point);
                    }
                    else
                    {
                        using (Pen widenPen = new Pen(Color.Black, (float)m_Param.PathThickness+Render.DrawHelper.EDGE_WIDTH))
                        {
                            widenPen.StartCap = LineCap.Flat;
                            widenPen.EndCap = LineCap.Flat;
                            widenPen.LineJoin = LineJoin.MiterClipped;

                            return m_Param.Path.IsOutlineVisible((Point)point, widenPen);
                        }
                    }
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine("Exception in LineToolFilter: " + exception.ToString());
                    return false;
                }
            }

        }

        /// <summary>
        /// Creates the line path hit testing (HitTest) and rendering (Draw, DrawOutline).
        /// </summary>
        public void Cache(Render.IDrawVisitor drawMethods)
        {
            if (m_Param.Path.PointCount == 0)
            {
                m_thickPath = new GraphicsPath();
                m_visibleThickPath = new GraphicsPath();
                SecondCache(drawMethods);
                return;
            }

             m_pointsInPath = ((Tools.Model.VectorPath)m_Param.Path).InternalPath.PathPoints;

            // I found a bug here where if I cloned the existing graphics path, it would
            // get into the GC even though it was referenced here. So, as a fix, create the
            // GraphicsPath as new graphics path in this object.
            
            m_thickPath = new GraphicsPath();
            if (m_Param.Path.PointCount > 0)
            {
                m_thickPath.AddPath(((Model.VectorPath)m_Param.Path).InternalPath, false);
                Pen widenPen = new Pen(Color.Black, (float)m_Param.PathThickness+Render.DrawHelper.EDGE_WIDTH);
                widenPen.StartCap = LineCap.Flat;
                widenPen.EndCap = LineCap.Flat;
                widenPen.LineJoin = LineJoin.MiterClipped;
                m_thickPath.Widen(widenPen);
            }

            m_visibleThickPath = new GraphicsPath();
            if (m_Param.Path.PointCount > 0)
            {
                m_visibleThickPath.AddPath(((Model.VectorPath)m_Param.Path).InternalPath, false);
                Pen widenPen = new Pen(Color.Black, (float)m_Param.PathThickness);
                widenPen.StartCap = LineCap.Flat;
                widenPen.EndCap = LineCap.Flat;
                widenPen.LineJoin = LineJoin.MiterClipped;
                m_visibleThickPath.Widen(widenPen);
            }

            SecondCache(drawMethods);


        }

        protected abstract void SecondCache(Render.IDrawVisitor drawMethods);

        #endregion

        #region Finding the tangent and normal vectors along a curve

        /// <summary>
        /// Creates the internal model to actually influence the cursor's movement.  Call Cache() after calling this method.
        /// </summary>
        public void SubsamplePoints()
        {
            PointF[] subsampled = FillPoints(((Tools.Model.VectorPath)m_Param.Path).InternalPath.PathPoints, 2f);
            m_Param.Path = new Tools.Model.VectorPath();
            m_Param.Path.AddLines(subsampled);
        }

        public void PointChargeNormalOfPoint(PointD mouseLocation, out PointD normalVector, out PointD nearestPoint, out double radius)
        {
            int n1;
            float r1;

            if (m_pointsInPath == null)
            {
                normalVector = PointD.Empty;
                nearestPoint = PointD.Empty;
                radius = 0;
                return;
            }

            PointF[] array = m_pointsInPath;

            //NearestPoint(array, (PointF)mouseLocation, out n1, out n2, out r1, out r2);
            //PointD tangent = new PointD(array[n1].X - array[n2].X, array[n1].Y - array[n2].Y);
            PointD tangent;
            SumAllPoints(array, (PointF)mouseLocation, out tangent, out n1, out r1);
            tangent = tangent / tangent.Magnitude();
            normalVector = PointD.Orthogonal(tangent);

            PointD vToPoint = mouseLocation - (PointD)array[n1];
            radius = PointD.DotProduct(normalVector, vToPoint);
            PointD orthVector = radius * normalVector;
            nearestPoint = mouseLocation - orthVector;


            // System.Diagnostics.Debug.WriteLine(String.Format("{0} {1}", array[n1], nearestPoint));

            //// TODO: to optimize here, instead of averaging two nearest points,
            //// we find the orthogonal vector and project to the mouseLocation
            //// onto the line to find nearest point.

            //nearestPoint = new PointD((array[n1].X + array[n2].X) / 2.0, (array[n1].Y + array[n2].Y) / 2.0);
            //normalVector = PointD.Orthogonal(tangent);
            //radius = (r1 + r2) / 2.0;

        }

        public void ExtendedNormalOfPoint(PointD mouseLocation, out PointD normalVector, out PointD nearestPoint, out double radius)
        {
            int n1;
            int n2;
            float r1;
            float r2;

            if (m_pointsInPath == null)
            {
                normalVector = PointD.Empty;
                nearestPoint = PointD.Empty;
                radius = 0;
                return;
            }

            PointF[] array = m_pointsInPath;

            NearestPoint(array, (PointF)mouseLocation, out n1, out n2, out r1, out r2);
            PointD tangent = new PointD(array[n1].X - array[n2].X, array[n1].Y - array[n2].Y);
            tangent = tangent / tangent.Magnitude();
            normalVector = PointD.Orthogonal(tangent);

            PointD vToPoint = mouseLocation - (PointD)array[n1];
            radius = PointD.DotProduct(normalVector, vToPoint);
            PointD orthVector = radius * normalVector;
            nearestPoint = mouseLocation - orthVector;
            

            // System.Diagnostics.Debug.WriteLine(String.Format("{0} {1}", array[n1], nearestPoint));

            //// TODO: to optimize here, instead of averaging two nearest points,
            //// we find the orthogonal vector and project to the mouseLocation
            //// onto the line to find nearest point.

            //nearestPoint = new PointD((array[n1].X + array[n2].X) / 2.0, (array[n1].Y + array[n2].Y) / 2.0);
            //normalVector = PointD.Orthogonal(tangent);
            //radius = (r1 + r2) / 2.0;
            
        }

        private static void FastNearestPoint(PointF[] sourcePoints, PointF testPoint,  out int nearestIdx, out int secondNearestIdx, out float nearestRadius, out float secondNearestRadius)
        {
            // call a binary search algorithm
            FastNearestPoint(sourcePoints, testPoint, 0, sourcePoints.Length - 1, out nearestIdx, out nearestRadius);

            // should find the second nearest point near the first
            if (nearestIdx < sourcePoints.Length - 1 && nearestIdx > 0)
            {
                float testDist = Distance(sourcePoints[nearestIdx], sourcePoints[nearestIdx + 1]);
                float testDist2 = Distance(sourcePoints[nearestIdx], sourcePoints[nearestIdx - 1]);
                if (testDist < testDist2)
                {
                    secondNearestIdx = nearestIdx + 1;
                    secondNearestRadius = testDist;
                }
                else
                {
                    secondNearestIdx = nearestIdx - 1;
                    secondNearestRadius = testDist2;
                }
            }
            else if (nearestIdx == 0)
            {
                secondNearestIdx = 1;
                secondNearestRadius = Distance(sourcePoints[nearestIdx], sourcePoints[nearestIdx + 1]);
            }
            else
            {
                secondNearestIdx = nearestIdx - 1;
                secondNearestRadius = Distance(sourcePoints[nearestIdx], sourcePoints[nearestIdx + -1]);
            }
        }

        private static void FastNearestPoint(PointF[] sourcePoints, PointF testPoint, int startIndex, int endIndex, out int nearestIdx, out float nearestRadius)
        {
            if (startIndex >= endIndex)
            {
                nearestIdx = endIndex;
                nearestRadius = Distance(testPoint, sourcePoints[nearestIdx]);
                return;
            }

            // test 2 points in the middle
            double rad_l = Distance(testPoint, sourcePoints[startIndex]);
            double rad_m = Distance(testPoint, sourcePoints[(startIndex + endIndex) / 2]);
            double rad_u = Distance(testPoint, sourcePoints[endIndex]);

            if ((rad_l < rad_m && rad_m < rad_u) ||
                (rad_m < rad_l && rad_l < rad_u))
            {
                FastNearestPoint(sourcePoints, testPoint, startIndex, (startIndex + endIndex) / 2, out nearestIdx, out nearestRadius);
            }
            else if ((rad_u < rad_m && rad_m < rad_l) ||
                (rad_m < rad_u && rad_u < rad_l))
            {
                FastNearestPoint(sourcePoints, testPoint, (startIndex + endIndex) / 2 + 1, endIndex, out nearestIdx, out nearestRadius);
            }
            else // ((rad_l < rad_u && rad_u < rad_m) ||
                // (rad_u < rad_l && rad_l < rad_m)
            {
                int lowerIdx, upperIdx;
                float lowerRad, upperRad;

                FastNearestPoint(sourcePoints, testPoint, startIndex, (startIndex + endIndex) / 2, out lowerIdx, out lowerRad);
                FastNearestPoint(sourcePoints, testPoint, (startIndex + endIndex) / 2 + 1, endIndex, out upperIdx, out upperRad);

                if (lowerRad < upperRad)
                {
                    nearestIdx = lowerIdx;
                    nearestRadius = lowerRad;
                }
                else
                {
                    nearestIdx = upperIdx;
                    nearestRadius = upperRad;
                }
            }
        }

        public static void NearestPoint(PointF[] sourcePoints, PointF testPoint, out int nearestIdx, out int secondNearestIdx, out float nearestRadius, out float secondNearestRadius)
        {
            if (sourcePoints.Length < 2)
            {
                nearestIdx = -1;
                secondNearestIdx = -1;
            }

            nearestIdx = 0;
            nearestRadius = Distance(testPoint, sourcePoints[nearestIdx]);

            // nearest point
            for (int i = 1; i < sourcePoints.Length; i++)
            {
                float testDist = Distance(sourcePoints[i], testPoint);
                if (testDist < nearestRadius)
                {
                    nearestRadius = testDist;
                    nearestIdx = i;
                }
            }

            // should find the second nearest point near the first
            if (nearestIdx < sourcePoints.Length - 1 && nearestIdx > 0)
            {
                float testDist = Distance(sourcePoints[nearestIdx], sourcePoints[nearestIdx + 1]);
                float testDist2 = Distance(sourcePoints[nearestIdx], sourcePoints[nearestIdx - 1]);
                if (testDist < testDist2)
                {
                    secondNearestIdx = nearestIdx + 1;
                    secondNearestRadius = testDist;
                }
                else
                {
                    secondNearestIdx = nearestIdx - 1;
                    secondNearestRadius = testDist2;
                }
            }
            else if (nearestIdx == 0)
            {
                secondNearestIdx = 1;
                secondNearestRadius = Distance(sourcePoints[nearestIdx], sourcePoints[nearestIdx + 1]);
            }
            else
            {
                secondNearestIdx = nearestIdx - 1;
                secondNearestRadius = Distance(sourcePoints[nearestIdx], sourcePoints[nearestIdx +-1]);
            }

            /*
            // second nearest point
            secondNearestIdx = (nearestIdx != 0) ? 0 : 1;
            secondNearestRadius = Distance(testPoint, sourcePoints[secondNearestIdx]);

            for (int i = 1; i < sourcePoints.Length; i++)
            {
                if (i != nearestIdx)
                {
                    float testDist = Distance(sourcePoints[i], testPoint);
                    if (testDist < secondNearestRadius)
                    {
                        secondNearestRadius = testDist;
                        secondNearestIdx = i;
                    }
                }
            }*/
        
        }

        private static void SumAllPoints(PointF[] sourcePoints, PointF testPoint, out PointD tangent, out int nearestIdx, out float nearestRadius)
        {
            if (sourcePoints.Length < 2)
            {
                tangent = PointD.Empty;
                nearestIdx = -1;
                nearestRadius = 0;
                return;
            }

            nearestIdx = 0;
            nearestRadius = Distance(testPoint, sourcePoints[nearestIdx]);

            PointD weightedVector = PointD.Empty ;

            // nearest point
            for (int i = 1; i < sourcePoints.Length; i++)
            {
                float testDist = Distance(sourcePoints[i], testPoint);

                if (testDist > 0.001)
                    weightedVector = weightedVector + ((PointD) sourcePoints[i] - (PointD)sourcePoints[i - 1]) / testDist;

                if (testDist < nearestRadius)
                {
                    nearestRadius = testDist;
                    nearestIdx = i;
                }

            }

            tangent = weightedVector / weightedVector.Magnitude();
        }


        public static PointF[] FillPoints(PointF[] sourcePoints, float stepDistance)
        {
            if (sourcePoints.Length < 2)
            {
                PointF[] retArray = new PointF[sourcePoints.Length];
                sourcePoints.CopyTo(retArray, 0);
                return retArray;
            }

            List<PointF> result = new List<PointF>(sourcePoints.Length);
            PointF lastPoint = sourcePoints[0];
            int i = 1;
            result.Add(lastPoint);

            while (i < sourcePoints.Length)
            {
                float dist = Distance(lastPoint, sourcePoints[i]);
                // point is within the proper distance
                if (dist <= stepDistance)
                {
                    result.Add(sourcePoints[i]);
                    lastPoint = sourcePoints[i];
                    i++;
                }
                else
                {
                    // slope between lastPoint and sourcePoints[i] 
                    float vX = (sourcePoints[i].X - lastPoint.X) / dist;
                    float vY = (sourcePoints[i].Y - lastPoint.Y) / dist;
                    PointF newPt = new PointF(lastPoint.X + vX * stepDistance, lastPoint.Y + vY * stepDistance);
                    result.Add(newPt);
                    lastPoint = newPt;
                }

               
            }
            return result.ToArray();
        }

        public static float Distance(PointF pt1, PointF pt2)
        {
            return (float) Math.Sqrt(Pow2(pt1.X - pt2.X) + Pow2(pt1.Y - pt2.Y));
        }

        public static float Pow2(float number)
        {
            return number * number;
        }


        #endregion
    }
}
