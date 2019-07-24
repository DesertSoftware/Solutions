//
//  Copyright 2013, Desert Software Solutions Inc.
//      https://github.com/DesertSoftware/Solutions
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
using System.Data.SqlClient;
using System.Linq;
using Dapper;

/// <summary>
/// Generic methods for accessing a database using dapper.net
/// </summary>
public class Database
{
    private ConnectionFactory connectionFactory;
    private int? connectionTimeout = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="Database" /> class.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="provider">The provider.</param>
    /// <param name="connectionTimeout">The connection timeout.</param>
    /// <param name="mapper">The mapper.</param>
    public Database(string connectionString, string provider = "System.Data.SqlClient", int? connectionTimeout = null, Mapper mapper = null) {
        this.connectionFactory = new ConnectionFactory(connectionString, provider);
        this.connectionTimeout = connectionTimeout;
        if (mapper != null)
            mapper.Initialize();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Database" /> class.
    /// </summary>
    /// <param name="connectionFactory">The <see cref="ConnectionFactory" /> instance.</param>
    /// <param name="connectionTimeout">The connection timeout.</param>
    /// <param name="mapper">The mapper.</param>
    public Database(ConnectionFactory connectionFactory, int? connectionTimeout = null, Mapper mapper = null) {
        this.connectionFactory = connectionFactory;
        this.connectionTimeout = connectionTimeout;
        if (mapper != null)
            mapper.Initialize();
    }

    /// <summary>
    /// Creates the connection.
    /// </summary>
    /// <returns></returns>
    public IDbConnection CreateConnection() {
        return this.connectionFactory.CreateConnection();
    }

    /// <summary>
    /// Executes the specified SQL.
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="connectionTimeout">The connection timeout.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns></returns>
    public int Execute(string sql, dynamic parameters = null, int? connectionTimeout = null, CommandType? commandType = null) {
        using (IDbConnection connection = CreateConnection()) {
            connection.Open();

            try {
                return Execute(connection, sql, parameters, connectionTimeout, commandType);
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
    /// <param name="connectionTimeout">The connection timeout.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns></returns>
    public int Execute(IDbConnection connection, string sql, dynamic parameters = null, int? connectionTimeout = null, CommandType? commandType = null) {
        return connection.Execute(sql, new DynamicParameters(parameters), commandTimeout: connectionTimeout ?? this.connectionTimeout, commandType: commandType);
    }

    /// <summary>
    /// Returns the first result for the specified SQL.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sql">The SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="connectionTimeout">The connection timeout.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns></returns>
    public T First<T>(string sql, dynamic parameters = null, int? connectionTimeout = null, CommandType? commandType = null) {
        using (IDbConnection connection = CreateConnection()) {
            connection.Open();

            try {
                return First<T>(connection, sql, parameters, connectionTimeout, commandType);
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
    /// <param name="connectionTimeout">The connection timeout.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns></returns>
    public T First<T>(IDbConnection connection, string sql, dynamic parameters = null, int? connectionTimeout = null, CommandType? commandType = null) {
        return connection.Query<T>(sql, new DynamicParameters(parameters), commandTimeout: connectionTimeout ?? this.connectionTimeout, commandType: commandType).FirstOrDefault();
    }

    /// <summary>
    /// Returns the result list of the specified SQL.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sql">The SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="connectionTimeout">The connection timeout.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns></returns>
    public List<T> ListOf<T>(string sql, dynamic parameters = null, int? connectionTimeout = null, CommandType? commandType = null) {
        using (IDbConnection connection = CreateConnection()) {
            connection.Open();

            try {
                return ListOf<T>(connection, sql, parameters, connectionTimeout, commandType);
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
    /// <param name="connectionTimeout">The connection timeout.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns></returns>
    public List<T> ListOf<T>(IDbConnection connection, string sql, dynamic parameters = null, int? connectionTimeout = null, CommandType? commandType = null) {
        return connection.Query<T>(sql, new DynamicParameters(parameters), commandTimeout: connectionTimeout ?? this.connectionTimeout, commandType: commandType).ToList();
    }

    private DbCommand CreateCommand(string commandText, IDbConnection connection) {
        DbCommand command = this.connectionFactory.ProviderFactory.CreateCommand(); // DbProviderFactories.GetFactory((DbConnection)connection).CreateCommand();

        command.CommandText = commandText;
        command.Connection = (DbConnection)connection;
        return command;
    }

    private DbParameter AddParameterWithValue(DbCommand command, string parameterName, object value) {
        DbParameter parameter = command.CreateParameter();

        parameter.ParameterName = string.Format(this.connectionFactory.ParameterFormat, parameterName);
        parameter.Value = value;
        command.Parameters.Add(parameter);

        return parameter;
    }

    private DbDataAdapter CreateAdapter(DbCommand selectCommand) {
        //        DbDataAdapter adapter = DbProviderFactories.GetFactory(selectCommand.Connection).CreateDataAdapter();
        DbDataAdapter adapter = this.connectionFactory.ProviderFactory.CreateDataAdapter();

        adapter.SelectCommand = selectCommand;
        return adapter;
    }

    /// <summary>
    /// Returns the result Table of the specified SQL.
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns></returns>
    public DataTable Table(string sql, dynamic parameters = null, CommandType? commandType = null) {
        DataTable table = new DataTable();

        using (DbCommand command = CreateCommand(sql, CreateConnection())) { //new SqlCommand(sql, CreateConnection())) {
            if (commandType != null)
                command.CommandType = commandType.Value;

            IDictionary<string, string> parameterDictionary = parameters as IDictionary<string, string>;

            if (parameterDictionary != null)
                foreach (var parameter in parameterDictionary.Keys)
                    AddParameterWithValue(command, parameter, parameterDictionary[parameter]);
            //                    command.AddParameterWithValue("@" + parameter, parameterDictionary[parameter]);
            else
                foreach (var property in parameters.GetType().GetProperties())
                    AddParameterWithValue(command, property.Name, property.GetGetMethod().Invoke(parameters, null));
            //                    command.Parameters.AddWithValue("@" + property.Name, property.GetGetMethod().Invoke(parameters, null));

            using (DbDataAdapter adapter = CreateAdapter(command))
                adapter.Fill(table);

            return table;
        }
    }

    /// <summary>
    /// Returns a result Reader for the specified SQL.
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns></returns>
    /// <remarks>Be sure to call Close or wrap in a using statement to release reader connection resources</remarks>
    public IDataReader Reader(string sql, dynamic parameters = null, CommandType? commandType = null) {
        IDbConnection connection = CreateConnection();

        connection.Open();

        try {
            using (DbCommand command = CreateCommand(sql, connection)) {
                if (commandType != null)
                    command.CommandType = commandType.Value;

                IDictionary<string, string> parameterDictionary = parameters as IDictionary<string, string>;

                if (parameterDictionary != null)
                    foreach (var parameter in parameterDictionary.Keys)
                        AddParameterWithValue(command, parameter, parameterDictionary[parameter]);
                else
                    foreach (var property in parameters.GetType().GetProperties())
                        AddParameterWithValue(command, property.Name, property.GetGetMethod().Invoke(parameters, null));

                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
        } catch {
            connection.Close();
            throw;
        }
    }

    /// <summary>
    /// Connection Factory class that provides a simple connection abstraction
    /// Inherit this class to provide a connection factory that dispenses connections in a manner that suits 
    /// your security and environment requirements
    /// </summary>
    public class ConnectionFactory
    {
        protected string connectionString = "";
        protected string provider = "System.Data.SqlClient";
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
        public ConnectionFactory(string connectionString, string provider = "System.Data.SqlClient") {
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
        public virtual IDbConnection CreateConnection() {
            DbConnection connection = this.providerFactory.CreateConnection();

            connection.ConnectionString = this.connectionString;
            return connection;
        }
    }

    public abstract class Mapper
    {
        public abstract void Initialize();
    }

}
