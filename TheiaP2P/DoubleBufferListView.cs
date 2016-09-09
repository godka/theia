using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace TheiaP2P
{
    public class DoubleBufferListView : ListView
    {

        public DoubleBufferListView()
        {

            SetStyle(ControlStyles.DoubleBuffer |

               ControlStyles.OptimizedDoubleBuffer |

               ControlStyles.AllPaintingInWmPaint, true);

            UpdateStyles();

        }

    }  

}
