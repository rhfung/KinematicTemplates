// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  

using System;
using System.Collections.Generic;
using System.Text;
using KinTemplates.Cursor;

namespace CDPaint2.Canvas
{
    /// <summary>
    /// Stores all the lines drawn on the canvas.
    /// </summary>
    class Command
    {
        public RenderPath Render;

        public Command(RenderPath render)
        {
            this.Render = render;
        }

        public Command()
        {
            this.Render = new RenderPath();
        }
    }
}
