#!/bin/bash

#deploying build folders to wwwroot-folder volume
rm -r /app/wwwroot/*

#copying new files
mkdir /app/wwwroot/dashboard
cp -r /app/packages/blog/build/* /app/wwwroot/
cp -r /app/packages/dashboard/build/* /app/wwwroot/dashboard/