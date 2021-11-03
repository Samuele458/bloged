#!/bin/bash

#checking for updating the database
if [[ -z "${BLOGED_DB_CONN}" ]]; then
  dotnet ef database update
else
  echo "BLOGED_DB_CONN is not set. Continuing without updating database."
fi

