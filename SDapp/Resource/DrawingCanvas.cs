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
    /// 此类的实现参考https://www.yuanmas.com/info/n4Ob3vvLzw.html，感谢博主
    /// </summary>
    class DrawingCanvas : Canvas
    {
        #region 字段
        private List<Visual> _visuals = new List<Visual>();
        #endregion

        #region 公有方法
        /// <summary>
        /// 将visual对象添加到视觉树和逻辑树
        /// </summary>
        /// <param name="visual">要添加的visual对象</param>
        public void AddVisual(Visual visual)
        {
            _visuals.Add(visual);

            base.AddLogicalChild(visual);//将对象添加到逻辑树
            base.AddVisualChild(visual);//定义视觉对象的父子关系
        }

        /// <summary>
        /// 将指定的visual对象从视觉树和逻辑树移除
        /// </summary>
        /// <param name="visual"></param>
        public void RemoveVisual(Visual visual)
        {
            _visuals.Remove(visual);

            base.RemoveLogicalChild(visual);
            base.RemoveVisualChild(visual);
        }

        /// <summary>
        /// 将逻辑树和视觉树全部清空
        /// </summary>
        public void RemoveAll()
        {
            while (_visuals.Count != 0)
            {
                base.RemoveLogicalChild(_visuals[0]);
                base.RemoveVisualChild(_visuals[0]);

                _visuals.RemoveAt(0);
            }
        }

        /// <summary>
        /// 在指定的索引出移除一个visual对象
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            base.RemoveLogicalChild(_visuals[index]);
            base.RemoveVisualChild(_visuals[index]);
            _visuals.RemoveAt(index);
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

        /// <summary>
        /// 获取指定索引的visual对象
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>索引到的visual对象</returns>
        protected override Visual GetVisualChild(int index)
        {
            return _visuals[index];
        }
        #endregion
    }
}
