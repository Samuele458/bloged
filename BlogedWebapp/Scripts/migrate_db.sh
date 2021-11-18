#!/bin/bash

echo "Checking for updating database..."

#checking for updating the database
if [[ -z "${AWS_ACCESS_KEY_ID}" ]]; then
  echo "AWS_ACCESS_KEY_ID is not set. Continuing without updating database."
else
  dotnet ef database update
fi

