# Odin Interview Solution

## Overview

This solution consists of 3 microservices that communicate over HTTP:

IpLookupService – public-facing service that returns IP details.
IpCacheService – in-memory cache with 1-minute TTL for IP details.
BatchProcessingService - 

Each service is independently deployable, has its own API surface, and is expected to run in its own container/process.

## Projects

SharedKernel — lightweight library containing shared models and utility components (e.g. DTOs, validators) used across microservices.
endpoins, configuration


## Deployment

`docker build -t cache-service:local -f CacheService/Dockerfile .`
`docker run --rm -p 5001:8080 --name cache-service cache-service:local`

`docker build -t ip-lookup-service:local -f IpLookupService/Dockerfile .`
`docker run --rm -p 5002:8080 --name ip-lookup-service ip-lookup-service:local`

`docker build -t batch-service:local -f BatchService/Dockerfile .`
`docker run --rm -p 5003:8080 --name batch-service batch-service:local`

`docker compose up --build`