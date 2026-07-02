#!/usr/bin/env bash
set -euo pipefail

usage() {
  cat <<'USAGE'
Usage:
  scripts/restore-sqlserver-backup.sh <backup.bak> [database-name]

Restores a SQL Server .bak into the local Docker SQL Server container as a separate database.
Defaults:
  database-name: ClubSite_RealCopy
  container:     sqlserver2022
  db user:       sa

Required environment:
  SQLCMDPASSWORD or SA_PASSWORD must contain the SQL Server password.

Optional environment:
  SQLSERVER_CONTAINER  Docker container name, default sqlserver2022
  SQLUSER              SQL login, default sa
USAGE
}

if [[ ${1:-} == "-h" || ${1:-} == "--help" || $# -lt 1 ]]; then
  usage
  exit $([[ $# -lt 1 ]] && echo 1 || echo 0)
fi

backup_path=$1
database_name=${2:-ClubSite_RealCopy}
if [[ ! "$database_name" =~ ^[A-Za-z_][A-Za-z0-9_]*$ ]]; then
  echo "ERROR: Database name must contain only letters, numbers, and underscores, and must not start with a number." >&2
  exit 1
fi
container=${SQLSERVER_CONTAINER:-sqlserver2022}
sql_user=${SQLUSER:-sa}
sql_password=${SQLCMDPASSWORD:-${SA_PASSWORD:-}}

if [[ -z "$sql_password" ]]; then
  echo "ERROR: Set SQLCMDPASSWORD or SA_PASSWORD before running this script." >&2
  exit 1
fi

if [[ ! -f "$backup_path" ]]; then
  echo "ERROR: Backup file not found: $backup_path" >&2
  exit 1
fi

if ! docker inspect "$container" >/dev/null 2>&1; then
  echo "ERROR: SQL Server container not found: $container" >&2
  exit 1
fi

sqlcmd=(docker exec -e SQLCMDPASSWORD="$sql_password" "$container" /opt/mssql-tools18/bin/sqlcmd -S localhost -U "$sql_user" -C -b)
container_backup="/var/opt/mssql/backup/$(basename "$backup_path")"

printf 'Copying %s into %s:%s\n' "$backup_path" "$container" "$container_backup"
docker exec "$container" mkdir -p /var/opt/mssql/backup
docker cp "$backup_path" "$container:$container_backup"

if "${sqlcmd[@]}" -W -h -1 -Q "SET NOCOUNT ON; SELECT name FROM sys.databases WHERE name = N'$database_name';" | grep -Fxq "$database_name"; then
  echo "ERROR: Database already exists, refusing to overwrite: $database_name" >&2
  exit 1
fi

filelist=$("${sqlcmd[@]}" -W -s '|' -Q "RESTORE FILELISTONLY FROM DISK = N'$container_backup';")
logical_data=$(printf '%s\n' "$filelist" | awk -F'|' '$3 == "D" {gsub(/^ +| +$/, "", $1); print $1; exit}')
logical_log=$(printf '%s\n' "$filelist" | awk -F'|' '$3 == "L" {gsub(/^ +| +$/, "", $1); print $1; exit}')

if [[ -z "$logical_data" || -z "$logical_log" ]]; then
  echo "ERROR: Could not determine logical data/log names from backup." >&2
  printf '%s\n' "$filelist" >&2
  exit 1
fi

cat > /tmp/clubsite_restore.sql <<SQL
RESTORE DATABASE [$database_name]
FROM DISK = N'$container_backup'
WITH
  MOVE N'$logical_data' TO N'/var/opt/mssql/data/${database_name}.mdf',
  MOVE N'$logical_log' TO N'/var/opt/mssql/data/${database_name}_log.ldf',
  RECOVERY,
  STATS = 10;
SQL

docker cp /tmp/clubsite_restore.sql "$container:/tmp/clubsite_restore.sql"
"${sqlcmd[@]}" -i /tmp/clubsite_restore.sql
"${sqlcmd[@]}" -W -Q "SET NOCOUNT ON; SELECT name, state_desc FROM sys.databases WHERE name = N'$database_name';"

echo "Restored database: $database_name"
