# Odin Interview Solution

## Overview

This solution demonstrates a **microservice-based architecture** built with **ASP.NET Core (.NET 8)**.  
It consists of three isolated microservices communicating through REST endpoints, containerized with Docker, and orchestrated via `docker-compose`.


| Service | Description |
|----------|-------------|
| **IP Lookup Service** | Retrieves IP details from an external provider (IPStack) and caches results to reduce redundant API calls. |
| **IP Cache Service** | Provides in-memory caching for IP details with automatic expiration. |
| **Batch Processing Service** | Handles asynchronous batch processing of multiple IPs, querying and caching their details. |

All services are containerized with Docker and orchestrated via `docker-compose`.


## Communication flow


`BatchService → IpLookupService → IpCacheService → External IPStack API`


1. **User** sends a batch of IP addresses to the `BatchService` via `POST /batch`.
2. **BatchService** creates a batch job, limits concurrent batch submissions (rate limiting), and processes IPs in chunks of 10 asynchronously.
3. For each IP address, **BatchService** calls **IpLookupService** to get details.
4. **IpLookupService** first queries **IpCacheService**:
   - If cached entry is found → immediately returns it.
   - If not → requests fresh data from **IPStack API**, stores it back in the cache, and returns the result.
5. **IpCacheService** maintains IP details in an in-memory cache (`MemoryCache`) with a TTL (Time To Live) of **1 minute**.
6. **BatchService** periodically updates the batch status and exposes it via `GET /batch/{batchId}` until completed.


## Service Interaction Details

### 1. IP Lookup Service

**Responsibilities:**
- Exposes `GET /ip/{ipAddress}`.
- Validates IP format.
- Checks cache first.
- Queries external provider (IPStack) if data is missing.
- Handles network and provider errors gracefully. For this purpose we use exception handling middleware.
- Applies retry policy (request is retried 3 times with exponential backoff in case of network connectivity problems).

### 2. IP Cache Service

**Responsibilities:**
- Provides REST endpoints for storing and retrieving cached IP details:
    - `GET /{ip}` — returns cached IP data if present.
    - `PUT /{ip}` — stores/updates cached IP data.
- Uses MemoryCache for storing objects with 1-minute TTL.

### 3. Batch Processing Service

**Responsibilities:**

- Accepts batches of IPs via `POST /batch`.
- Assigns a unique Guid for tracking.
- Splits the list into chunks of 10 IPs and processes them asynchronously.
- Uses IPLookupService to fetch details and automatically caches results.
- Exposes status via `GET /batch/{batchId}`.
- Restricts batch submission frequency to 5 per 10 seconds.

All services implement a unified global exception middleware that transforms internal errors into standardized HTTP responses.


## Setup & Run

### 1. Requirements
- Docker & Docker Compose installed.
- Internet connection (for IPStack API).

### 2. Clone repository
```bash
git clone https://github.com/ArtiomRegino/odin-interview-solution.git
cd odin-interview-solution
```

### 3. Configuration
All configuration values can be set via appsettings.json or environment variables (Docker uses env vars).

| Service             | Key                 | Description                   | Example (Docker env)            |
| ------------------- | ------------------- | ----------------------------- | ------------------------------- |
| **IpLookupService** | `IpStack__BaseUrl`  | External IP provider base URL | `https://api.ipstack.com`       |
|                     | `IpStack__ApiKey`   | IPStack API key               | `your_api_key`                  |
|                     | `Cache__BaseUrl`    | URL of cache service          | `http://ip-cache-service:8080`  |
| **BatchService**    | `IPLookup__BaseUrl` | URL of lookup service         | `http://ip-lookup-service:8080` |
| **CacheService**    | TTL                 | Cache expiration (1 minute)   | defined in code                 |
Pay attention to `IpStack__ApiKey` setting. Yoy can find this key on https://ipstack.com/ in the personal account.

### 4. Build and run all services
`docker compose up --build`

### 5. Access the services
| Service               | URL                                                            | Description                       |
| --------------------- | -------------------------------------------------------------- | --------------------------------- |
| **IP Cache Service**  | [http://localhost:5001/swagger](http://localhost:5001/swagger) | Stores & retrieves cached IP data |
| **IP Lookup Service** | [http://localhost:5002/swagger](http://localhost:5002/swagger) | Fetches IP info (via IPStack)     |
| **Batch Service**     | [http://localhost:5003/swagger](http://localhost:5003/swagger) | Processes batch IP requests       |


## Separate run

To run **CacheService** separatly:

`docker build -t cache-service:local -f CacheService/Dockerfile .`

`docker run --rm -p 5001:8080 --name cache-service cache-service:local`

To run **IpLookupService** separatly:

`docker build -t ip-lookup-service:local -f IpLookupService/Dockerfile .`

`docker run --rm -p 5002:8080 --name ip-lookup-service ip-lookup-service:local`

To run **BatchService** separatly:

`docker build -t batch-service:local -f BatchService/Dockerfile .`

`docker run --rm -p 5003:8080 --name batch-service batch-service:local`