﻿//
//  Copyright 2013, Desert Software Solutions Inc.
//    Database.cs: https://gist.github.com/rostreim/9453953
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//

using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using Dapper;

/// <summary>
/// Generic methods for accessing a database using dapper.net
/// </summary>
public class OleDbDatabase
{
    private ConnectionFactory connectionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="Database"/> class.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="provider">The provider.</param>
    /// <param name="mapper">The mapper.</param>
    internal OleDbDatabase(string connectionString, string provider = "System.Data.OleDb", Mapper mapper = null) {
        this.connectionFactory = new ConnectionFactory(connectionString, provider);
        if (mapper != null)
            mapper.Initialize();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Database"/> class.
    /// </summary>
    /// <param name="connectionFactory">The <see cref="ConnectionFactory"/> instance.</param>
    /// <param name="mapper">The mapper.</param>
    internal OleDbDatabase(ConnectionFactory connectionFactory, Mapper mapper = null) {
        this.connectionFactory = connectionFactory;
        if (mapper != null)
            mapper.Initialize();
    }

    /// <summary>
    /// Creates the connection.
    /// </summary>
    /// <returns></returns>
    public OleDbConnection CreateConnection() {
        return this.connectionFactory.CreateConnection();
    }

    /// <summary>
    /// Executes the specified SQL.
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns></returns>
    public int Execute(string sql, dynamic parameters = null, CommandType? commandType = null) {
        using (OleDbConnection connection = CreateConnection()) {
            connection.Open();

            try {
                return Execute(connection, sql, parameters, commandType);
            } finally {
                connection.Close();
            }
        }
    }

    /// <summary>
    /// Executes the specified SQL on the specified connection.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns></returns>
    public int Execute(IDbConnection connection, string sql, dynamic parameters = null, CommandType? commandType = null) {
        return connection.Execute(sql, new DynamicParameters(parameters), commandTimeout: 0, commandType: commandType);
    }

    /// <summary>
    /// Returns the first result for the specified SQL.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sql">The SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns></returns>
    public T First<T>(string sql, dynamic parameters = null, CommandType? commandType = null) {
        using (OleDbConnection connection = CreateConnection()) {
            connection.Open();

            try {
                return First<T>(connection, sql, parameters, commandType);
            } finally {
                connection.Close();
            }
        }
    }

    /// <summary>
    /// Returns the first result for the specified SQL.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection">The connection.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns></returns>
    public T First<T>(OleDbConnection connection, string sql, dynamic parameters = null, CommandType? commandType = null) {
        return connection.Query<T>(sql, new DynamicParameters(parameters), commandTimeout: 0, commandType: commandType).FirstOrDefault();
    }

    /// <summary>
    /// Returns the result list of the specified SQL.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sql">The SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns></returns>
    public List<T> ListOf<T>(string sql, dynamic parameters = null, CommandType? commandType = null) {
        using (OleDbConnection connection = CreateConnection()) {
            connection.Open();

            try {
                return ListOf<T>(connection, sql, parameters, commandType);
            } finally {
                connection.Close();
            }
        }
    }

    /// <summary>
    /// Returns the result list of the specified SQL.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection">The connection.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns></returns>
    public List<T> ListOf<T>(OleDbConnection connection, string sql, dynamic parameters = null, CommandType? commandType = null) {
        return connection.Query<T>(sql, new DynamicParameters(parameters), commandTimeout: 0, commandType: commandType).ToList();
    }

    /// <summary>
    /// Connection Factory class that provides a simple connection abstraction
    /// Inherit this class to provide a connection factory that dispenses connections in a manner that suits 
    /// your security and environment requirements
    /// </summary>
    public class ConnectionFactory
    {
        protected string connectionString = "";
        protected string provider = "System.Data.OleDb";
        protected DbProviderFactory providerFactory;
        protected Dictionary<string, string> parameterFormats = new Dictionary<string, string>(System.StringComparer.CurrentCultureIgnoreCase)
        {
            { "System.Data.SqlClient", "@{0}" },
            { "System.Data.OracleClient", ":{0}" },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionFactory"/> class.
        /// </summary>
        protected ConnectionFactory() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionFactory"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public ConnectionFactory(string connectionString, string provider = "System.Data.OleDb") {
            this.connectionString = connectionString;
            this.provider = provider;
            this.providerFactory = DbProviderFactories.GetFactory(this.provider);
        }

        public DbProviderFactory ProviderFactory {
            get { return this.providerFactory; }
        }

        public string ParameterFormat {
            get {
                if (this.parameterFormats.ContainsKey(this.provider))
                    return this.parameterFormats[this.provider];

                return "{0}";
            }
        }

        /// <summary>
        /// Creates a connection.
        /// </summary>
        /// <returns></returns>
        public virtual OleDbConnection CreateConnection() {
            return new System.Data.OleDb.OleDbConnection(this.connectionString);
        }
    }

    internal abstract class Mapper
    {
        public abstract void Initialize();
    }

}