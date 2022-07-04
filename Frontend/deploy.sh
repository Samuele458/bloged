#!/bin/bash

#deploying build folders to wwwroot-folder volume
rm -r -f /app/wwwroot/*


#copying new files
mkdir -p /app/wwwroot/bloged
cp -r /app/public/* /app/wwwroot/
