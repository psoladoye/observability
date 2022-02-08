# observability
A guide to instrumenting monitoring of applications

## Docker Compose

### Build Application Images
```shell
docker compose -f ops/docker/docker-compose.yml build
```

### Run Application and Monitoring Components
```shell
docker compose -f ops/docker/docker-compose.yml up -d
```