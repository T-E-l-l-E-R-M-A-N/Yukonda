using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using Dapper;
using Microsoft.Data.Sqlite;
using Npgsql;
using Yukonda.Core.ViewModels.Windows;

namespace Yukonda.Core.Models
{
    public class ConnectionModel : BindableBase
    {
        private MainWindowViewModel _mainWindowViewModel = null!;

        public IDbConnection DbConnection { get; set; } = null!;
        public bool Connected { get; set; } = false;
        public ObservableCollection<SchemaModel> Schemas { get; set; } = new();
        public DbProviderModel DbProvider { get; set; } = null!;

        private void LoadPublicTables()
        {
            if (DbConnection == null) return;

            Schemas.Clear();
            IEnumerable<dynamic> schemas = null!;
            switch (DbProvider.Name)
            {
                case "PostgreSQL":
                    schemas = DbConnection.Query<dynamic>("SELECT n.nspname AS schema_name,\n       CASE WHEN has_schema_privilege(n.nspname, 'CREATE') THEN 'YES' ELSE 'NO' END AS can_create,\n       CASE WHEN has_schema_privilege(n.nspname, 'USAGE') THEN 'YES' ELSE 'NO' END AS can_use\nFROM pg_namespace n\nWHERE n.nspname NOT LIKE 'pg_%' AND n.nspname != 'information_schema';");
                    break;
            }
            foreach (var schema in schemas)
            {
                var name = schema.schema_name;
                var canUse = schema.can_use == "YES" ? true : false;
                var canCreate = schema.can_create == "YES" ? true : false;
                var isReadonly = !(canCreate & canUse);
                var tables = DbConnection.Query<string>(
                    $"SELECT table_name\nFROM information_schema.tables\nWHERE table_schema = '{name}'\n  AND table_type = 'BASE TABLE';");
                var schemaModel = new SchemaModel()
                {
                    SchemaName = name,
                    IsReadOnly = isReadonly,
                    Tables = new ObservableCollection<string>(tables)
                };
                Schemas.Add(schemaModel);
            }
        }

        public void CloseConnection()
        {
            DbConnection?.Close();
        }

        public void Init(IDbConnection connection)
        {
            _mainWindowViewModel = IoC.Resolve<MainWindowViewModel>();
            try
            {
                if (DbConnection != null) DbConnection.Close();

                DbConnection = connection;

                Connected = true;

                LoadPublicTables();


            }
            catch (Exception e)
            {
                _mainWindowViewModel.RaiseError(e);
            }
        }
    }
}