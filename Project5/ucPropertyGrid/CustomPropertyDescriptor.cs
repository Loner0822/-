using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ucPropertyGrid
{
    public class CustomPropertyDescriptor : PropertyDescriptor
    {
        Property m_Property;
        public CustomPropertyDescriptor(ref Property myProperty, Attribute[] attrs)
            : base(myProperty.Description, attrs)
        {
            m_Property = myProperty;
        }

        #region PropertyDescriptor 重写方法

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get
            {
                return null;
            }
        }

        public override object GetValue(object component)
        {
            return m_Property.Value;
        }

        public override string Description
        {
            get
            {
                return m_Property.Description;
            }
        }
        public override string Category
        {
            get
            {
                return m_Property.Category;
            }
        }
        public override string DisplayName
        {
            get
            {
                return m_Property.DisplayName != "" ? m_Property.DisplayName : m_Property.Description;
            }
        }
        public override bool IsReadOnly
        {
            get
            {
                return m_Property.ReadOnly;
            }
        }
        public override void ResetValue(object component)
        {
            //Have to implement  
        }
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
        public override void SetValue(object component, object value)
        {
            m_Property.Value = value;
        }
        public override TypeConverter Converter
        {
            get
            {
                return m_Property.Converter;
            }
        }
        public override Type PropertyType
        {
            get { return m_Property.Value.GetType(); }
        }
        public override object GetEditor(Type editorBaseType)
        {
            return m_Property.Editor == null ? base.GetEditor(editorBaseType) : m_Property.Editor;
        }
        #endregion
    }
}
