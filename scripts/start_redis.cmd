@echo off

setx DB_RUS "localhost:6000"
setx DB_EU "localhost:6001"
setx DB_OTHER "localhost:6002"

start "RedisServer" redis-server
start "Rus" redis-server --port 6000
start "Eu" redis-server --port 6001
start "Other" redis-server --port 6002