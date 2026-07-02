# Local testing with a production copy

This project can be tested locally against a copy of the live SQL Server database without committing the database or secrets.

## What is intentionally not stored in git

- SQL Server backups (`*.bak`)
- SQL Server data/log files (`*.mdf`, `*.ldf`)
- Connection-string secrets
- `node_modules/`
- Generated `ClubSite/wwwroot/` output

## Prerequisites

- Docker SQL Server container, for example `sqlserver2022`
- A local `.bak` file, for example `~/backup/volleyballclub.bak`
- SQL Server password available as `SQLCMDPASSWORD` or `SA_PASSWORD`
- Node.js/npm for the frontend asset build

## 1. Restore the database copy

Restore the backup into a separate database so the normal local database is not overwritten:

```bash
export SQLCMDPASSWORD='<local sql password>'
scripts/restore-sqlserver-backup.sh ~/backup/volleyballclub.bak ClubSite_RealCopy
```

The script refuses to overwrite an existing database.

## 2. Point local development config at the copy

In the local secrets/config file used by development, set the ClubSite connection string database to:

```text
Database=ClubSite_RealCopy
```

For this workspace that file is outside git at:

```text
../Secrets/credentials.Development.json
```

Do not commit that file.

## 3. Build frontend assets

`ClubSite/wwwroot/assets` is generated from source assets and package dependencies:

```bash
cd ClubSite
npm ci
npx gulp
cd ..
```

This creates CSS, fonts, Font Awesome webfonts, and Bootstrap JavaScript under `ClubSite/wwwroot/assets`.

## 4. Mirror public uploads needed for pages under test

The SQL backup contains media metadata, but not the files stored under `wwwroot/uploads`.
Mirror the public upload files referenced by the pages you want to test:

```bash
scripts/mirror-public-uploads.sh https://www.volleyballclub.de/
```

You can pass more page URLs:

```bash
scripts/mirror-public-uploads.sh \
  https://www.volleyballclub.de/ \
  https://www.volleyballclub.de/turniere
```

This copies only public `/uploads/...` files into `ClubSite/wwwroot/uploads`.

## 5. Verify local rendering resources

When the app is running locally, key homepage resources should return `200`:

```bash
curl -kIL https://127.0.0.1:5001/assets/css/style.min.css -H 'Host: www.volleyballclub.de:5001'
curl -kIL https://127.0.0.1:5001/assets/js/bootstrap/bootstrap.bundle.min.js -H 'Host: www.volleyballclub.de:5001'
```

If the page is opened through an SSH tunnel, use the hostname mapping/tunnel described in `TestRemote.md` or equivalent local notes.
