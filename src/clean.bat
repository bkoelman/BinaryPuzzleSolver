@echo off
for /R %%x in (bin) do if exist "%%x" rmdir /s /q "%%x" 1>NUL 2>NUL
for /R %%x in (obj) do if exist "%%x" rmdir /s /q "%%x" 1>NUL 2>NUL

rmdir /s /q .vs 1>NUL 2>NUL

del /f /s /q *.user 1>NUL 2>NUL