#!/bin/bash

#checking for updating the database


if [[ -z "${BLOGED_DB_CONN}" ]]; then
  echo "BLOGED_DB_CONN is not set. Continuing without updating database."
else
  dotnet ef database update
fi

