using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test.Entity
{
    public partial class UserAdd
	{
		public UserAdd()
		{}
        #region Model
        private string _user;
        private string _pass;
        private int _phone;
        private string _role;
        /// <summary>
        /// 文件名称
        /// </summary>
        public string USER
        {
            set { _user = value; }
            get { return _user; }
        }
        /// <summary>
        /// 作者
        /// </summary>
        public string PASS
        {
            set { _pass = value; }
            get { return _pass; }
        }
        /// <summary>
        /// 来源
        /// </summary>
        public int PHONE
        {
            set { _phone = value; }
            get { return _phone; }
        }
        /// <summary>
        /// 角色
        /// </summary>
        public string ROLE
        {
            set { _role = value; }
            get { return _role; }
        }
        
        
        #endregion Model
    }
}