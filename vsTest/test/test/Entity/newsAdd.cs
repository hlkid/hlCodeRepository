using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test.Entity
{
    public partial class newsAdd
    {
        public newsAdd()
        { }
        #region Model
        private string _newstitle;
        private string _newssummary;
        private string _newstype;
        private string _newscontent;
        private string _newsimage;
        /// <summary>
        /// 新闻标题
        /// </summary>
        public string NEWS_TITLE
        {
            set { _newstitle = value; }
            get { return _newstitle; }
        }
        /// <summary>
        /// 新闻摘要
        /// </summary>
        public string NEWS_SUMMARY
        {
            set { _newssummary = value; }
            get { return _newssummary; }
        }
        /// <summary>
        /// 新闻类型
        /// </summary>
        public string NEWS_TYPE
        {
            set { _newstype = value; }
            get { return _newstype; }
        }
        /// <summary>
        /// 新闻内容
        /// </summary>
        public string NEWS_CONTENT
        {
            set { _newscontent = value; }
            get { return _newscontent; }
        }
        /// <summary>
        /// 新闻图片
        /// </summary>
        public string NEWS_IMAGE
        {
            set { _newsimage = value; }
            get { return _newsimage; }
        }


        #endregion Model
    }
}