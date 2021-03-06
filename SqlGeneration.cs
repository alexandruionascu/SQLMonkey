﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;

namespace SQLMonkey {

    class SQLGeneration {

        private static IEnumerable<string> getColumnNames<T>() {
            var properties = typeof(T).GetProperties();
            return properties.Select(x => x.Name);
        }

        private static IEnumerable<Tuple<string, object>> getPropValuePairs<T>(T instance) {
            Func<object, object> encapsulateString = 
                (x) => x is string ? String.Format("'{0}'", x) : x;
            return typeof(T).GetProperties().Select(
                x => Tuple.Create(
                    x.Name,
                    encapsulateString(
                        instance.GetType().GetProperty(x.Name).GetValue(instance, null)
                    )
                )
            );
        }

        public static string generateInsertQuery<T>(T instance, string tableName) {
            var propValuePairs = getPropValuePairs<T>(instance);

            return String.Format("INSERT INTO {0} ({1}) VALUES ({2});",
                tableName,
                String.Join(",", propValuePairs.Select(x => x.Item1)),
                String.Join(",", propValuePairs.Select(x => x.Item2))
            );
        }

        public static string generateRetrieveQuery(string tableName) {
            return String.Format("SELECT * FROM {0};", tableName);
        }

        private static IEnumerable<PropertyInfo> getPrimaryKeyProperties<T>() {
            return typeof(T).GetProperties().Where(
               prop => prop.GetCustomAttributes().Any(
                   attr => attr.GetType() == typeof(PrimaryKeyAttribute)
               )
            );
        }


        public static string generateDeleteQuery<T>(T instance, string tableName) {
            if (getPrimaryKeyProperties<T>().Count() == 0) {
                throw new Exception("Primary key attribute is not defined.");
            }

            var primaryKeyProperties = getPrimaryKeyProperties<T>();
            var propValuePairs = getPropValuePairs<T>(instance);
            var condition = String.Join(" AND ",
                propValuePairs.Where(
                    x => primaryKeyProperties.Any(
                        prop => prop.Name == x.Item1
                    )
                ).Select(
                    x => String.Format("{0} = {1}", x.Item1, x.Item2)
                )
            );
            
            return String.Format("DELETE FROM {0} WHERE {1};", tableName, condition);
        }

        public static string generateUpdateQuery<T>(T instance, string tableName) {
            if (getPrimaryKeyProperties<T>().Count() == 0) {
                throw new Exception("Primary key attribute is not defined.");
            }

            var primaryKeyProperties = getPrimaryKeyProperties<T>();
            var propValuePairs = getPropValuePairs<T>(instance);
            var setCondition = String.Join(", ",
                propValuePairs.Where(
                    x => !primaryKeyProperties.Any(
                        prop => prop.Name == x.Item1
                    )
                ).Select(
                    x => String.Format("{0} = {1}", x.Item1, x.Item2)
                )
            );
            var whereCondition = String.Join(" AND ",
                propValuePairs.Where(
                    x => primaryKeyProperties.Any(
                        prop => prop.Name == x.Item1
                    )
                ).Select(
                    x => String.Format("{0} = {1}", x.Item1, x.Item2)
                )
            );

            return String.Format(
                "UPDATE {0} SET {1} WHERE {2};", tableName, setCondition, whereCondition
            );
        }
    }
}
