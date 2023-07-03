using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EL.BaseUI.Themes
{
    public class ReflectHelper
    {
        #region 实例

        /// <summary>
        ///     创建类实例, 可以替换工厂
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateInstance<T>() where T : new()
        {
            return (T)CreateInstance(typeof(T));
        }

        /// <summary>
        ///     创建类实例, 可以替换工厂
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static object CreateInstance(Type t)
        {
            return t.Assembly.CreateInstance(t.FullName);
        }

        #endregion

        #region 方法

        /// <summary>
        ///     动态调用方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodName">方法名</param>
        /// <param name="objs">方法参数, 个数与方法的参数一致,不然报错</param>
        /// <param name="isStatic">是否是静态方法</param>
        /// <returns></returns>
        public static object ExecuteMethod<T>(string methodName, object[] objs = null, bool isStatic = true)
            where T : new()
        {
            Type type = typeof(T);
            return isStatic ? type.InvokeMember(methodName, BindingFlags.InvokeMethod, null, null, objs) : type.InvokeMember(methodName, BindingFlags.InvokeMethod, null, CreateInstance<T>(), objs);
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取列值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static object GetPropertyValue<T>(T t, string propertyName) where T : new()
        {
            if (string.IsNullOrEmpty(propertyName) || t == null)
            {
                return string.Empty;
            }
            Type type = t.GetType();
            PropertyInfo propertyInfo = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (propertyInfo != null) return propertyInfo.GetValue(t, null);
            return null;
        }

        /// <summary>
        /// 设置列值，不区分大小写
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="propertyName">属性名</param>
        /// <param name="propertyValue">属性值</param>
        /// <returns></returns>
        public static void SetPropertyValue<T>(T t, string propertyName, string propertyValue) where T : new()
        {
            if (t == null || string.IsNullOrEmpty(propertyName) || string.IsNullOrEmpty(propertyValue))
            {
                return;
            }
            Type type = t.GetType();
            PropertyInfo propertyInfo = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (null != propertyInfo)
            {
                propertyInfo.SetValue(t, propertyValue, null);
            }
        }

        #endregion

        #region 版本

        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <param name="ass"></param>
        /// <returns></returns>
        public static Version GetVersion(Assembly ass)
        {
            return ass.GetName().Version;
        }

        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Version GetVersion<T>(T t)
        {
            return GetVersion(t.GetType().Assembly);
        }

        #endregion

        #region 程序集

        /// <summary>
        /// 动态加载，并创建类实例
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static object LoadAssembly(string filePath, string className)
        {
            if (!File.Exists(filePath)) { throw new Exception("路径不存在：" + filePath); }
            AppDomain domain = AppDomain.CreateDomain(new FileInfo(filePath).Name);
            return domain.CreateInstanceFromAndUnwrap(filePath, className);
        }

        /// <summary>
        /// 卸载
        /// </summary>
        /// <param name="domain"></param>
        public static void RemoveDomain(AppDomain domain)
        {
            AppDomain.Unload(domain);
        }

        #endregion
    }
}
