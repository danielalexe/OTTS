using DataLink;
using DataObjects;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTTS_WPF.Helpers
{
    public static class PersistentData
    {
        public static DTOUser LoggedUser
        {
            get;set;
        }

        public static EnumDatabaseType DatabaseType
        {
            get;set;
        }

        public static EnumAuthenticationType AuthenticationType
        {
            get; set;
        }

        public static string DatabaseServer
        {
            get;
            set;
        }

        public static string DatabaseName
        {
            get;
            set;
        }
        public static string UserName_SQL
        {
            get;
            set;
        }
        public static string Password_SQL
        {
            get;
            set;
        }

        public static string ConnectionString
        {
            get;
            set;
        }

        public static string GetConnectionString_Offline()
        {
            // Specify the provider name, server and database.
            string providerName = "System.Data.SqlClient";
            string serverName = "(LocalDB)\\MSSQLLocalDB";

            // Initialize the connection string builder for the
            // underlying provider.
            SqlConnectionStringBuilder sqlBuilder =
                new SqlConnectionStringBuilder();

            // Set the properties for the data source.
            sqlBuilder.DataSource = serverName;
            sqlBuilder.IntegratedSecurity = true;
            sqlBuilder.AttachDBFilename = "|DataDirectory|\\Offline\\OTTS.mdf";
            sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.ApplicationName = "EntityFramework";

            // Build the SqlConnection connection string.
            string providerString = sqlBuilder.ToString();

            // Initialize the EntityConnectionStringBuilder.
            EntityConnectionStringBuilder entityBuilder =
                new EntityConnectionStringBuilder();

            //Set the provider name.
            entityBuilder.Provider = providerName;

            // Set the provider-specific connection string.
            entityBuilder.ProviderConnectionString = providerString;

            // Set the Metadata location.
            entityBuilder.Metadata = "res://*/DBModel.csdl|res://*/DBModel.ssdl|res://*/DBModel.msl";

            return entityBuilder.ToString();
        }

        public static string GetConnectionString_MSSQL()
        {
            if (AuthenticationType==EnumAuthenticationType.SQLAuth)
            {
                // Specify the provider name, server and database.
                string providerName = "System.Data.SqlClient";
                string serverName = DatabaseServer;
                string databaseName = DatabaseName;

                // Initialize the connection string builder for the
                // underlying provider.
                SqlConnectionStringBuilder sqlBuilder =
                    new SqlConnectionStringBuilder();

                // Set the properties for the data source.
                sqlBuilder.DataSource = serverName;
                sqlBuilder.InitialCatalog = databaseName;
                sqlBuilder.IntegratedSecurity = false;
                sqlBuilder.UserID = UserName_SQL;
                sqlBuilder.Password = Password_SQL;

                // Build the SqlConnection connection string.
                string providerString = sqlBuilder.ToString();

                // Initialize the EntityConnectionStringBuilder.
                EntityConnectionStringBuilder entityBuilder =
                    new EntityConnectionStringBuilder();

                //Set the provider name.
                entityBuilder.Provider = providerName;

                // Set the provider-specific connection string.
                entityBuilder.ProviderConnectionString = providerString;

                // Set the Metadata location.
                entityBuilder.Metadata = "res://*/DBModel.csdl|res://*/DBModel.ssdl|res://*/DBModel.msl";

                return entityBuilder.ToString();
            }
            else
            {
                // Specify the provider name, server and database.
                string providerName = "System.Data.SqlClient";
                string serverName = DatabaseServer;
                string databaseName = DatabaseName;

                // Initialize the connection string builder for the
                // underlying provider.
                SqlConnectionStringBuilder sqlBuilder =
                    new SqlConnectionStringBuilder();

                // Set the properties for the data source.
                sqlBuilder.DataSource = serverName;
                sqlBuilder.InitialCatalog = databaseName;
                sqlBuilder.IntegratedSecurity = true;

                // Build the SqlConnection connection string.
                string providerString = sqlBuilder.ToString();

                // Initialize the EntityConnectionStringBuilder.
                EntityConnectionStringBuilder entityBuilder =
                    new EntityConnectionStringBuilder();

                //Set the provider name.
                entityBuilder.Provider = providerName;

                // Set the provider-specific connection string.
                entityBuilder.ProviderConnectionString = providerString;

                // Set the Metadata location.
                entityBuilder.Metadata = "res://*/DBModel.csdl|res://*/DBModel.ssdl|res://*/DBModel.msl";

                return entityBuilder.ToString();
            }
        }
        public static string GetConnectionString_Oracle()
        {
            return "";
        }
        public static string GetConnectionString_MySQL()
        {
            return "";
        }
        public static string GetConnectionString_PostgreSQL()
        {
            return "";
        }
    }
}
