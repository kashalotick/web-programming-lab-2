## Run separate
### Run DB
```shell
docker compose -f compose.db.yaml up -d
```

### Run App
Mannualy or by
```shell
docker compose -f compose.app.yaml up --build
```

## Run All
```shell
docker compose -f compose.db.yaml -f compose.app.yaml up --build
```