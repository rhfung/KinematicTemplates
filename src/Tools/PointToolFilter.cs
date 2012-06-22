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
    public abstract class PointToolFilter : IToolFilter
    {
        /// <summary>
        /// Used by PointToolFilter.
        /// </summary>
        public class RadialFactors
        {
            public RadialPointD Scaling;
            public RadialPointD Additive;

            public RadialFactors()
            {
                Scaling = new RadialPointD(1, 1);
                Additive = new RadialPointD(0, 0);
            }

            public RadialFactors(double S_r, double S_theta, double C_r, double C_theta)
            {
                Scaling = new RadialPointD(S_r, S_theta);
                Additive = new RadialPointD(C_r, C_theta);
            }
        }


        protected Model.ToolParameter m_Param = new Model.AspectRatioToolParameter(Model.PathShape.Ellipse);
        private int m_uid;
        private GraphicsPath m_thickPath;

        #region IToolFilter Members

        public PointToolFilter()
        {
            m_uid = FilterUID.GetNewUID();
        }

        public abstract PointD GetParamHandlePos();

        protected abstract RadialFactors GetInnerVelocity(Position.RadialMousePosition m);

        public int UID
        {
            get
            {
                return m_uid;
            }
        }

        public PointD GetVelocity(Position.IVirtualMousePosition m)
        {
            Matrix matrix = new Matrix();

            matrix.Rotate((float)(m_Param.Rotation));
            if (m_Param.PtSize.X < m_Param.PtSize.Y)
                matrix.Scale(1, (float)(m_Param.PtSize.Y / m_Param.PtSize.X));
            else
                matrix.Scale((float)(m_Param.PtSize.X / m_Param.PtSize.Y), 1);
            matrix.Invert();

            // convert Cartesian points into local coordinate system

            PointF[] pointsToCheck = 
                new PointF[] {
                    (PointF) m.GetDisplacement(),
                    (PointF) m.GetVelocity(),
                    (PointF) (m.GetVirtualPointD() - m_Param.Pt) };

            matrix.TransformPoints(pointsToCheck);

            // create the parameters 
            Position.RadialMousePosition mp = new Position.RadialMousePosition();

            // PointD newPt = (PointD)pointsToCheck[2] + (PointD)pointsToCheck[0];
            // velocity is modified by other templates, not the new displacement
            PointD newPt = (PointD)pointsToCheck[2] + (PointD)pointsToCheck[1] * m.GetTimeInterval();

            //System.Diagnostics.Debug.WriteLine("");

            //System.Diagnostics.Debug.WriteLine(String.Format("{0} {1} {2}", (PointD)pointsToCheck[2],
            //    (PointD)pointsToCheck[1], (PointD)pointsToCheck[1] * m.GetTimeInterval()));

            mp.VirtualPoint = RadialPointD.ToPolar((PointD)pointsToCheck[2], PointD.Empty);
            mp.Delta = RadialPointD.ToPolar(newPt, PointD.Empty) - mp.VirtualPoint;
            mp.Velocity = RadialPointD.ToPolar((PointD)pointsToCheck[1], PointD.Empty);
            mp.TimeInterval = m.GetTimeInterval();

            // crossing near angle -PI or PI
            int signOrig = Math.Sign(RadialPointD.ToPolar(newPt, PointD.Empty).Theta);
            int signNew = Math.Sign(mp.VirtualPoint.Theta);
            if (signOrig != signNew)
            {
                if (signOrig == -1)
                {
                    if (mp.Delta.Theta < -3.141592)
                        mp.Delta.Theta += 2 * 3.141592;
                }
                else if (signOrig == 1)
                {
                    if (mp.Delta.Theta > 3.141592)
                        mp.Delta.Theta -= 2 * 3.141592;
                }

            }

            //System.Diagnostics.Debug.WriteLine(String.Format("{0} {1}", signOrig, signNew));

            //System.Diagnostics.Debug.WriteLine(String.Format("{0} {1} {2} {3}", 
            //    mp.Delta, mp.Velocity, mp.VirtualPoint,mp.TimeInterval));

            RadialFactors rfactors = GetInnerVelocity(mp);
            //RadialPointD newDelta = new RadialPointD(mp.Delta.R * rfactors.Scaling.R, mp.Delta.Theta * rfactors.Scaling.Theta)
            //    + new RadialPointD(rfactors.Additive.R, rfactors.Additive.Theta);

            RadialPointD newDelta1 = new RadialPointD(mp.Delta.R * rfactors.Scaling.R, mp.Delta.Theta * rfactors.Scaling.Theta);
            RadialPointD newDelta2 = new RadialPointD(rfactors.Additive.R, rfactors.Additive.Theta);

            PointD deltaXY = RadialPointD.ToCartesian(newDelta1 + newDelta2 + mp.VirtualPoint, PointD.Empty);
            PointD diff = deltaXY - (PointD)pointsToCheck[2]; // difference from original location, in local coordinate system

            //System.Diagnostics.Debug.WriteLine(String.Format("theta:{0} theta':{1} xy:{2} xy':{3}", mp.Delta.Theta, mp.Delta.Theta * rfactors.Scaling.Theta,
            //    (PointD) pointsToCheck[2], deltaXY));

            // convert point back into global coordinate system
            PointF[] outputPoints = new PointF[] { (PointF)(diff) };
            matrix.Invert();
            matrix.TransformPoints(outputPoints );

            if (this is OrbitFilter3)
            {
                if (mp.VirtualPoint.Theta < 0) // top half
                {
                    if (m.GetVelocity().Y > 0) // moving down
                        return (PointD)outputPoints[0] / m.GetTimeInterval() - new PointD(m.GetVelocity().X, 0);
                    else // moving up
                        return (PointD)outputPoints[0] / m.GetTimeInterval() - new PointD(m.GetVelocity().X, 0);
                }
                else // bottom half
                {
                    if (m.GetVelocity().Y > 0) // moving down
                        return (PointD)outputPoints[0] / m.GetTimeInterval() - new PointD(m.GetVelocity().X, 2 * m.GetVelocity().Y) ;
                    else // moving up
                        return (PointD)outputPoints[0] / m.GetTimeInterval() - new PointD(m.GetVelocity().X, 2 * m.GetVelocity().Y);
                }
            }
            else
            {
                return (PointD)outputPoints[0] / m.GetTimeInterval() - m.GetVelocity();
            }
        }

        public void DrawOutline(System.Drawing.Graphics gc, Render.RenderParameter r)
        {
            if (!m_Param.Pt.IsEmpty)
            {
                gc.DrawEllipse(r.RegionOutline, new Rectangle((int)m_Param.Pt.X - 2, (int)m_Param.Pt.Y - 2, 4, 4));

                if (m_Param.PtRadius > 0)
                {
                   gc.DrawPath(r.RegionOutline, m_Param.Path.InternalPath);
                }

            }
        }

        public void Draw(System.Drawing.Graphics gc, Render.RenderParameter r, Render.RenderHint editState, Render.IDrawVisitor drawMethods, PointD mousePosition)
        {
            if (!m_Param.Pt.IsEmpty)
            {
                if (Math.Min( m_Param.PtSize.X, m_Param.PtSize.Y) > Render.DrawHelper.MIN_SPEED_SENSITIVITY )
                {
                    //m_Param.Path = new Tools.Model.VectorPath();
                    //m_Param.Path.AddEllipse(m_Param.GetBoundingRect());

                    drawMethods.DrawNegativeSpace(gc, m_Param.Path.InternalPath, r);
                    drawMethods.DrawPositiveSpace(gc, m_Param.Path.InternalPath, r);

                    // crosshair
                    gc.DrawLine(r.RegionOutline, (Point)(m_Param.Pt + new PointD(-5, 0)), (Point)(m_Param.Pt + new PointD(5, 0)));
                    gc.DrawLine(r.RegionOutline, (Point)(m_Param.Pt + new PointD(0, -5)), (Point)(m_Param.Pt + new PointD(0, 5)));

                    // transform coordinate system
                    GraphicsState origGC = gc.Save();
                    gc.Clip = new Region(m_Param.Path.InternalPath);

                    gc.TranslateTransform((float)m_Param.Pt.X, (float)m_Param.Pt.Y);
                    gc.RotateTransform((float)(m_Param.Rotation));
                        if (m_Param.PtSize.X < m_Param.PtSize.Y)
                        gc.ScaleTransform(1, (float)(m_Param.PtSize.Y / m_Param.PtSize.X));
                    else
                        gc.ScaleTransform((float)(m_Param.PtSize.X / m_Param.PtSize.Y), 1);

                    // draw outline
                    //GraphicsPath path = new GraphicsPath();
                    //path.AddEllipse(new RectangleF(-(float)m_Param.PtRadius, -(float)m_Param.PtRadius, (float)m_Param.PtRadius *2 , (float)m_Param.PtRadius * 2));

                    // draw feedback -- suppress for when feedback state is asked for (??)
                    if (IsForce() && (editState != Render.RenderHint.Feedback))
                    {
                        DrawInsides(gc, r, drawMethods);
                    }
                    else if (!IsForce())
                    {
                        DrawInsides(gc, r, drawMethods);
                    }


                    // restore old graphics system
                    gc.Restore(origGC);

                }
                else
                {
                    // crosshair
                    gc.DrawLine(r.RegionOutline, (Point)(new PointD(-5, 0)), (Point)(new PointD(5, 0)));
                    gc.DrawLine(r.RegionOutline, (Point)(new PointD(0, -5)), (Point)(new PointD(0, 5)));

                }

            }


            //if (editState == Render.RenderHint.Handles)
            //{
                drawMethods.DrawHandles(gc, this, r);

                //TODO: ((States.IFilterHandles)editState).Handles.DrawHandles(gc, m_Param, r);
            //}

        }

        private bool IsForce()
        {
            return this is OrbitFilter
                || this is OrbitFilter2
                || this is OrbitFilter3
                || this is MagneticPointAttractionFilter
                || this is RubberBandFilter ;
        }

        // draw passive
        private void DrawInsides(System.Drawing.Graphics gc, Render.RenderParameter r, Render.IDrawVisitor drawMethods)
        {
            if (IsForce())
                DrawArrowPoints(gc, r, drawMethods);
            else if (this is CompassFilter)
            {
                float ringSpacing = (float) drawMethods.Spacing(m_Param.C);
                    int numRings = (int)(m_Param.PtRadius / ringSpacing);
                for (int i = 1; i < numRings; i++)
                {
                    gc.DrawEllipse(r.RegionGuides, 
                        new RectangleF(-i * ringSpacing,
                            -i * ringSpacing,
                            i * ringSpacing * 2 ,
                            i * ringSpacing * 2) );
                }
            }
            else if (this is DimpleChadFilter)
            {
                double angle = 360 / 8;
                for (int j = 0; j < 8; j++)
                {
                    // put some points in the circle region
                    PointD theVector = new PointD(
                        Math.Cos(angle * j / 180.0 * Math.PI),
                        Math.Sin(angle * j / 180.0 * Math.PI));
                    gc.DrawLine(r.RegionGuides, new PointF(0, 0),
                        (PointF)(theVector * m_Param.PtRadius));
                }
            }
            
        }

        // draw active
        private void DrawArrowPoints(System.Drawing.Graphics gc, Render.RenderParameter r,Render.IDrawVisitor drawMethods)
        {
            double size = (Math.Min(m_Param.PtSize.X, m_Param.PtSize.Y) / 2);
            int ptRings = (int)(size / (Render.DrawHelper.SPEED_AMPLIFIER * 1.1 + 10));

            double angle = 360 / 8;
            for (int i = 1; i <= ptRings; i++)
            {
                double innerRadius = (Render.DrawHelper.SPEED_AMPLIFIER * 1.1  + 10) * i;
                for (int j = 0; j < 8; j++)
                {
                    // put some points in the circle region
                    PointD theVector = new PointD(
                        Math.Cos(angle * j / 180.0 * Math.PI),
                        Math.Sin(angle * j / 180.0 * Math.PI));
                    PointD thePoint = theVector * innerRadius;
                    // test these points
                    //Position.VirtualMousePosition mouse = new Position.VirtualMousePosition((Point)thePoint);
                    //ParameterizedMousePosition fakeMouse = new ParameterizedMousePosition(mouse, 20);// TODO: instrument from the computer
                    //PointD vel = GetVelocity(fakeMouse) * Render.DrawHelper.SPEED_AMPLIFIER ;  
                    PointD vel = PointD.Empty;
                    if (this is MagneticPointAttractionFilter  )
                    {
                        vel = theVector * (m_Param.V.Y != 0 ? Math.Sign(m_Param.V.Y) : 1) * -Render.DrawHelper.SPEED_AMPLIFIER * (m_Param.C + 0.1);
                    }
                    if (this is RubberBandFilter)
                    {
                        
                        vel = theVector * (m_Param.V.Y != 0 ? Math.Sign(m_Param.V.Y) : 1) * -Render.DrawHelper.SPEED_AMPLIFIER * (m_Param.C + 0.1) * Math.Pow(innerRadius / m_Param.PtRadius, 2);
                    }
                    if (this is OrbitFilter )
                    {
                        vel = PointD.Orthogonal(theVector) * (m_Param.V.X != 0 ? Math.Sign(m_Param.V.X) : 1) * Render.DrawHelper.SPEED_AMPLIFIER * (m_Param.C + 0.1); 
                    }
                    if (this is OrbitFilter2)
                    {
                        vel = PointD.Orthogonal(theVector) * (m_Param.V.X != 0 ? Math.Sign(m_Param.V.X) : 1)  * (m_Param.C + 0.1) * (innerRadius / 4 ); 
                    }
                    if (this is OrbitFilter3)
                    {
                        vel = PointD.Orthogonal(theVector) * (m_Param.V.X != 0 ? Math.Sign(m_Param.V.X) : 1) * (m_Param.C + 0.1) * (innerRadius / 4);
                    }
                    drawMethods.DrawArrow(gc, r, thePoint, vel);
                }
            }

            if (this is OrbitFilter3)
            {
                gc.DrawString("Up/down to change radius", SystemFonts.DialogFont, Brushes.Blue, new PointF(-(float)size, 0f));
            }

        }


        private void DrawDirectionHint(System.Drawing.Graphics gc, Render.RenderParameter r, PointD thePoint, PointD theVector)
        {
                gc.DrawLine(r.RegionGuides, (Point)(thePoint - theVector * 0.5), (Point)(thePoint + theVector));
        }

        /*
        public void DrawDebug(System.Drawing.Graphics gc)
        {
            String name = ToString();
            if (name.LastIndexOf(".") > 0)
                name = name.Substring(name.LastIndexOf(".") + 1);

            PointD centre = m_Param.Pt;
            gc.FillEllipse(Brushes.Black, new Rectangle((int)centre.X - 2, (int)centre.Y - 2, 4, 4));
            gc.DrawString(name, new Font(FontFamily.GenericSansSerif, 10),
                Brushes.Black, new Point((int)centre.X - 10, (int)centre.Y + 2));

        }
         */

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

        public virtual bool HitTest(PointD point)
        {
            if (m_thickPath != null)
            {
                return m_thickPath.IsVisible((PointF)point);
                
            }
            else if (m_Param.PtSize != PointD.Empty)
            {
                return m_Param.Path.IsVisible((PointF)point);
            }
            else
            {
                return false; //!m_Param.Pt.IsEmpty;
            }

        }

        public void Cache(Render.IDrawVisitor drawMethods)
        {
            RectangleF s = m_Param.GetBoundingRect();
            s.X -= Render.DrawHelper.EDGE_WIDTH/2;
            s.Y -= Render.DrawHelper.EDGE_WIDTH/2;
            s.Width += Render.DrawHelper.EDGE_WIDTH;
            s.Height += Render.DrawHelper.EDGE_WIDTH;
            m_thickPath = new GraphicsPath();
            m_thickPath.AddEllipse(s);
            Matrix m = new Matrix();
            m.RotateAt((float)this.m_Param.Rotation, (PointF)this.m_Param.Pt);
            m_thickPath.Transform(m);

            //System.Diagnostics.Debug.WriteLine("PointToolFilter.Cache called from " + Environment.StackTrace);
        }

        #endregion
    }
}
