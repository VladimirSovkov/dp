@echo off

reg delete "HKCU\Environment" /f /v DB_RUS 
reg delete "HKCU\Environment" /f /v DB_EU 
reg delete "HKCU\Environment" /f /v DB_OTHER 

taskkill /f /im redis-server.exe