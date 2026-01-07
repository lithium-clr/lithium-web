#!/bin/sh
if [ -f /home/app/certs/lithium.pfx ]; then
    chown $APP_UID:$APP_UID /home/app/certs/lithium.pfx
    chmod 400 /home/app/certs/lithium.pfx
fi

if [ -d /home/app/.aspnet/DataProtection-Keys ]; then
    chown -R $APP_UID:$APP_UID /home/app/.aspnet/DataProtection-Keys
    chmod 700 /home/app/.aspnet/DataProtection-Keys
fi

exec dotnet Lithium.Web.dll
