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