#!/usr/bin/env bash

set -xe

cd ../

MOD_NAME="UILib"
VERSION="$(git describe --abbrev=0 | tr -d  "v")"

BP_NAME="$MOD_NAME-$VERSION-BepInEx"
BP_DIR="build/$BP_NAME"

EXTRA_ARGS=""

if [ "$1" = "gog" ]; then
    BP_NAME="$BP_NAME-GOG"
    EXTRA_ARGS="-p:ExtraConstants=GOG"
fi

dotnet build -c Release "$EXTRA_ARGS"

mkdir -p "$BP_DIR"/plugins

# BepInEx
cp bin/release/net472/"$MOD_NAME.dll" \
    "$BP_DIR/plugins/"
cp build/README.txt "$BP_DIR/README.txt"

# Zip everything
pushd "$BP_DIR"
zip -r ../"$BP_NAME.zip" .
popd

# Remove directories
rm -rf "$BP_DIR"
