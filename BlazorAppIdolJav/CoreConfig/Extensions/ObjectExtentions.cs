
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace GameManagement.CoreConfig.Extensions

{
    public static class ObjectExtentions
    {
        public static T As<T>(this object source)
        {
            if (source == null)
            {
                return default;
            }
            var result = Activator.CreateInstance<T>();
            foreach (var item in typeof(T).GetProperties())
            {
                if (source.GetType().GetProperty(item.Name) != null)
                    item.SetValue(result, source.GetType().GetProperty(item.Name).GetValue(source) ?? null);
            }
            return result;
        }

        public static void Update(this object targetObject, object sourceObject, Dictionary<string, string> propertyFields = null)
        {
            if (propertyFields == null)
            {
                foreach (var item in targetObject.GetType().GetProperties())
                {
                    var info2 = sourceObject.GetType().GetProperty(item.Name);
                    if (info2 != null && item.PropertyType == info2.PropertyType)
                    {
                        item.SetValue(targetObject, info2.GetValue(sourceObject) ?? null);
                    }
                }
            }
            else
            {
                Type targetType = targetObject.GetType();
                Type sourceType = sourceObject.GetType();
                foreach (var item in propertyFields)
                {
                    var targetProperty = targetType.GetProperty(item.Key);
                    if (targetProperty != null)
                    {
                        var sourceProperty = sourceType.GetProperty(item.Value);
                        if (sourceProperty != null && targetProperty.PropertyType == sourceProperty.PropertyType)
                        {
                            try
                            {
                                var value = sourceProperty.GetValue(sourceObject);
                                targetProperty.SetValue(targetObject, value);
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        }
                    }
                }
            }
        }

        public static void UpdateIgnoreNull(this object targetObject, object sourceObject, Dictionary<string, string> propertyFields = null)
        {
            if (propertyFields == null)
            {
                foreach (var item in targetObject.GetType().GetProperties())
                {
                    var info2 = sourceObject.GetType().GetProperty(item.Name);
                    if (info2 != null && item.PropertyType == info2.PropertyType)
                    {
                        var value = info2.GetValue(sourceObject);
                        if (value != null)
                        {
                            if (value is DateTime?)
                            {
                                if (((DateTime?)value).HasValue && ((DateTime?)value) != DateTime.MinValue)
                                {
                                    item.SetValue(targetObject, value);
                                }
                            }
                            else
                            {
                                item.SetValue(targetObject, value);
                            }
                        }
                    }
                }
            }
            else
            {
                Type targetType = targetObject.GetType();
                Type sourceType = sourceObject.GetType();
                foreach (var item in propertyFields)
                {
                    var targetProperty = targetType.GetProperty(item.Key);
                    if (targetProperty != null)
                    {
                        var sourceProperty = sourceType.GetProperty(item.Value);
                        if (sourceProperty != null && targetProperty.PropertyType == sourceProperty.PropertyType)
                        {
                            try
                            {
                                var value = sourceProperty.GetValue(sourceObject);
                                if (value != null)
                                {
                                    targetProperty.SetValue(targetObject, value);
                                }
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        }
                    }
                }
            }
        }

        public static void UpdateIfNullOrEmpty(this object targetObject, object sourceObject, Dictionary<string, string> propertyFields = null)
        {
            if (propertyFields == null)
            {
                foreach (var targetProperty in targetObject.GetType().GetProperties())
                {
                    var sourceProperty = sourceObject.GetType().GetProperty(targetProperty.Name);
                    if (sourceProperty != null && targetProperty.PropertyType == sourceProperty.PropertyType)
                    {
                        var targetValue = targetProperty.GetValue(targetObject);
                        var sourceValue = sourceProperty.GetValue(sourceObject);
                        if (targetValue == null)
                        {
                            targetProperty.SetValue(targetObject, sourceValue);
                        }
                    }
                }
            }
            else
            {
                Type targetType = targetObject.GetType();
                Type sourceType = sourceObject.GetType();
                foreach (var item in propertyFields)
                {
                    var targetProperty = targetType.GetProperty(item.Key);
                    var sourceProperty = sourceType.GetProperty(item.Value);

                    if (targetProperty != null && sourceProperty != null && targetProperty.PropertyType == sourceProperty.PropertyType)
                    {
                        var targetValue = targetProperty.GetValue(targetObject);
                        var sourceValue = sourceProperty.GetValue(sourceObject);

                        if (targetValue == null)
                        {
                            targetProperty.SetValue(targetObject, sourceValue);
                        }
                    }
                }
            }
        }

        public static void Trim(this object targetObject)
        {
            try
            {
                foreach (var item in targetObject.GetType().GetProperties().Where(c => c.PropertyType == typeof(string)))
                {
                    var currentValue = item.GetValue(targetObject) as string;
                    if (currentValue.IsNotNullOrEmpty() && (currentValue[0] == ' ' || currentValue[currentValue.Length - 1] == ' '))
                    {
                        item.SetValue(targetObject, currentValue.Trim());
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// EncodeHtml object cho object
        /// </summary>
        /// <param name="targetObject"></param>
        public static T EncodeHtml<T>(T targetObject)
        {
            try
            {
                T tempObject = (T)Activator.CreateInstance(targetObject.GetType());
                tempObject.Update(targetObject);
                foreach (var item in tempObject.GetType().GetProperties().Where(c => c.PropertyType == typeof(string)))
                {
                    var currentValue = item.GetValue(tempObject) as string;
                    if (currentValue.IsNotNullOrEmpty())
                    {
                        item.SetValue(tempObject, HttpUtility.HtmlEncode(currentValue));
                    }
                }
                return tempObject;
            }
            catch (Exception)
            {
                return targetObject;
            }
        }

        /// <summary>
        /// EncodeHtml object cho List va object
        /// </summary>
        /// <param name="targetObject"></param>
        public static List<T> EncodeHtml<T>(List<T> targetObject)
        {
            try
            {
                List<T> tempObject = new List<T>();
                foreach (var item in targetObject as System.Collections.IList)
                {
                    tempObject.Add(EncodeHtml(item).As<T>());
                }
                return tempObject;
            }
            catch (Exception)
            {
                return targetObject;
            }
        }

        public static void SetValue<T>(this T obj, string nameProperty, object value)
        {
            try
            {
                var info = obj.GetType().GetProperty(nameProperty);
                CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
                DateTime dateTime;
                if (value is DBNull)
                {
                    return;
                }
                if (info != null)
                {
                    if (info.PropertyType == typeof(int))
                    {
                        value = new Regex(@"[^\d]+").Replace(value.ToString(), "");
                        info.SetValue(obj, Convert.ToInt32(value));
                    }
                    else if (info.PropertyType == typeof(int?))
                    {
                        if (value == null || value?.ToString() == "null" || value?.ToString() == "")
                        {
                            info.SetValue(obj, null);
                        }
                        else
                        {
                            //value = new Regex(@"[^\d]+").Replace(value.ToString(), "");
                            value = new Regex(@"[^\d-]+").Replace(value.ToString(), "");
                            info.SetValue(obj, Convert.ToInt32(value));
                        }
                    }
                    else if (info.PropertyType == typeof(decimal))
                    {
                        info.SetValue(obj, Convert.ToDecimal(value));
                    }
                    else if (info.PropertyType == typeof(decimal?))
                    {
                        if (value.IsNullOrEmpty() || value?.ToString() == "null")
                        {
                            info.SetValue(obj, null);
                        }
                        else
                        {
                            info.SetValue(obj, Convert.ToDecimal(value));
                        }
                    }
                    //else if (info.PropertyType == typeof(DateTime))
                    //{
                    //    DateTime.TryParseExact(value.ToString(), "dd/MM/yyyy", culture1, DateTimeStyles.None, out dateTime);
                    //    info.SetValue(obj, dateTime);
                    //}
                    else if (info.PropertyType == typeof(DateTime?))
                    {
                        if (value.IsNullOrEmpty() || value?.ToString() == "null")
                        {
                            info.SetValue(obj, null);
                        }
                        else
                        {
                            DateTime.TryParse(value.ToString(), currentCulture, DateTimeStyles.None, out dateTime);
                            info.SetValue(obj, dateTime);
                        }
                    }
                    else if (info.PropertyType == typeof(bool))
                    {
                        info.SetValue(obj, Convert.ToBoolean(value));
                    }
                    else if (info.PropertyType == typeof(bool?))
                    {
                        if (value.IsNotNullOrEmpty())
                        {
                            info.SetValue(obj, Convert.ToBoolean(value.ToString()));
                        }
                        else
                        {
                            info.SetValue(obj, null);
                        }
                    }
                    else if (info.PropertyType.IsEnum)
                    {
                        if (Enum.TryParse(info.PropertyType, value?.ToString(), true, out object convertValue))
                        {
                            info.SetValue(obj, convertValue);
                        }

                    }
                    else
                    {
                        info.SetValue(obj, value);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static object GetValue<T>(this T obj, string nameProperty)
        {
            var info = obj.GetType().GetProperty(nameProperty);
            if (info == null)
            {
                return null;
            }
            return info.GetValue(obj);
        }

        public static TResult GetExactValue<T, TResult>(this T obj, string nameProperty)
        {
            var info = obj.GetType().GetProperty(nameProperty);
            if (info == null)
            {
                return default(TResult);
            }
            return (TResult)info.GetValue(obj);
        }

        public static object GetValueField(this Type obj, string fieldName)
        {
            var info = obj.GetField(fieldName);
            if (info == null)
            {
                return null;
            }
            return info.GetValue(obj);
        }

        public static object GetValueField<T>(this T obj, string fieldName)
        {
            var info = obj.GetType().GetField(fieldName);
            if (info == null)
            {
                return null;
            }
            return info.GetValue(obj);
        }

        //public static IEnumerable<TSource> DistinctBy<TSource, TKey>
        //    (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        //{
        //    HashSet<TKey> seenKeys = new HashSet<TKey>();
        //    foreach (TSource element in source)
        //    {
        //        if (seenKeys.Add(keySelector(element)))
        //        {
        //            yield return element;
        //        }
        //    }
        //}

        public static void Add<T>(this IList<PropertyInfo> propertyInfos, Expression<Func<T, bool>> property)
        {
            propertyInfos.Add<T, bool>(property);
        }
        public static void Add<T>(this IList<PropertyInfo> propertyInfos, Expression<Func<T, string>> property)
        {
            propertyInfos.Add<T, string>(property);
        }
        public static void Add<T>(this IList<PropertyInfo> propertyInfos, Expression<Func<T, int>> property)
        {
            propertyInfos.Add<T, int>(property);
        }
        public static void Add<T>(this IList<PropertyInfo> propertyInfos, Expression<Func<T, decimal>> property)
        {
            propertyInfos.Add<T, decimal>(property);
        }
        public static void Add<T>(this IList<PropertyInfo> propertyInfos, Expression<Func<T, decimal?>> property)
        {
            propertyInfos.Add<T, decimal?>(property);
        }
        public static void Add<T>(this IList<PropertyInfo> propertyInfos, Expression<Func<T, DateTime?>> property)
        {
            propertyInfos.Add<T, DateTime?>(property);
        }
        public static void Add<T>(this IList<PropertyInfo> propertyInfos, Expression<Func<T, int?>> property)
        {
            propertyInfos.Add<T, int?>(property);
        }
        public static void Add<T>(this IList<PropertyInfo> propertyInfos, Expression<Func<T, long?>> property)
        {
            propertyInfos.Add<T, long?>(property);
        }
        public static void Add<T>(this IList<PropertyInfo> propertyInfos, Expression<Func<T, long>> property)
        {
            propertyInfos.Add<T, long>(property);
        }
        public static void Add<T>(this IList<PropertyInfo> propertyInfos, Expression<Func<T, IEnumerable<string>>> property)
        {
            propertyInfos.Add<T, IEnumerable<string>>(property);
        }

        public static void Add<T, TR>(this IList<PropertyInfo> propertyInfos, Expression<Func<T, TR>> property)
        {
            MemberExpression memberExpression = property.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("'property' không có body");
            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null || propertyInfo.ReflectedType == null)
                throw new ArgumentException(string.Format("Expression '{0}' can't be cast to an Operand.", property));
            propertyInfos.Add(propertyInfo);
        }

        public static void Add<T>(this ICollection<string> propertyInfos, Expression<Func<T, bool>> property)
        {
            propertyInfos.Add<T, bool>(property);
        }
        public static void Add<T>(this ICollection<string> propertyInfos, Expression<Func<T, string>> property)
        {
            propertyInfos.Add<T, string>(property);
        }
        public static void Add<T>(this ICollection<string> propertyInfos, Expression<Func<T, int>> property)
        {
            propertyInfos.Add<T, int>(property);
        }
        public static void Add<T>(this ICollection<string> propertyInfos, Expression<Func<T, decimal>> property)
        {
            propertyInfos.Add<T, decimal>(property);
        }
        public static void Add<T>(this ICollection<string> propertyInfos, Expression<Func<T, decimal?>> property)
        {
            propertyInfos.Add<T, decimal?>(property);
        }
        public static void Add<T>(this ICollection<string> propertyInfos, Expression<Func<T, DateTime?>> property)
        {
            propertyInfos.Add<T, DateTime?>(property);
        }
        public static void Add<T>(this ICollection<string> propertyInfos, Expression<Func<T, int?>> property)
        {
            propertyInfos.Add<T, int?>(property);
        }

        public static void Add<T, TR>(this ICollection<string> propertyInfos, Expression<Func<T, TR>> property)
        {
            MemberExpression memberExpression = property.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("'property' không có body");
            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null || propertyInfo.ReflectedType == null)
                throw new ArgumentException(string.Format("Expression '{0}' can't be cast to an Operand.", property));
            propertyInfos.Add(propertyInfo.Name);
        }

        public static bool Contains<T, TR>(this IList<PropertyInfo> propertyInfos, Expression<Func<T, TR>> property)
        {
            MemberExpression memberExpression = property.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("'property' không có body");
            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null || propertyInfo.ReflectedType == null)
                return false;
            if (propertyInfos.Contains(propertyInfo))
            {
                return true;
            }
            return false;
        }

        public static string GenerateGuid() => Guid.NewGuid().ToString("N");
        public static string GenerateCustomCode(int start = 1000, int end = 10000)
        {
            var random = new Random();
            int randomNumber = random.Next(start, end);
            return randomNumber.ToString();
        }

        public static void AddRangeWithCheckExist(this HashSet<string> source, IEnumerable<string> addItems)
        {
            foreach (var item in addItems)
            {
                if (!source.Contains(item))
                {
                    source.Add(item);
                }
            }
        }

        public static bool IsNullOrEmpty(this object input)
        {
            if (input == null || input.ToString().Length == 0 || input.ToString().Trim().Length == 0)
            {
                return true;
            }
            return false;
        }
        public static bool IsNotNullOrEmpty(this object input)
        {
            if (input == null || input.ToString().Length == 0 || input.ToString().Trim().Length == 0)
            {
                return false;
            }
            return true;
        }
        public static bool IsNotNullOrEmptyOrNotDBNull(this object input)
        {
            if (input == null || input?.ToString() == "null" || input == DBNull.Value || input.ToString().Length == 0 || input.ToString().Trim().Length == 0)
            {
                return false;
            }
            return true;
        }
        public static string RemoveSpecialCharacters(this string input)
        {
            string pattern = "[^\\w\\s]";
            return Regex.Replace(input, pattern, string.Empty);
        }
        public static DateTime? ConvertCultureVN(this string input)
        {
            CultureInfo culture1 = CultureInfo.CreateSpecificCulture("vi-VN");
            DateTime dateTime;
            string[] font = { "dd/MM/yyyy", "dd/MM/yyyy HH:mm:ss", "dd-MM-yyyy", "dd-MM-yyyy HH:mm:ss", "d/M/yyyy", "d/M/yyyy HH:mm:ss", "d-M-yyyy", "dd/MM/yyyy hh:mm:ss tt" };
            if (string.IsNullOrEmpty(input))
                return null;
            if (DateTime.TryParseExact(input.Trim(), font, culture1, DateTimeStyles.None, out dateTime))
                return dateTime;
            return null;
        }
        public static void Add(this List<KeyValuePair<int, int>> list, int i, int j)
        {
            list.Add(new KeyValuePair<int, int>(i, j));
        }

        /// <summary>
        /// Chuyển object về số nguyên. Nếu obj null hoặc empty hoặc không chuyển được trả về 0
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int ToInt(this object obj)
        {
            if (obj == null || obj.IsNullOrEmpty())
            {
                return 0;
            }
            try
            {
                int value;
                int.TryParse(obj.ToString(), out value);
                return value;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Kiểm tra chuối ký tự có thể chuyển thành số hay không kiểu 0.000,0
        /// </summary>
        /// <param name="pText">Chuỗi ký tự</param>
        /// <returns>True nếu chuối ký tự là số. False nếu ngược lại</returns>
        public static bool IsNumberFormatVN(this string pText)
        {
            if (pText.IsNullOrEmpty())
            {
                return false;
            }
            Regex regex = new Regex(@"^[-+]?[0-9.]*\,?[0-9]+$");
            return regex.IsMatch(pText);
        }

        public static void SetValue<T>(this DataRow row, string key, List<T> list, T item, List<KeyValuePair<int, int>> errors, int i, int j)
        {
            if (list.Contains(item))
            {
                row[key] = item;
            }
            else
            {
                errors.Add(i, j);
            }
        }

        public static void SetRowValue<T>(this DataRow row, string key, ICollection<T> list, T item, List<KeyValuePair<int, int>> errors, int i, int j)
        {
            if (list.Contains(item))
            {
                row[key] = item;
            }
            else
            {
                errors.Add(i, j);
            }
        }

        public static bool EqualValues<T>(T a1, T a2)
        {
            return EqualityComparer<T>.Default.Equals(a1, a2);
        }
        //public static List<KeyValuePair<string, string>> ToListKeyValuePair(this Dictionary<string, ISelectItem> sources, bool customDisplay = false)
        //{
        //    if (sources == null)
        //    {
        //        return new List<KeyValuePair<string, string>>();
        //    }
        //    else
        //    {
        //        if (customDisplay)
        //        {
        //            return sources.Select(c => new KeyValuePair<string, string>(c.Key, c.Value.GetCustomDisplay())).ToList();
        //        }
        //        else
        //        {
        //            return sources.Select(c => new KeyValuePair<string, string>(c.Key, c.Value.GetDisplay())).ToList();
        //        }
        //    }
        //}

        public static Decimal ToDecimal(this object obj)
        {
            CultureInfo culture1 = CultureInfo.CreateSpecificCulture("vi-VN");
            if (obj == null || obj.IsNullOrEmpty())
            {
                return 0;
            }
            decimal value;
            Decimal.TryParse(obj.ToString(), NumberStyles.Number, culture1, out value);
            return value;
        }

        public static Decimal ToDecimal(this object obj, CultureInfo culture)
        {
            if (culture == null)
            {
                throw new InvalidDataException("Culture is null");
            }
            if (obj == null || obj.IsNullOrEmpty())
            {
                return 0;
            }
            decimal value;
            Decimal.TryParse(obj.ToString(), NumberStyles.Number, culture, out value);
            return value;
        }

        /// <summary>
        /// Hàm chuyển số x dạng decimal về chuỗi theo format 
        /// Ví dụ:
        ///     x >= 100: 1000 => 1.000
        ///     x < 100: 50 => 50,00
        ///              3,5 => 3,5
        ///              3,55 => 3,55
        /// </summary>
        /// <param name="d_value"></param>
        /// <returns></returns>
        public static string ToDecimalFormated(this decimal? d_value, int numberAfterComma = 2)
        {
            if (d_value.IsNullOrEmpty())
            {
                return string.Empty;
            }
            var valueAfterComma = Math.Pow(10, numberAfterComma).ToInt();
            if (d_value > 0m && d_value < 100m && ((int)d_value * valueAfterComma) % valueAfterComma == 0)
            {
                // Yêu cầu chỉ có 2 số 0 sau dấu phẩy
                return d_value?.ToString("N2");
            }
            else
            {
                var strValue = d_value?.ToString("N" + numberAfterComma);
                if (strValue != null && strValue.Contains(','))
                {
                    return strValue.TrimEnd('0').TrimEnd(',');
                }
                else
                {
                    return strValue;
                }
            }
        }

        public static string ToFormatedNumber<T>(this T d_value, string format = "0.##0,#", bool clearZero = true)
        {
            if (d_value.IsNullOrEmpty())
            {
                return "0";
            }

            var strValue = string.Format("{0:" + format + "}", d_value);
            if (!clearZero)
            {
                return strValue;
            }
            if (strValue != null && strValue.Contains(','))
            {
                return strValue.TrimEnd('0').TrimEnd(',');
            }
            else
            {
                return strValue;
            }
        }

        public static string ToFormatedNumberEmpty<T>(this T d_value, string format = "0.##0,#")
        {
            if (d_value.IsNullOrEmpty())
            {
                return "";
            }

            var strValue = string.Format("{0:" + format + "}", d_value);
            if (strValue == "0,000")
            {
                return "";
            }
            if (strValue != null && strValue.Contains(','))
            {
                return strValue.TrimEnd('0').TrimEnd(',');
            }
            else
            {
                return strValue;
            }
        }

        public static string GetFormatNumber(this CultureInfo culture)
        {
            if (culture.Name == "vi")
            {
                return "0.##0,0";
            }
            else
            {
                return "0,##0.0";
            }
        }

        public static string ToDecimalFormated(this decimal d_value, int numberAfterComma = 2)
        {
            if (d_value.IsNullOrEmpty())
            {
                return string.Empty;
            }
            var valueAfterComma = Math.Pow(10, numberAfterComma).ToInt();
            if (d_value > 0m && d_value < 100m && ((int)d_value * valueAfterComma) % valueAfterComma == 0)
            {
                // Yêu cầu chỉ có 2 số 0 sau dấu phẩy
                return d_value.ToString("N" + numberAfterComma);
            }
            else
            {
                var strValue = d_value.ToString("N" + numberAfterComma);
                if (strValue != null && strValue.Contains(','))
                {
                    return strValue.TrimEnd('0').TrimEnd(',');
                }
                else
                {
                    return strValue;
                }
            }
        }

        public static string ToDecimalFormated1(this decimal? d_value, int numberAfterComma = 1)
        {
            if (d_value.IsNullOrEmpty())
            {
                return string.Empty;
            }
            var valueAfterComma = Math.Pow(10, numberAfterComma).ToInt();
            if (d_value > 0m && d_value < 100m && ((int)d_value * valueAfterComma) % valueAfterComma == 0)
            {
                // Yêu cầu chỉ có 1 số 0 sau dấu phẩy
                return d_value?.ToString("N" + numberAfterComma);
            }
            else
            {
                var strValue = d_value?.ToString("N" + numberAfterComma);
                if (strValue != null && strValue.Contains(','))
                {
                    return strValue.TrimEnd('0').TrimEnd(',');
                }
                else
                {
                    return strValue;
                }
            }
        }
        public static string ToDecimalFormated3(this decimal? d_value, int numberAfterComma = 3)
        {
            if (d_value.IsNullOrEmpty())
            {
                return string.Empty;
            }
            var valueAfterComma = Math.Pow(10, numberAfterComma).ToInt();
            if (d_value > 0m && d_value < 100m && ((int)d_value * valueAfterComma) % valueAfterComma == 0)
            {
                // Yêu cầu chỉ có 3 số 0 sau dấu phẩy
                return d_value?.ToString("N" + numberAfterComma);
            }
            else
            {
                var strValue = d_value?.ToString("N" + numberAfterComma);
                if (strValue != null && strValue.Contains(','))
                {
                    return strValue.TrimEnd('0').TrimEnd(',');
                }
                else
                {
                    return strValue;
                }
            }
        }
        public static string ToDecimalUnFormat(this decimal? d_value)
        {
            if (d_value.IsNullOrEmpty())
            {
                return string.Empty;
            }
            var strValue = d_value?.ToString("N");
            if (strValue != null && strValue.Contains(','))
            {
                return strValue.TrimEnd('0').TrimEnd(',');
            }
            else
            {
                return strValue;
            }
        }

        public static string ToDecimalUnFormat(this decimal d_value)
        {
            if (d_value.IsNullOrEmpty())
            {
                return string.Empty;
            }
            string value = d_value.ToString("N4");
            if (value.Contains(","))
            {
                return value.TrimEnd('0').TrimEnd(',');
            }
            else
            {
                return value;
            }
        }

        public static string ToDecimalUnFormatString(this decimal d_value)
        {
            if (d_value.IsNullOrEmpty())
            {
                return string.Empty;
            }
            string value = d_value.ToString();
            if (value.Contains("."))
            {
                return value.TrimEnd('0').TrimEnd('.');
            }
            else
            {
                return value;
            }
        }

        public static string ToDecimalUnFormat(this decimal d_value, IFormatProvider provider = null, int numberAfterComma = 3)
        {
            if (d_value.IsNullOrEmpty())
            {
                return string.Empty;
            }
            string value = provider != null ? d_value.ToString("N" + numberAfterComma, provider) : d_value.ToString("N" + numberAfterComma);
            if (value.Contains(","))
            {
                return value.TrimEnd('0').TrimEnd(',');
            }
            else
            {
                return value;
            }
        }

        public static string ToDecimalUnFormat(this decimal? d_value, IFormatProvider provider = null, int numberAfterComma = 3)
        {
            if (d_value.IsNullOrEmpty())
            {
                return string.Empty;
            }
            //use numberAfterComma
            string value = provider != null ? d_value?.ToString("N" + numberAfterComma, provider) : d_value?.ToString("N" + numberAfterComma);
            if (value.Contains(","))
            {
                return value.TrimEnd('0').TrimEnd(',');
            }
            else
            {
                return value;
            }
        }

        public static string ToDecimalUnFormat3(this decimal? d_value)
        {
            return ToDecimalUnFormat(d_value, null, 3);
        }

        public static string ToDecimalUnFormatDot(this string value)
        {
            if (value.Contains(","))
            {
                return value.Replace(",", ".");
            }
            return value;
        }

        public static string ConvertDotToComma(this string value)
        {
            if (value.Contains("."))
            {
                return value.Replace(".", ",");
            }
            return value;
        }

        public static void AddExist(this Dictionary<string, List<string>> dictionary, string key, string value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key].Add(value);
            }
            else
            {
                dictionary.Add(key, new List<string> { value });
            }
        }

        public static string DecimalToIntXml(this decimal? d_value)
        {
            if (d_value.IsNullOrEmpty())
            {
                return "0";
            }
            else
            {
                return d_value.ToString().Split(',')[0].Replace(".", "");
            }
        }
        public static string DecimalToFormatTyLeXml(this decimal? d_value)
        {
            if (d_value.IsNullOrEmpty())
            {
                return "0";
            }
            else
            {
                return d_value.ToString().Replace(".", "").Replace(",", ".");
            }
        }

        public static string GetDescription<T>(this T enumValue)
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                return null;

            var description = enumValue.ToString();
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    description = ((DescriptionAttribute)attrs[0]).Description;
                }
                else
                {
                    var displayAttrs = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), true);
                    if (displayAttrs != null && displayAttrs.Length > 0)
                    {
                        description = ((DisplayAttribute)displayAttrs[0]).Name;
                    }
                }
            }

            return description;
        }

        public static string GetDisplay(this PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                return null;
            var description = propertyInfo.Name;
            var attrs = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), true);
            if (attrs != null && attrs.Length > 0)
            {
                description = ((DisplayAttribute)attrs[0]).Name;
            }
            return description;
        }

        public static string GetDescription(this object enumValue)
        {
            if (enumValue == null)
            {
                return string.Empty;
            }
            var description = enumValue.ToString();
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    description = ((DescriptionAttribute)attrs[0]).Description;
                }
                else
                {
                    var displayAttrs = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), true);
                    if (displayAttrs != null && displayAttrs.Length > 0)
                    {
                        description = ((DisplayAttribute)displayAttrs[0]).Name;
                    }
                }
            }

            return description;
        }

        public static string ToEnumDescription<TEnum>(this string enumStr) where TEnum : Enum
        {
            if (Enum.TryParse(typeof(TEnum), enumStr, out var result))
            {
                return result.GetDescription();
            }
            throw new InvalidEnumArgumentException($"'{enumStr}' is not a valid value for enum '{typeof(TEnum).Name}'");
        }

        public static int ToEnumInt<TEnum>(this string enumString) where TEnum : Enum
        {
            if (Enum.TryParse(typeof(TEnum), enumString, out var result))
            {
                return Convert.ToInt32(result);
            }
            throw new InvalidEnumArgumentException($"'{enumString}' is not a valid value for enum '{typeof(TEnum).Name}'");
        }

        public static string ToEnumString<TEnum>(this int enumValue) where TEnum : Enum
        {
            if (Enum.IsDefined(typeof(TEnum), enumValue))
            {
                return ((TEnum)(object)enumValue).ToString();
            }
            throw new InvalidEnumArgumentException($"'{enumValue}' is not a valid value for enum '{typeof(TEnum).Name}'");
        }

        public static int EnumToInt<T>(this T enumObj)
        {
            if (!typeof(T).IsEnum)
                throw new InvalidEnumArgumentException("Method is only supported enum");
            return Convert.ToInt32(enumObj);
        }

        public static string EnumToString<T>(this T enumObj)
        {
            if (!typeof(T).IsEnum)
                throw new InvalidEnumArgumentException("Method is only supported enum");
            return Convert.ToInt32(enumObj).ToString();
        }

        public static string GetFriendlyUrl(this string str)
        {
            str = str.ToLower();
            str = Regex.Replace(str, @"[áàảạãăắẳằặẵâấầẩậẫ]", "a");
            str = Regex.Replace(str, @"[eéèẻẹẽêếềểệễ]", "e");
            str = Regex.Replace(str, @"[oõóòỏọôốồổộỗơớờởỡợ]", "o");
            str = Regex.Replace(str, @"[uủùúụưứừửựũữ]", "u");
            str = Regex.Replace(str, @"[iíìỉịĩ]", "i");
            str = Regex.Replace(str, @"[yýỳỷỵỹ]", "y");
            str = Regex.Replace(str, @"[đ]", "d");
            str = Regex.Replace(str, @"[^a-zA-Z0-9]", "-");
            str = Regex.Replace(str, "-+", "-");
            str = Regex.Replace(str, "^[-]", "");
            str = Regex.Replace(str, "[-]$", "");

            return str;
        }

        public static string GetBasicString(this string str)
        {
            str = str.ToLower();
            str = Regex.Replace(str, @"[áàảạãăắẳằặẵâấầẩậẫ]", "a");
            str = Regex.Replace(str, @"[eéèẻẹẽêếềểệễ]", "e");
            str = Regex.Replace(str, @"[oõóòỏọôốồổộỗơớờởỡợ]", "o");
            str = Regex.Replace(str, @"[uủùúụưứừửựũữ]", "u");
            str = Regex.Replace(str, @"[iíìỉịĩ]", "i");
            str = Regex.Replace(str, @"[yýỳỷỵỹ]", "y");
            str = Regex.Replace(str, @"[đ]", "d");
            str = Regex.Replace(str, @"[^a-zA-Z0-9]", "_");
            str = Regex.Replace(str, "_+", "_");
            str = Regex.Replace(str, "^[_]", "");
            str = Regex.Replace(str, "[_]$", "");

            return str;
        }

        public static string GetNonVietnameseString(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return str;
            }
            str = str.GetBasicString();
            str = Regex.Replace(str, "[_]+", " ");
            return GetUpperFirstWord(str);
        }

        public static string GetNormalName(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return str;
            }
            str = Regex.Replace(str, "[ ]+", " ");
            return GetUpperFirstWord(str);
        }

        public static string GetUpperFirstWord(string str)
        {
            if (str.IsNullOrEmpty())
            {
                return str;
            }
            var strArr = str.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in strArr)
            {
                var charArr = item.ToCharArray();
                charArr[0] = Char.ToUpper(charArr[0]);
                stringBuilder.Append(string.Concat(charArr[0].ToString().ToUpper(), item.AsSpan(1)) + " ");
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static IEnumerable<T> SelectManyRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            var result = source.SelectMany(selector);
            if (!result.Any())
            {
                return result;
            }
            return result.Concat(result.SelectManyRecursive(selector));
        }

        public static T GetAttribute<T>(this MemberInfo member, bool isRequired) where T : Attribute
        {
            var attribute = member.GetCustomAttributes(typeof(T), false).SingleOrDefault();

            if (attribute == null && isRequired)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The {0} attribute must be defined on member {1}",
                        typeof(T).Name,
                        member.Name));
            }

            return (T)attribute;
        }

        public static string GetPropertyDisplayName<T>(Expression<Func<T, object>> propertyExpression)
        {
            var memberInfo = GetPropertyInformation(propertyExpression.Body);
            if (memberInfo == null)
            {
                throw new ArgumentException(
                    "No property reference expression was found.",
                    "propertyExpression");
            }

            var attr = memberInfo.GetAttribute<DisplayAttribute>(false);
            if (attr == null)
            {
                return memberInfo.Name;
            }

            return attr.Name;
        }

        public static string GetDisplayName<T, TProp>(this T obj, Expression<Func<T, TProp>> propertyExpression)
        {
            var memberInfo = GetPropertyInformation(propertyExpression.Body);
            if (memberInfo == null)
            {
                throw new ArgumentException(
                    "No property reference expression was found.",
                    "propertyExpression");
            }

            var attr = memberInfo.GetAttribute<DisplayAttribute>(false);
            if (attr == null)
            {
                return memberInfo.Name;
            }

            return attr.Name;
        }

        public static string GetPropertyDisplayName<T>(string propertyName)
        {
            var memberInfo = GetPropertyInformation<T>(propertyName);
            if (memberInfo == null)
            {
                throw new ArgumentException(
                    "No property reference expression was found.",
                    "propertyExpression");
            }

            var attr = memberInfo.GetAttribute<DisplayAttribute>(false);
            if (attr == null)
            {
                return memberInfo.Name;
            }

            return attr.Name;
        }

        public static MemberInfo GetPropertyInformation(Expression propertyExpression)
        {
            Debug.Assert(propertyExpression != null, "propertyExpression != null");
            MemberExpression memberExpr = propertyExpression as MemberExpression;
            if (memberExpr == null)
            {
                UnaryExpression unaryExpr = propertyExpression as UnaryExpression;
                if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
                {
                    memberExpr = unaryExpr.Operand as MemberExpression;
                }
            }

            if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property)
            {
                return memberExpr.Member;
            }

            return null;
        }

        public static MemberInfo GetPropertyInformation<T>(string propertyName)
        {
            Debug.Assert(propertyName != null, "propertyName != null");
            var propery = typeof(T).GetProperty(propertyName);
            return propery as MemberInfo;
        }

        public static async Task CopyFileAsync(string sourceFile, string destinationFile, FileMode fileMode, CancellationToken cancellationToken)
        {
            var fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
            var bufferSize = 65536;

            using var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, fileOptions);
            using var destinationStream = new FileStream(destinationFile, fileMode, FileAccess.Write, FileShare.None, bufferSize, fileOptions);
            await sourceStream.CopyToAsync(destinationStream, bufferSize, cancellationToken)
                                       .ConfigureAwait(continueOnCapturedContext: false);
        }

        public static void SetDefaultPageParameter(ref int pageIndex, ref int pageSize, int pageIndexDefault = 1, int pageSizeDefault = 20)
        {
            if (pageIndex < 1)
            {
                pageIndex = pageIndexDefault;
            }
            if (pageSize <= 0 || pageSize > 50)
            {
                pageSize = pageSizeDefault;
            }
        }

        public static long ToLong(this object obj)
        {
            if (obj == null || obj.IsNullOrEmpty())
            {
                return 0;
            }
            long value;
            long.TryParse(obj.ToString(), out value);
            return value;
        }

        public static double ToDouble(this object obj)
        {
            if (obj == null || obj.IsNullOrEmpty())
            {
                return 0;
            }
            double value;
            Double.TryParse(obj.ToString(), out value);
            return value;
        }

        public static List<TR> AsList<T, TR>(this List<T> source)
        {
            return source.Select(c => c.As<TR>()).ToList();
        }

        public static List<T> Clone<T>(this List<T> source)
        {
            return source.Select(c => c.As<T>()).ToList();
        }

        public static T Clone<T>(this T source)
        {
            return source.As<T>();
        }

        public static DayOfWeek GetDayOfWeekByName(this string dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case "Thứ 2":
                case "Monday":
                    return DayOfWeek.Monday;
                case "Thứ 3":
                case "Tuesday":
                    return DayOfWeek.Tuesday;
                case "Thứ 4":
                case "Wednesday":
                    return DayOfWeek.Wednesday;
                case "Thứ 5":
                case "Thursday":
                    return DayOfWeek.Thursday;
                case "Thứ 6":
                case "Friday":
                    return DayOfWeek.Friday;
                case "Thứ 7":
                case "Saturday":
                    return DayOfWeek.Saturday;
                case "Chủ nhật":
                case "Sunday":
                    return DayOfWeek.Sunday;
                default:
                    return DayOfWeek.Sunday;
            }
        }

        public static string GetDayOfWeekByName(this DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    return "Thứ 2";
                case DayOfWeek.Tuesday:
                    return "Thứ 3";
                case DayOfWeek.Wednesday:
                    return "Thứ 4";
                case DayOfWeek.Thursday:
                    return "Thứ 5";
                case DayOfWeek.Friday:
                    return "Thứ 6";
                case DayOfWeek.Saturday:
                    return "Thứ 7";
                case DayOfWeek.Sunday:
                    return "Chủ nhật";
                default:
                    return null;
            }
        }

        public static T CheckAndDeserializeJson<T>(this string strInput)
        {
            var result = Activator.CreateInstance<T>();
            strInput = strInput.Trim();
            if (strInput.IsNotNullOrEmpty() &&
                ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]")))) //For array
            {
                try
                {
                    result = JsonSerializer.Deserialize<T>(strInput);
                    return result;
                }
                catch // not valid
                {
                    return result;
                }
            }
            else
            {
                return result;
            }
        }
        public static string ToDecimalFormatDot(this decimal? d_value)
        {
            var value = d_value.ToString();
            if (value.Contains("."))
            {
                return value.Replace(".", ",");
            }
            return value;
        }

        public static string ToDecimalFormatDot(this decimal d_value)
        {
            var value = d_value.ToString();
            if (value.Contains("."))
            {
                return value.Replace(".", ",");
            }
            return value;
        }

        public static string ToJsonString<T>(this T value)
        {
            if (value != null)
            {
                return JsonSerializer.Serialize(value);
            }
            else
            {
                return null;
            }
        }

        public static string ToJsonString<T>(this IEnumerable<T> value, JsonSerializerOptions options)
        {
            if (value != null)
            {
                return JsonSerializer.Serialize(value, options);
            }
            else
            {
                return null;
            }
        }

        public static string ToJsonDictionary<TKey, TValue>(this Dictionary<TKey, TValue> value, JsonSerializerOptions options)
        {
            return value != null ? JsonSerializer.Serialize(value, options) : null;
        }

        /// <summary>
        /// Safe parse json with prevent json injection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToCustomType<T>(this string value)
        {
            if (value.IsNullOrEmpty())
            {
                return default(T);
            }
            string sanitizedJsonString = SanitizeJsonString(value);
            return JsonSerializer.Deserialize<T>(sanitizedJsonString);
        }

        public static T ToCustomType<T>(this string value, JsonSerializerOptions options)
        {
            if (value.IsNullOrEmpty())
            {
                return default(T);
            }
            string sanitizedJsonString = SanitizeJsonString(value);
            return JsonSerializer.Deserialize<T>(sanitizedJsonString, options);
        }

        public static T ToCustomType<T>(this JsonElement element, JsonSerializerOptions options)
        {
            var json = element.GetRawText();
            return JsonSerializer.Deserialize<T>(json, options);
        }

        static string SanitizeJsonString(string jsonString)
        {
            // Null or empty check
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                throw new ArgumentException("Invalid JSON input: Input cannot be null or empty");
            }

            // Remove unexpected characters or potentially dangerous patterns
            jsonString = Regex.Replace(jsonString, @"[\n\r\t]", string.Empty); // Remove newlines, tabs
            jsonString = Regex.Replace(jsonString, @"[<>]", string.Empty);     // Remove angle brackets

            // Further custom sanitization logic could be added here

            // Validate that the sanitized string is still a valid JSON structure
            if (!IsValidJson(jsonString))
            {
                throw new ArgumentException("Invalid JSON input: Failed JSON validation");
            }

            return jsonString;
        }

        static bool IsValidJson(string jsonString)
        {
            try
            {
                JsonDocument.Parse(jsonString);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        public static DateTime GetStartDateInMonth(this DateTime? value)
        {
            return new DateTime(value.Value.Year, value.Value.Month, 1);
        }

        public static DateTime GetStartDateInMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }

        public static DateTime GetEndDateInMonth(this DateTime? value)
        {
            return value.GetStartDateInMonth().AddMonths(1).AddMilliseconds(-1);
        }

        public static DateTime GetEndDateInMonth(this DateTime value)
        {
            return value.GetStartDateInMonth().AddMonths(1).AddMilliseconds(-1);
        }

        public static string ConvertToCodeFormat(this string format)
        {
            if (format.IsNullOrEmpty())
            {
                return format;
            }
            return format.Replace(",", ";").Replace(".", ",").Replace(";", ".");
        }

        public static string ConvertToExcelFormat(this string format)
        {
            if (format.IsNullOrEmpty())
            {
                return format;
            }
            return format.Replace(".", ";").Replace(",", ".").Replace(";", ",");
        }

        public static string ToUsCultureString(this DateTime? dateTime)
        {
            var enCulture = new CultureInfo("en-Us");
            string currentPattern = enCulture.DateTimeFormat.ShortDatePattern;
            return dateTime?.ToString(currentPattern);
        }

        public static string ToVnCultureString(this DateTime? dateTime)
        {
            var enCulture = new CultureInfo("vi-Vn");
            string currentPattern = enCulture.DateTimeFormat.ShortDatePattern;
            return dateTime?.ToString(currentPattern);
        }

        public static int GetSmsCharacterCount(string message)
        {
            bool isGsm7 = Encoding.UTF8.GetBytes(message).Length == message.Length;
            if (isGsm7)
            {
                return message.Length;
            }
            else
            {
                return Encoding.Unicode.GetByteCount(message) / 2;
            }
        }

        public static T GetEnumByGetDescription<T>(this object enumDescription) where T : Enum
        {
            foreach (var enumValue in Enum.GetValues(typeof(T)))
            {
                var description = enumValue.GetDescription();
                if (description == enumDescription.ToString())
                {
                    return (T)enumValue;
                }
            }
            throw new ArgumentException($"There is no {nameof(T)} value with description {enumDescription}");
        }

        /// <summary>
        /// Merges two dictionaries into a single dictionary. 
        /// If there are duplicate keys, the values from the source dictionary will override those in the target dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionaries.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionaries.</typeparam>
        /// <param name="source">The dictionary whose values will override duplicates in the target dictionary.</param>
        /// <param name="dic">The initial dictionary to be merged into.</param>
        /// <returns>A new dictionary containing all keys and values from both input dictionaries, with conflicts resolved in favor of the source dictionary.</returns>
        public static Dictionary<TKey, TValue> MergeDictionaries<TKey, TValue>(
           Dictionary<TKey, TValue> source,
           Dictionary<TKey, TValue> dic)
        {
            var result = new Dictionary<TKey, TValue>(dic);

            foreach (var kvp in source)
            {
                result[kvp.Key] = kvp.Value;
            }
            return result;
        }

        public static string NormalizeString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            string normalized = input.Normalize(NormalizationForm.FormD);
            var charsWithoutDiacritics = normalized
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray();

            string cleanedString = new string(charsWithoutDiacritics)
                .Replace(" ", "")
                .Replace(",", "")
                .ToLowerInvariant();

            return cleanedString;
        }

        public static string ToBase64String<T>(this T value)
        {
            if (value == null)
            {
                return null;
            }
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value.ToJsonString()));
        }

        public static string GetEnumDisplayNames<TEnum>() where TEnum : Enum
        {
            return string.Join(", ", typeof(TEnum)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(f => f.GetCustomAttribute<DisplayAttribute>()?.Name)
                .Where(name => !string.IsNullOrEmpty(name)));
        }

        public static T? TryGetEnumFromDescription<T>(this string description) where T : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return null;
            }
            foreach (var enumValue in Enum.GetValues(typeof(T)))
            {
                var enumText = ((T)enumValue).GetDescription();
                if (string.Equals(enumText, description, StringComparison.InvariantCultureIgnoreCase))
                {
                    return (T)enumValue;
                }
            }
            return null;
        }

        public static string NormalizePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return null;
            }
            string normalized = Regex.Replace(phoneNumber, @"[^\d+]", "");
            if (normalized.StartsWith("+"))
            {
                normalized = "+" + Regex.Replace(normalized, @"[^\d]", "").TrimStart('+');
            }
            else
            {
                normalized = Regex.Replace(normalized, @"[^\d]", "");
            }
            string digitsOnly = Regex.Replace(normalized, @"\D", "");
            if (digitsOnly.Length < 9 || digitsOnly.Length > 10)
            {
                return null;
            }

            return normalized;
        }

        public static string NormalizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            return string.Join(" ", name.ToLower().Trim()
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(word => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word)));
        }

        /// <summary>
        /// Checks if a filename has a specific extension using case-insensitive comparison
        /// </summary>
        /// <param name="fileName">The filename to check</param>
        /// <param name="extension">The extension to check for (with or without the dot)</param>
        /// <returns>True if the filename has the specified extension</returns>
        public static bool HasExtension(this string fileName, string extension)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            extension = extension.StartsWith(".") ? extension : "." + extension;
            return fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks if a filename is of a common image file type (jpg, jpeg, png, gif, bmp)
        /// </summary>
        /// <param name="fileName">The filename to check</param>
        /// <returns>True if the filename is an image file</returns>
        public static bool IsImageFile(this string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            return fileName.HasExtension(".jpg") ||
                   fileName.HasExtension(".jpeg") ||
                   fileName.HasExtension(".png") ||
                   fileName.HasExtension(".gif") ||
                   fileName.HasExtension(".bmp");
        }

        /// <summary>
        /// Checks if a filename is a PDF file
        /// </summary>
        /// <param name="fileName">The filename to check</param>
        /// <returns>True if the filename is a PDF file</returns>
        public static bool IsPdfFile(this string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            return fileName.HasExtension(".pdf");
        }

        public static bool IsJsonEqualTo<T>(this T source, T other)
        {
            if (source == null && other == null)
            {
                return true;
            }
            if (source == null || other == null)
            {
                return false;
            }
            var sourceJson = JsonSerializer.Serialize(source);
            var otherJson = JsonSerializer.Serialize(other);
            return sourceJson == otherJson;
        }

        /// <summary>
        /// Tạo Expression<Func<object>> để lấy giá trị property từ type dựa trên property name (phiên bản an toàn)
        /// </summary>
        /// <param name="propertyName">Tên property cần lấy</param>
        /// <param name="type">Type chứa property</param>
        /// <param name="obj">Object instance</param>
        /// <returns>Expression<Func<object>> trả về giá trị của property hoặc null nếu property không tồn tại</returns>
        public static Expression<Func<object>> CreatePropertyExpressionSafe(this string propertyName, Type type, object obj)
        {
            try
            {
                var property = type.GetProperty(propertyName,BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                
                if (property == null)
                {
                    return null;
                }

                var constant = Expression.Constant(obj, type);
                
                var propertyAccess = Expression.Property(constant, property);
                
                var convertedProperty = Expression.Convert(propertyAccess, typeof(object));
                
                return Expression.Lambda<Func<object>>(convertedProperty);
            }
            catch
            {
                return null;
            }
        }

        public static int GetMissingOrdinalNumber<T>(List<T> items, Func<T, int> ordinalSelector)
        {
            if (items == null || items.Count == 0)
            {
                return 1;
            }
            var sorted = items.Select(ordinalSelector).Distinct().OrderBy(n => n).ToList();

            for (int i = 1; i <= sorted.Last(); i++)
            {
                if (!sorted.Contains(i))
                {
                    return i;
                }
            }
            return sorted.Last() + 1;
        }

        /// <summary>
        /// Generates the next string by pattern with custom number replacement
        /// </summary>
        /// <param name="list">Enumerable collection of strings to filter and analyze</param>
        /// <param name="pattern">Regex pattern to filter strings</param>
        /// <param name="numberPattern">Regex pattern to extract numbers from strings</param>
        /// <returns>New string with maximum number + 1 following the pattern</returns>
        public static string GenerateNextStringByPattern(IEnumerable<string> list, string pattern, string numberPattern)
        {
            if (list == null || !list.Any()  || string.IsNullOrEmpty(pattern) || string.IsNullOrEmpty(numberPattern))
            {
                return string.Empty;
            }

            try
            {
                var filterRegex = new Regex(pattern);
                var numberRegex = new Regex(numberPattern);
                
                var filteredStrings = list.Where(s => !string.IsNullOrEmpty(s) && filterRegex.IsMatch(s)).ToList();

                if (filteredStrings.Count == 0)
                {
                    return string.Empty;
                }

                var maxNumber = 0;
                foreach (var str in filteredStrings)
                {
                    var match = numberRegex.Match(str);
                    if (match.Success)
                    {
                        var numberStr = match.Value;
                        if (int.TryParse(numberStr, out int number))
                        {
                            maxNumber = Math.Max(maxNumber, number);
                        }
                    }
                }

                if (maxNumber == 0)
                {
                    return string.Empty;
                }
                var nextNumber = maxNumber + 1;
                var templateString = filteredStrings.First();
                var numberMatch = numberRegex.Match(templateString);
                var digitCount = numberMatch.Success ? numberMatch.Value.Length : 3;
                var paddedNext = digitCount > 0 ? nextNumber.ToString("D" + digitCount) : nextNumber.ToString();
                var newString = numberRegex.Replace(templateString, paddedNext);

                return newString;
            }
            catch(InvalidOperationException ex)
            {
                throw new Exception($"GenerateNextStringByPattern failed. pattern='{pattern}', numberPattern='{numberPattern}', message='{ex.Message}'", ex);
            }
        }

        public static int DoubleToInt(this object obj)
        {
            if (obj == null || obj.ToString().Trim() == string.Empty)
                return 0;

            try
            {
                if (double.TryParse(obj.ToString(), out var d))
                {
                    return (int)d;
                                   
                }

                if (int.TryParse(obj.ToString(), out var i))
                {
                    return i;
                }

                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex?.ToString());
            }
        }
    }
}
