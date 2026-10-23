#!/usr/bin/env sh
set -eu
MAVEN_VERSION="3.9.16"
if command -v mvn >/dev/null 2>&1; then
  exec mvn "$@"
fi
MAVEN_HOME="${HOME}/.m2/wrapper/dists/apache-maven-${MAVEN_VERSION}"
if [ ! -x "${MAVEN_HOME}/bin/mvn" ]; then
  BASE="${HOME}/.m2/wrapper/dists"
  ARCHIVE="apache-maven-${MAVEN_VERSION}-bin.tar.gz"
  URL="https://dlcdn.apache.org/maven/maven-3/${MAVEN_VERSION}/binaries/${ARCHIVE}"
  mkdir -p "$BASE"
  TMP="$(mktemp -d)"
  trap 'rm -rf "$TMP"' EXIT
  echo "Maven ${MAVEN_VERSION} is not installed. Downloading Maven automatically..."
  if command -v curl >/dev/null 2>&1; then
    curl -fL "$URL" -o "${TMP}/${ARCHIVE}"
  elif command -v wget >/dev/null 2>&1; then
    wget -O "${TMP}/${ARCHIVE}" "$URL"
  else
    echo "ERROR: install curl or wget to download Maven." >&2
    exit 1
  fi
  tar -xzf "${TMP}/${ARCHIVE}" -C "$BASE"
fi
exec "${MAVEN_HOME}/bin/mvn" "$@"
