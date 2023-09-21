Oak
===

A project management app where tasks are organised in a tree structure.
This gives the benefit of enabling the aggregation of task stats like time
and cost estimates up the tree structure such that each tasks is a summary of all
the sub tasks beneath it.

### Prerequisites

To build and run this project you need `.net core 7`, `docker` and `docker-compose` installed.

To build and run unit tests:
```bash
./bin/pre
```
To build and run the app:
```bash
./bin/run
```
You can pass parameter `nuke` to either `./bin/pre` or `./bin/run` to delete
docker containers and rebuild them, this is typically useful if there has been a db schema change.