# observability
A guide to instrumenting monitoring of applications

## Docker Compose

### Build Application Images
```shell
docker compose -f ops/docker/docker-compose.yml build
```

### GCP
```shell
export GOOGLE_APPLICATION_CREDENTIALS=${HOME}/tmp/observability-dev.json
```

### Run Application and Monitoring Components
```shell
docker compose -f ops/docker/docker-compose.yml up -d
```