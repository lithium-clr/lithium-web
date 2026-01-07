#!/bin/sh
set -e

if [ ! -f /app/cert.pfx ]; then
  echo "$TLS_CERT_BASE64" | base64 -d > /app/cert.pfx
fi

exec dotnet Lithium.Web.dll
