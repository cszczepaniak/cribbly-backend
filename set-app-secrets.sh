#!/bin/bash

# this script is an example of the commands needed to set all dotnet user secrets. you can
# replace the secrets below with your own, or pass them into this script. just don't commit
# any secrets.

# usage: sh set-app-secrets.sh <firebaseAuthAudience>

dotnet user-secrets set FirebaseAuth:Audience "$2" --project src/CribblyBackend