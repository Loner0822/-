using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ucPropertyGrid
{
    public class Property//属性类型
    {
        private string _description = string.Empty;//属性描述 默认为属性名称
        private object _value = null;//属性值
        private bool _readonly = false;//只读
        private bool _visible = true;//可见
        private string _category = string.Empty;//分类
        TypeConverter _converter = null;//类型转换
        object _editor = null;//编辑类型
        private string _displayname = string.Empty;//显示名称
        private string _propfdname = string.Empty;//字段
        private string _proptatype = string.Empty;//数据类型
        private string _propfkxfw = string.Empty;//可选范围
        private bool _isnum = false;

        public Property(string sName, object sValue)//构造方法
        {
            this._description = sName;
            this._value = sValue;
        }
        public Property(string sName, object sValue, bool sReadonly, bool sVisible)//构造方法
        {
            this._description = sName;
            this._value = sValue;
            this._readonly = sReadonly;
            this._visible = sVisible;
        }
        public string PropDataType  //获得数据类型
        {
            get
            {
                return _proptatype;
            }
            set
            {
                _proptatype = value;
            }
        }
        public string PropKxfw  //获得可选范围
        {
            get
            {
                return _propfkxfw;
            }
            set
            {
                _propfkxfw = value;
            }
        }
        public bool isNum
        {
            get
            {
                return _isnum;
            }
            set
            {
                _isnum = value;
            }
        }
        public string Description  //获得属性描述
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        public string FdName  //字段
        {
            get
            {
                return _propfdname;
            }
            set
            {
                _propfdname = value;
            }
        }


        public string DisplayName   //属性显示名称  
        {
            get
            {
                return _displayname;
            }
            set
            {
                _displayname = value;
            }
        }
        public string Category  //属性所属类别  
        {
            get
            {
                return _category;
            }
            set
            {
                _category = value;
            }
        }

        public object Value  //属性值  
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public bool ReadOnly  //是否为只读属性  
        {
            get
            {
                return _readonly;
            }
            set
            {
                _readonly = value;
            }
        }
        public bool Visible  //是否可见  
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
            }
        }

        public TypeConverter Converter  //类型转换器，我们在制作下拉列表时需要用到  
        {
            get
            {
                return _converter;
            }
            set
            {
                _converter = value;
            }
        }

        public virtual object Editor   //属性编辑器  
        {
            get
            {
                return _editor;
            }
            set
            {
                _editor = value;
            }
        }
    }
}
