# `postgres-uri-to-npgsql-connection-string`

> Converts a PostgreSQL URI into an Npgsql-compatible connection string.

## Purpose

Cloud platforms such as Neon, Supabase, and Heroku provision PostgreSQL
databases with URI-format connection strings
(`postgres://user:pass@host:port/db`). .NET applications that use the Npgsql
driver require a semicolon-delimited connection string
(`Host=…;Port=…;Database=…;Username=…;Password=…`). This action bridges the
two formats so workflows can provision a database in one step and hand a
ready-to-use connection string to the application in the next.

## Inputs

| Name | Required | Default | Description |
|------|----------|---------|-------------|
| `connection-uri` | Yes | — | Full PostgreSQL URI to convert (e.g., `postgres://user:pass@host:5432/mydb`). |
| `extra-params` | No | `""` | Additional Npgsql parameters to append (e.g., `SSL Mode=Require;Trust Server Certificate=true`). |

## Outputs

| Name | Description |
|------|-------------|
| `connection-string` | The converted Npgsql-format connection string, ready to inject into app configuration. |

## Usage

```yaml
- uses: ./.github/actions/postgres-uri-to-npgsql-connection-string
  id: pg
  with:
    connection-uri: ${{ steps.neon.outputs.db_url }}
    extra-params: "SSL Mode=Require;Trust Server Certificate=true"

- name: Run migrations
  run: dotnet ef database update
  env:
    ConnectionStrings__Default: ${{ steps.pg.outputs.connection-string }}
```

## Behavior

1. Parses the incoming URI into its component parts: scheme, username,
   password, host, port, and database name.
2. URL-decodes the username and password to handle special characters.
3. Assembles the parts into the Npgsql key-value format:
   `Host={host};Port={port};Database={db};Username={user};Password={pass}`.
4. Appends any `extra-params` value to the end of the string.
5. Writes the final connection string to the `connection-string` output.

## Error Handling

- **Missing `connection-uri`** — the action fails immediately with
  `Error: connection-uri is required`.
- **Malformed URI** — if the URI cannot be parsed (missing scheme, host, or
  database), the action fails with a descriptive parse error indicating which
  component is missing.
- **Empty password** — permitted; the resulting connection string omits the
  `Password` key rather than setting it to an empty value.

## Dependencies

- `bash` — the action runs as a composite action using a Bash shell step.
  No additional runtimes or packages are required.

## Workflow Integration

- **`pr-preview.yml`** — used during the **database provisioning** stage to
  convert the Neon branch URI into a connection string consumed by the .NET
  application container.

## Maintainer Notes

- The parser assumes the `postgres://` or `postgresql://` scheme. Other
  schemes (e.g., `jdbc:postgresql://`) are not supported.
- Secrets should be masked in logs automatically by GitHub Actions when the
  input is sourced from a secret or a prior step output marked as a secret.
- See the [Action README Template](../../../docs/guides/developer/action-readme-template.md)
  for the canonical documentation format.
