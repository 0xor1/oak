Oak 
===
[![Build Status](https://github.com/0xor1/oak/actions/workflows/build.yml/badge.svg)](https://github.com/0xor1/oak/actions/workflows/build.yml)
[![Coverage Status](https://coveralls.io/repos/github/0xor1/oak/badge.svg)](https://coveralls.io/github/0xor1/oak)
[![Demo Live](https://img.shields.io/badge/demo-live-4ec820)](https://oak.dans-demos.com)


A project management app where tasks are organised in a tree structure.
This gives the benefit of enabling the aggregation of task stats like time
and cost estimates up the tree structure such that each tasks is a summary of all
the sub tasks beneath it.

### Prerequisites

To build and run this project you need `.net core 8`, `docker` and `docker-compose` installed.

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
