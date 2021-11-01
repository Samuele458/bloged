#!/bin/bash

#deploying build folders to wwwroot-folder volume
rm -r -f /app/wwwroot/*

echo app:
ls -la /app

echo wwwroot:
ls /app/wwwroot



#copying new files
mkdir -p /app/wwwroot/dashboard
cp -r /app/packages/blog/build/* /app/wwwroot/
cp -r /app/packages/dashboard/build/* /app/wwwroot/dashboard/

echo wwwroot:
ls /app/wwwroot