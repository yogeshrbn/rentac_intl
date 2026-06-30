using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;
namespace BAL.Common
{
    public class DataCopier<T, Z>
    {
        public void CopyData(T source, Z destination)
        {

            Type t = typeof(T);
            BindingFlags bflags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;
            ConstructorInfo cInfo = typeof(T).GetConstructor(bflags, null, new Type[0] { }, null);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            PropertyDescriptorCollection desProps = TypeDescriptor.GetProperties(typeof(Z));
            foreach (PropertyDescriptor prop in properties)
            {
                PropertyDescriptor dProp = desProps.Find(prop.Name, false);
                if (dProp != null)
                {
                    //dProp.SetValue(source, prop.GetValue(source));
                    dProp.SetValue(destination, prop.GetValue(source));
                }
            }
        }
    }
}
