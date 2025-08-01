FROM mariadb:11.8.2 AS builder

# That file does the DB initialization but also runs mysql daemon, by removing the last line it will only init
RUN ["sed", "-i", "s/exec \"$@\"/echo \"not running $@\"/", "/usr/local/bin/docker-entrypoint.sh"]

ENV MYSQL_ROOT_PASSWORD=root

# generic sqls for user pkg tests
COPY sql/oak.sql /docker-entrypoint-initdb.d/oak.sql

RUN ["/usr/local/bin/docker-entrypoint.sh", "mariadbd", "--datadir", "/initialized-db", "--aria-log-dir-path", "/initialized-db"]

FROM mariadb:11.8.2

COPY --from=builder /initialized-db /var/lib/mysql