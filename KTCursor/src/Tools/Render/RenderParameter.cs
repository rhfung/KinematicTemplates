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

namespace KinTemplates.Cursor.Tools.Render
{
    public class RenderParameter 
    {
        public Pen RegionOutline;  // for the outline
        public Pen RegionGuides; // for the information inside the region
        public Pen StrokeOutline; // while stroking
        public Pen HandleOutline; // for edit mode handles
        public Brush HandleFill;
        public Brush StrokeFill;
        public Brush RegionInsideFill;  // positive space
        public Brush RegionOutsideFill; // negative space
        public Font FontType;

        private static RenderParameter m_DrawMode = null;
        private static RenderParameter m_DetailedMode = null;

        protected RenderParameter()
        {
        }

        // DrawMode() should be used in conjuction with DrawOutline()
        public static RenderParameter DrawMode()
        {
            if (m_DrawMode == null)
                m_DrawMode = CreateDrawMode();

            return m_DrawMode;
        }

        /// <summary>
        /// Changes the RegionOutline, RegionGuides, and StrokeOutline to the specified templateColour.
        /// </summary>
        /// <param name="templateColour"></param>
        public void SetNewTemplateColour(Color templateColour)
        {
            // user can change pen colour at run-time
            this.RegionOutline = new Pen(templateColour);
            this.RegionGuides = new Pen(templateColour);
            this.StrokeOutline = new Pen(templateColour);

        }

        private static RenderParameter CreateDrawMode()
        {
            RenderParameter n = new RenderParameter();
            CreateDrawMode(n);
            return n;
        }

        protected static void CreateDrawMode(RenderParameter n)
        {
            n.RegionOutline = new Pen(Color.FromArgb(192, Color.Silver));
            n.RegionGuides = n.RegionOutline;
            n.StrokeOutline = n.RegionOutline;
            n.HandleOutline = Pens.Transparent;
            n.RegionInsideFill = Brushes.Transparent; // new SolidBrush(Color.FromArgb(127, Color.White));
            n.RegionOutsideFill = null;
            n.FontType = new Font(System.Drawing.FontFamily.GenericSansSerif, 8);
        }

        // EditMode() should be used in conjuction with Draw()
        public static RenderParameter EditMode(Render.RenderHint subState )
        {
            if (subState == Render.RenderHint.Start)
                return DrawMode();
            else
                return DetailedMode();
        }

        public static RenderParameter DetailedMode()
        {
            if (m_DetailedMode == null)
                m_DetailedMode = CreateDetailedMode();
            return m_DetailedMode;
        }

        private static RenderParameter CreateDetailedMode()
        {
            RenderParameter n = new RenderParameter();
            n.RegionOutline = new Pen(Color.Black, 1);
            n.RegionGuides = new Pen(Color.Black, 1);
            n.StrokeOutline = new Pen(Color.Black, 1);
            n.HandleOutline = new Pen(Color.DarkBlue, 1);
            n.HandleFill = new SolidBrush(Color.LightBlue);
            n.StrokeFill = new SolidBrush(Color.FromArgb(127, Color.White));
            n.RegionInsideFill = new SolidBrush(Color.FromArgb(127, Color.White));
            n.RegionOutsideFill = null;
            n.FontType = new Font(System.Drawing.FontFamily.GenericSansSerif, 8);
            return n;
        }
    }
}
