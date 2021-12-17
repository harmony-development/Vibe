# Vibe

A **yet-to-be-implemented** distributed server for Harmony

## High-level architecture

Vibe is composed of multiple services. The main components are a [streams handler](#streams-handler), a [requests handler](#requests-handler) and a REST handler.

- TODO: how is Vibe structured in relationship to Harmony protocol?

By default, these work together in the same process using a local message queue, but they can be split out into multiple processes using a message queue such as [Redis pub/sub](https://redis.io/topics/pubsub). It is possible to host *x* instances of the streams handler and *y* instances of the requests handler, depending on your needs.

Redis (or in-memory caching) is also used for transient information such as statuses.

Vibe does not handle HTTPS or load balancing. It must be put behind a reverse proxy such as HAProxy or nginx. Example configuration files, for both single-process and larger installations, will be provided in the repo.

## Components

TODO: codegen service interfaces from protocol spec

### Requests Handler

This component handles unary hRPC requests and talks to the database, optionally adding to the message queue if the action creates a resource.

TODO: rate limiting?
-> per remote IP address
-> per user account
-> per request per resource
- put this in IVibeState (to store it in redis for multi-process deployments)

### Streams Handler

This component handles dispatching events from the message queue to users' currently connected streams.

TODO: send messages only to users who should recieve them

## Future Work / Ideas

- It should be possible to migrate from a [scherzo](https://github.com/harmony-development/scherzo) installation to a Vibe installation.

## File tree maybe?

/
|---- Components
    |-- Requests
    |-- Streams
    |-- REST
|---- State
    |-- Database
    |-- Cache
        |-- InMemory
        |-- Redis
    |-- MessageQueue
        |-- InMemory
        |-- Redis
|---- Util
    |-- AspNetWrapper
|---- Core
    |-- Config
    |-- 
