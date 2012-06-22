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
using System.Windows.Forms;

namespace KinTemplates.Cursor.Position
{
    public class VirtualBoundedMousePosition : VirtualMousePosition
    {
        private Rectangle[] m_bounds;
        private Form m_parentForm;

        public VirtualBoundedMousePosition(Form parentForm, System.Drawing.Point startPosition)
            : base(startPosition)
        {
            m_parentForm = parentForm;

            m_bounds = new Rectangle[Screen.AllScreens.Length];
            for (int i = 0; i < Screen.AllScreens.Length; i++ )
            {
                m_bounds[i] = new Rectangle(Screen.AllScreens[i].Bounds.Left + 50,
                    Screen.AllScreens[i].Bounds.Top + 50,
                    Screen.AllScreens[i].Bounds.Width - 100,
                    Screen.AllScreens[i].Bounds.Height - 100);
            }
        }

        public override bool SetPhysicalPoint(System.Drawing.Point physicalPosition)
        {
            bool ret = base.SetPhysicalPoint(physicalPosition);

            // see if cursor is in any of the bounded areas
            for (int i = 0; i < m_bounds.Length; i++)
            {
                if (m_bounds[i].Contains(physicalPosition))
                    return ret;
            }
            
            // cursor has to be repositioned
            Rectangle dim = Screen.GetBounds(physicalPosition); //Screen.GetBounds(physicalPosition);
            // centre the mouse cursor
            int offsetX = (dim.Left + dim.Width) / 2 - m_CurPhysicalPos.X;
            int offsetY = (dim.Top + dim.Height) / 2 - m_CurPhysicalPos.Y;
            m_LastPhysicalPos.Offset(offsetX, offsetY);
            m_CurPhysicalPos.Offset(offsetX, offsetY);
            System.Windows.Forms.Cursor.Position = m_parentForm.PointToScreen(m_CurPhysicalPos);

            return ret;
        }
    }
}
