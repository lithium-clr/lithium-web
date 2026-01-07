#!/bin/sh
set -e

CERT_PATH=/home/app/certs/lithium.pfx
DATA_KEYS=/home/app/.aspnet/DataProtection-Keys

# Créer les dossiers si absent
mkdir -p "$CERT_PATH" "$(dirname "$CERT_PATH")"
mkdir -p "$DATA_KEYS"

# Générer certificat self-signed si absent
if [ ! -f "$CERT_PATH" ]; then
    echo "Generating self-signed certificate..."
    openssl req -x509 -nodes -days 365 \
        -subj "/CN=localhost" \
        -newkey rsa:2048 \
        -keyout /tmp/lithium.key \
        -out /tmp/lithium.crt
    openssl pkcs12 -export -out "$CERT_PATH" -inkey /tmp/lithium.key -in /tmp/lithium.crt -passout pass:ChangeMe
    rm /tmp/lithium.key /tmp/lithium.crt
fi

# Fix permissions
chmod 400 "$CERT_PATH"
chmod 700 "$DATA_KEYS"

# Lancer l'app
exec dotnet Lithium.Web.dll
