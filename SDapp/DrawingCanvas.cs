using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace SoftwareDesign_2017
{
    /// <summary>
    /// 此类引用自https://www.yuanmas.com/info/n4Ob3vvLzw.html，感谢博主
    /// </summary>
    class DrawingCanvas : Canvas
    {
        #region 字段
        private List<Visual> _visuals = new List<Visual>();
        #endregion

        #region 公有方法

        public void AddVisual(Visual visual)
        {
            _visuals.Add(visual);

            base.AddLogicalChild(visual);
            base.AddVisualChild(visual);
        }

        public void RemoveVisual(Visual visual)
        {
            _visuals.Remove(visual);

            base.RemoveLogicalChild(visual);
            base.RemoveVisualChild(visual);
        }

        public void RemoveAll()
        {
            while (_visuals.Count != 0)
            {
                base.RemoveLogicalChild(_visuals[0]);
                base.RemoveVisualChild(_visuals[0]);

                _visuals.RemoveAt(0);
            }
        }

        #endregion

        #region 构造

        public DrawingCanvas()
        {
            Width = 200;
            Height = 200;
        }
        #endregion

        #region 重写
        protected override int VisualChildrenCount
        {
            get
            {
                return _visuals.Count;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visuals[index];
        }
        #endregion
    }
}
