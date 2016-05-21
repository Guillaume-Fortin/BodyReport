using BodyReport.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace BodyReport.Services
{
    /// <summary>
    /// Data base service. Import and export datatable
    /// </summary>
    public class DatabaseService
    {
        private readonly ApplicationDbContext _dbContext = null;
        private readonly IHostingEnvironment _env;
        private readonly ILogger _logger;
        string _exportPath;
        string _importPath;
        bool _insertOnly = true;
        bool _dataExported = false;
        string _fileName;
        char _tableSeparator = (char)1;
        char _headerTableBeginner = (char)2;
        char _headerfieldBeginner = (char)3;
        char _fieldSeparator = (char)4;

        public DatabaseService(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _dbContext = new ApplicationDbContext();
            _env = env;
            _logger = loggerFactory.CreateLogger<DatabaseService>();
            _exportPath = Path.Combine(_env.WebRootPath, "datas", "export");
            _importPath = Path.Combine(_env.WebRootPath, "datas", "import");
            _fileName = Guid.NewGuid().ToString() + ".txt";

            if (!Directory.Exists(_exportPath))
                Directory.CreateDirectory(_exportPath);
            if (!Directory.Exists(_importPath))
                Directory.CreateDirectory(_importPath);
        }

        private PropertyInfo GetDbSetPropertyInfo(string tableName)
        {
            return _dbContext.GetType().GetRuntimeProperties().Where(o =>
                o.PropertyType.IsConstructedGenericType &&
                o.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                o.Name == tableName).FirstOrDefault();
        }

        private List<object> _rowList = new List<object>();

        internal void ImportDataTables(IFormFile formFile, bool insertOnly)
        {
            _insertOnly = insertOnly;
            if (formFile == null)
                return;
            
            string line, tableName, fields;
            string[] fieldsSplit;
            PropertyInfo dbSetPropertyInfo = null;
            object dbSet = null;
            List<PropertyInfo> propertyInfoList = new List<PropertyInfo>();
            var importDataInDbSetMethodInfo = this.GetType().GetRuntimeMethods().Where(m => m.Name == "ImportDataInDbSet").FirstOrDefault();
            var commitDataMethodInfo = this.GetType().GetRuntimeMethods().Where(m => m.Name == "CommitData").FirstOrDefault();
            MethodInfo importDataInDbSetGenericMethodInfo = null;
            MethodInfo commitDataGenericMethodInfo = null;
            using (var stream = formFile.OpenReadStream())
            {
                using (var textReader = new StreamReader(stream, Encoding.UTF8))
                {
                    while ((line = textReader.ReadLine()) != null)
                    {
                        if (line[0] == _tableSeparator)
                        {
                            if(dbSet != null && commitDataGenericMethodInfo != null) // Security commit data
                                commitDataGenericMethodInfo.Invoke(this, new object[] { dbSet, null, true });

                            tableName = line.Substring(1);
                            propertyInfoList.Clear();
                            dbSetPropertyInfo = GetDbSetPropertyInfo(tableName);
                            if (dbSetPropertyInfo == null)
                            {
                                importDataInDbSetGenericMethodInfo = null;
                                commitDataGenericMethodInfo = null;
                            }
                            else
                            {
                                dbSet = dbSetPropertyInfo.GetValue(_dbContext);
                                Type rowType = dbSetPropertyInfo.PropertyType.GenericTypeArguments[0];
                                importDataInDbSetGenericMethodInfo = importDataInDbSetMethodInfo.MakeGenericMethod(dbSetPropertyInfo.PropertyType.GenericTypeArguments[0]);
                                commitDataGenericMethodInfo = commitDataMethodInfo.MakeGenericMethod(dbSetPropertyInfo.PropertyType.GenericTypeArguments[0]);
                            }
                        }
                        else if (line[0] == _headerTableBeginner)
                        {
                            fields = line.Substring(1);
                            if (string.IsNullOrWhiteSpace(fields))
                                continue;
                            fieldsSplit = fields.Split(_fieldSeparator);
                            if (fieldsSplit == null || fieldsSplit.Length == 0)
                                continue;

                            Type rowType = dbSetPropertyInfo.PropertyType.GenericTypeArguments[0];
                            PropertyInfo propertyInfo;
                            foreach (string fieldName in fieldsSplit)
                            {
                                if (!string.IsNullOrWhiteSpace(fieldName))
                                {
                                    propertyInfo = rowType.GetRuntimeProperty(fieldName);
                                    if (propertyInfo != null)
                                        propertyInfoList.Add(propertyInfo);
                                    else
                                        propertyInfoList.Add(null);
                                }
                                else
                                    propertyInfoList.Add(null);
                            }
                        }
                        else if (line[0] == _headerfieldBeginner && dbSetPropertyInfo != null && propertyInfoList.Count > 0)
                        {
                            if (importDataInDbSetGenericMethodInfo != null && commitDataGenericMethodInfo != null)
                            {
                                object rowInstance = importDataInDbSetGenericMethodInfo.Invoke(this, new object[] { dbSet, line.Substring(1), propertyInfoList });
                                if (rowInstance != null)
                                    commitDataGenericMethodInfo.Invoke(this, new object[] { dbSet, rowInstance, false });
                            }
                        }
                    }
                }
            }
            if (dbSet != null && commitDataGenericMethodInfo != null) // Security commit data
                commitDataGenericMethodInfo.Invoke(this, new object[] { dbSet, null, true });
        }

        int count = 0;
        
        private void CommitData<TEntity>(InternalDbSet<TEntity> dbSet, TEntity rowInstance, bool forceCommit) where TEntity : class
        {
            if (dbSet == null)
                return;

            if (rowInstance != null)
            {
                _rowList.Add(rowInstance);
            }

            if (_rowList.Count > 0 && (forceCommit || _rowList.Count % 5000 == 0)) //Commit all 500 row
            {
                count += _rowList.Count;
                _logger.LogInformation("Count : " + count);
                List<TEntity> list = new List<TEntity>();
                foreach (TEntity row in _rowList)
                {
                    if(row != null)
                        list.Add(row);
                }
                _rowList.Clear();
                if (list.Count > 0)
                { 
                    dbSet.AddRange(list);
                    _dbContext.SaveChanges();
                }
            }
            
        }

        private static Expression StackExpression(List<Expression> expressionList)
        {
            Expression expression = null;
            foreach (var exp in expressionList)
            {
                if (expression == null)
                {
                    expression = exp;
                }
                else
                {
                    expression = Expression.And(expression, exp);
                }
            }
            return expression;
        }

        private TEntity ImportDataInDbSet<TEntity>(InternalDbSet<TEntity> dbSet, string fieldValues, List<PropertyInfo> propertyInfoList) where TEntity : class
        {
            if (dbSet == null || propertyInfoList == null)
                return null;

            string[] fieldValuesSplit = fieldValues.Split(_fieldSeparator);
            if (fieldValuesSplit == null || fieldValuesSplit.Length == 0)
                return null;

            if (!_insertOnly)
            {
                var entityType = _dbContext.Model.FindEntityType(typeof(TEntity));
                var keys = entityType.GetKeys();

                object convertedValue;
                string fieldValue;
                var entityParameter = Expression.Parameter(typeof(TEntity), "e");
                Expression expression;
                PropertyInfo propertyInfo;
                List<Expression> expressionList = new List<Expression>();
                foreach (var key in keys)
                {
                    foreach (var keyProperty in key.Properties)
                    {
                        for (int i = 0; i < propertyInfoList.Count; i++)
                        {
                            fieldValue = fieldValuesSplit[i];
                            propertyInfo = propertyInfoList[i];
                            if (propertyInfo == null)
                                continue;
                            convertedValue = ConvertField(propertyInfo, fieldValue);
                            if (propertyInfo.Name == keyProperty.Name)
                            {
                                if (propertyInfo.PropertyType != typeof(string))
                                {
                                    expressionList.Add(Expression.Equal(
                                            Expression.Property(entityParameter, propertyInfo),
                                            Expression.Constant(convertedValue)
                                           ));
                                }
                                else
                                {
                                    MemberExpression m = Expression.MakeMemberAccess(entityParameter, propertyInfo);
                                    ConstantExpression c = Expression.Constant(convertedValue, typeof(string));
                                    MethodInfo mi = typeof(string).GetMethod("Equals", new Type[] { typeof(string), typeof(StringComparison) });
                                    Expression expressionTmp = Expression.Call(m, mi, c, Expression.Constant(StringComparison.OrdinalIgnoreCase));
                                    expressionList.Add(Expression.Equal(expressionTmp, Expression.Constant(true)));
                                }
                            }
                        }
                    }
                }

                expression = StackExpression(expressionList);
                var lambda = Expression.Lambda(expression, entityParameter) as Expression<Func<TEntity, bool>>;
                var row = dbSet.Where(lambda).FirstOrDefault();

                if (row == null)
                {
                    row = (TEntity)Activator.CreateInstance(typeof(TEntity));
                    ManageRow(row, fieldValuesSplit, propertyInfoList);
                    return row;
                }
                else
                    ManageRow(row, fieldValuesSplit, propertyInfoList);
            }
            else
            {
                var row = (TEntity)Activator.CreateInstance(typeof(TEntity));
                ManageRow(row, fieldValuesSplit, propertyInfoList);
                return row;
            }

            return null;
        } 
         
        private void ManageRow<TEntity>(TEntity row, string[] fieldValuesSplit, List<PropertyInfo> propertyInfoList)
        {
            object convertedValue;
            string fieldValue;
            PropertyInfo propertyInfo;
            for (int i=0; i < fieldValuesSplit.Length; i++)
            {
                fieldValue = fieldValuesSplit[i];
                propertyInfo = propertyInfoList[i];
                if (propertyInfo == null)
                    continue;
                
                convertedValue = ConvertField(propertyInfo, fieldValue);
                propertyInfo.SetValue(row, convertedValue);
            }
        }

        private object ConvertField(PropertyInfo propertyInfo, string fieldValue)
        {
            Type type = propertyInfo.PropertyType;
            type = Nullable.GetUnderlyingType(type) ?? type;

            if(type == typeof(DateTimeOffset))
            {
                if(string.IsNullOrWhiteSpace(fieldValue))
                    return null;
                else
                    return DateTimeOffset.Parse(fieldValue);
            }
            else if (type == typeof(DateTime))
            {
                if (string.IsNullOrWhiteSpace(fieldValue))
                    return null;
                else
                    return DateTime.Parse(fieldValue);
            }

            return Convert.ChangeType(fieldValue, type);
        }

        internal void ExportDataTables(List<string> tableNameList)
        {
            if (tableNameList != null)
            {
                foreach (string tableName in tableNameList)
                {
                    ExportDataTable(tableName);
                }
            }
        }

        internal void ExportDataTable(string tableName)
        {
            var dbSet = GetDbSetPropertyInfo(tableName);

            if (dbSet != null)
            {
                var methodInfo = this.GetType().GetRuntimeMethods().Where(m => m.Name == "ExportDataDbSet").FirstOrDefault();
                if (methodInfo != null)
                {
                    var value = dbSet.GetValue(_dbContext);
                    Type delegateType = (typeof(InternalDbSet<>).MakeGenericType(dbSet.PropertyType.GenericTypeArguments[0]));
                    methodInfo = methodInfo.MakeGenericMethod(dbSet.PropertyType.GenericTypeArguments[0]);
                    methodInfo.Invoke(this, new object[] { tableName, value });
                }
            }
        }

        private void ExportDataDbSet<TEntity>(string tableName, InternalDbSet<TEntity> dbSet) where TEntity : class
        {
            try
            {
                if (dbSet == null)
                    return;

                TypeInfo typeInfo;
                object value;
                using (FileStream fs = new FileStream(Path.Combine(_exportPath, _fileName), FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter sr = new StreamWriter(fs, Encoding.UTF8))
                    {
                        Type tableType = typeof(TEntity);

                        sr.Write(_tableSeparator);
                        sr.Write(tableName); //table name
                        sr.WriteLine();

                        var fieldProperties = tableType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                        if (fieldProperties != null)
                        {
                            sr.Write(_headerTableBeginner);
                            foreach (var fieldProperty in fieldProperties)
                            {
                                typeInfo = fieldProperty.PropertyType.GetTypeInfo();
                                if (typeInfo.IsPrimitive || typeInfo.IsValueType || fieldProperty.PropertyType == typeof(string))
                                {
                                    sr.Write(fieldProperty.Name);
                                    sr.Write(_fieldSeparator);
                                }
                            }
                            sr.WriteLine();

                            foreach (TEntity entity in dbSet)
                            {
                                if (entity == null)
                                    continue;
                                sr.Write(_headerfieldBeginner);
                                foreach (var fieldProperty in fieldProperties)
                                {
                                    typeInfo = fieldProperty.PropertyType.GetTypeInfo();
                                    if (typeInfo.IsPrimitive || typeInfo.IsValueType || fieldProperty.PropertyType == typeof(string))
                                    {
                                        value = fieldProperty.GetValue(entity);
                                        sr.Write(value);
                                        sr.Write(_fieldSeparator);
                                    }
                                }
                                sr.WriteLine();
                            }
                        }
                    }
                }
            }
            catch(Exception except)
            {
                _logger.LogError("ExportDataTable failed", except);
            }
        }
    }
}
