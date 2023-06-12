# observability
A guide to instrumenting monitoring of applications

## Conventions


## Logger

### Web Log Structure


### Serilog

## Docker Compose

### Build and Push to Container Registry
```shell
docker build -t gcr.io/${GCP_PROJECT}/observability:${IMAGE_VERSION} .

docker push gcr.io/${GCP_PROJECT}/observability:${IMAGE_VERSION}
```

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

docker compose up -d --force-recreate --build web
```

### Open Telemetry Collector
[zpages](https://github.com/open-telemetry/opentelemetry-collector/tree/main/extension/zpagesextension) <br>
[troubleshooting](https://github.com/open-telemetry/opentelemetry-collector/blob/main/docs/troubleshooting.md)
[serilog](https://github.com/serilog/serilog-sinks-opentelemetry)
[best practices](https://github.com/open-telemetry/opentelemetry-collector/blob/main/docs/security-best-practices.md)
[Config - dotnet](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/main/docs/config.md)
[Semantic Conventions](https://github.com/open-telemetry/semantic-conventions)
[Log Exporter](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/protocol/exporter.md)
[Dotnet Examples](https://github.com/open-telemetry/opentelemetry-dotnet/tree/main/examples)

### Grafana Tempo
[GCS](https://grafana.com/docs/tempo/latest/configuration/gcs/)

