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

namespace KinTemplates.Cursor.Tools.Render
{

    // make objects for each of:
    //      guides
    //      sandpaper and ice sheet
    //      forces
    //      positive and negative space

    public class OnlineFeedback 
    {
        struct Clip
        {
            public GraphicsPath ClipPath;
            public IToolFilter Filter;
        }

        #region Members


        int m_zoom = 50;// default zoom
        Region m_shadeRegion;
        List<Clip> m_guideLines;

        public void ClearFeedback()
        {
            m_zoom = 50;

            if (m_shadeRegion != null)
            {
                m_shadeRegion.Dispose();
                m_shadeRegion = null;
            }

            if (m_guideLines != null)
            {
                m_guideLines.Clear(); ;
            }
        }

        
        public void AddFilter(KinTemplates.Cursor.Tools.IToolFilter filter, FeedbackParameter r, PointD mousePos)
        {
            if (filter.HitTest(mousePos))
            {
                // zoom in and out
                if (filter is Tools.SandpaperFilter ||
                    filter is Tools.MaxFilter)
                {
                    RegionToolRegionFilter rtrf = (RegionToolRegionFilter)filter;
                    m_zoom = (int)((m_zoom + filter.Parameters.C * 100 * rtrf.GetStrength(mousePos)));
                }
                else if (filter is Tools.MinFilter ||
                    filter is Tools.IceSheetFilter)
                {
                    RegionToolRegionFilter rtrf = (RegionToolRegionFilter)filter;
                    m_zoom = (int)((m_zoom - filter.Parameters.C * 40 * rtrf.GetStrength(mousePos)));
                    if (m_zoom < 10) 
                        m_zoom = 10;
                }

                //// shade out surrounding area
                //if (filter is Tools.TunnelLineFilter || (filter is Tools.MagneticLineAttractionFilter
                //    && !(filter is Tools.MagneticLineRepulsionFilter)))
                //{
                //    /*
                //    if (m_shadeRegion == null)
                //    {
                //        m_shadeRegion = new Region();
                //        m_shadeRegion.MakeEmpty();
                //    }

                //    Region inversion = new Region();
                //    inversion.MakeInfinite();
                //    GraphicsPath test = (GraphicsPath)((Tools.Model.VectorPath)filter.Parameters.Path).InternalPath.Clone();
                //    test.Widen(new Pen(Color.Black, (float)filter.Parameters.PtRadius));

                //    inversion.Exclude(test);
                //    m_shadeRegion.Union(inversion);
                //    inversion.Dispose();
                //    test.Dispose();
                //     */

                //    // shade in the region
                //    if (m_shadeRegion == null)
                //    {
                //        m_shadeRegion = new Region();
                //        m_shadeRegion.MakeEmpty();
                //    }
                //    GraphicsPath test = (GraphicsPath)((Tools.Model.VectorPath)filter.Parameters.Path).InternalPath.Clone();
                //    test.Widen(new Pen(Color.Black, (float)filter.Parameters.PathThickness));
                //    m_shadeRegion.Union(test);
                //    test.Dispose();    

                //}
                //// shade in the filter
                //else if (filter is Tools.MagneticLineRepulsionFilter)
                //{
                //    if (m_shadeRegion == null)
                //    {
                //        m_shadeRegion = new Region();
                //        m_shadeRegion.MakeEmpty();
                //    }
                //    GraphicsPath test = (GraphicsPath)((Tools.Model.VectorPath)filter.Parameters.Path).InternalPath.Clone();
                //    test.Widen(new Pen(Color.Black, (float)filter.Parameters.PathThickness));
                //    m_shadeRegion.Union(test);
                //    test.Dispose();                    
                //}
               
                //// draw guide lines
                //if (filter is Tools.CorduroyFilter || filter is Tools.GridFilter
                //    || filter is Tools.SteadyHandFilter)
                //{

                //    if (m_guideLines == null)
                //        m_guideLines = new List<Clip>();

                //    Clip c = new Clip();

                //    PointD norm = filter.Parameters.V;
                //    PointD side1 = mousePos + norm * 1000;
                //    PointD side2 = mousePos - norm * 1000;

                //    c.ClipPath = new GraphicsPath();
                //    c.ClipPath.StartFigure();
                //    c.ClipPath.AddLine((Point)side1, (Point)side2);
                //    c.Filter = filter;

                //    if (filter is Tools.GridFilter)
                //    {
                //        PointD orth = PointD.Orthogonal(filter.Parameters.V);
                //        PointD end1 = mousePos + orth * 1000;
                //        PointD end2 = mousePos - orth * 1000;

                //        c.ClipPath.StartFigure();
                //        c.ClipPath.AddLine((Point)end1, (Point)end2);
                //    }

                //    m_guideLines.Add(c);
                //}

                //// draw online guides
                //if (filter is Tools.CompassFilter)
                //{
                //    PointD radiusVector = mousePos - filter.Parameters.Pt;
                //    double radius = radiusVector.Magnitude();

                //    if (radius > 0.0001)
                //    {
                //        if (m_guideLines == null)
                //            m_guideLines = new List<Clip>();

                //        Clip c = new Clip();
                //        c.ClipPath = new GraphicsPath();
                //        c.ClipPath.StartFigure();
                //        c.ClipPath.AddEllipse((float)(filter.Parameters.Pt.X - radius), (float)(filter.Parameters.Pt.Y - radius),
                //            (float)radius * 2, (float)radius * 2);

                //        m_guideLines.Add(c);
                //    }
                //}

                //if (filter is Tools.DimpleChadFilter)
                //{
                //    PointD vector = mousePos - filter.Parameters.Pt;
                //    if (vector.Magnitude() > 0.0001)
                //    {
                //        PointD unitVector = vector / vector.Magnitude();
                //        if (m_guideLines == null)
                //            m_guideLines = new List<Clip>();

                //        Clip c = new Clip();
                //        c.ClipPath = new GraphicsPath();
                //        c.ClipPath.StartFigure();
                //        c.ClipPath.AddLine((Point)(filter.Parameters.Pt + unitVector * Math.Max( filter.Parameters.PtSize.X, filter.Parameters.PtSize.Y) ),
                //            (Point)(filter.Parameters.Pt - unitVector * Math.Max(filter.Parameters.PtSize.X, filter.Parameters.PtSize.Y)));

                //        m_guideLines.Add(c);
                //    }

                //}

            }
        }

        public void RenderGuides(System.Drawing.Graphics gc, FeedbackParameter r)
        {
            if (m_guideLines != null)
            {
                foreach (Clip c in m_guideLines)
                {
                    if (c.Filter != null)
                        gc.SetClip(c.Filter.Parameters.Path.GetBounds());

                    gc.DrawPath(r.GuideLine, c.ClipPath);

                    if (c.Filter != null)
                        gc.ResetClip();
                }
            }
        }

        public void RenderShade(System.Drawing.Graphics gc, FeedbackParameter r)
        {
            if (m_shadeRegion != null)
            {

                gc.FillRegion(r.ShadeBrush, m_shadeRegion);
            }
        }


        public int GetPIPScalingValue()
        {
            return m_zoom;
        }

        #endregion
    }
}
